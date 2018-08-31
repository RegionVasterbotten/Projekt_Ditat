using System;
using System.IO;
using Ditat.Logic.Models;
using Ditat.Logic.Providers;
using Ditat.Data.EntityModels;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Collections.Generic;

namespace Ditat.Logic.Managers
{
    public static class ProductManager
    {
        public static ProductImageDTO InsertProductImage(Guid ProductId, HttpPostedFile file, bool PrimaryImage)
        {
            string ImagePath = Properties.Settings.Default.ImagePath;
            
            var dbProductImage = new ProductImage();
            var productImageDTO = new ProductImageDTO();
            
            int categoryId = 0;
            ClassifierResponse classifierResponse = null;

            string ProductName = string.Empty;
            string imagePath = "";            

            string productProperties = "{}";

            //Save image to disk            
            if (file != null && file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);

                dbProductImage.Id = Guid.NewGuid();
                dbProductImage.Inserted = DateTime.Now;
                dbProductImage.SelectedForExport = true;

                var path = ImagePath + dbProductImage.Id + "_" + fileName;
                
                var inp = file.InputStream;

                var savedProduct = GetProduct(ProductId);

                SaveOriginalImage(file, ImagePath, savedProduct.Id);

                //Save with number
                var cvProvider = new CVProvider();
                cvProvider.SaveImageWithProductNumber(new FileInfo(path), savedProduct.ProductNumber, inp);

                dbProductImage.ProductId = ProductId;
                dbProductImage.PrimaryImage = PrimaryImage;

                imagePath = "Images/" + Path.GetFileName(path);
                dbProductImage.Path = imagePath;

                //Classify image if not classified

                var prevCategory = savedProduct.Category;

                if (prevCategory == null)
                {
                    var classifierProvider = new ClassifierProvider();
                    classifierResponse = classifierProvider.ClassifyImage(path);

                    dbProductImage.Category = classifierResponse.Category;
                    dbProductImage.ClassifierPropability = classifierResponse.CategoryPorpability;
                    //ToDo: Check i low propability 
                }
                else
                {
                    dbProductImage.Category = prevCategory;
                    dbProductImage.ClassifierPropability = 0;
                }

                using (var db = new DitatEntities())
                {
                    db.ProductImages.Add(dbProductImage);
                    db.SaveChanges();

                    productImageDTO.Category = dbProductImage.Category;
                    switch (dbProductImage.Category)
                    {
                        case "DVD":
                            categoryId = 2;

                            var eanAttributeProvider = new EANAttributeProvider();
                            EANInfo eanInfo = eanAttributeProvider.GetEANInfo(path);

                            productProperties = "[]"; //Only name is provided from ean-search.org
                            ProductName = eanInfo.name;

                            productImageDTO.ProductProperties = productProperties; 
                            productImageDTO.Name = ProductName;
                            productImageDTO.ErrorMessage = eanInfo.ErrorMessage;

                            /*
                            productProperties = @"[{""Property"":""Artist"",""Value"":""" + "Test" + @"""},{""Property"":""År"",""Value"":""" + "1234" + @"""}]";
                            ProductName = dbProductImage.Category + " - " + (dbProductImage.ClassifierPropability * 100).ToString() + "%, ej klart";
                            productImageDTO.ProductProperties = productProperties;
                            productImageDTO.Name = ProductName;
                            */

                            break;
                        case "CD":
                            string format = string.Empty;
                            string code = string.Empty;

                            categoryId = 3;

                            var cdAttributeProvider = new CDAttributeProvider();
                            CDInfo cdInfo = cdAttributeProvider.GetCDInfo(path);

                            productProperties = cdInfo.ProductProperties;

                            ProductName = cdInfo.Title;

                            productImageDTO.ProductProperties = productProperties;
                            productImageDTO.Name = ProductName;
                            productImageDTO.ErrorMessage = cdInfo.ErrorMessage;

                            break;
                        default:
                            if (savedProduct.Name == "")
                            {
                                ProductName = dbProductImage.Category + " - " + (dbProductImage.ClassifierPropability * 100).ToString() + "%, ej klart";
                            }
                            break;
                    }

                    if (dbProductImage.Category == "Bok" && (savedProduct.Name == "Bok" || savedProduct.Name == ""))
                    {
                        categoryId = 1; //Bok

                        var bookAttributeProvider = new BookAttributeProvider();
                        var bookInfo = bookAttributeProvider.GetBookInfo(path);

                        productProperties = bookInfo.ProductProperties;

                        ProductName = bookInfo.Title;

                        productImageDTO.ProductProperties = productProperties;
                        productImageDTO.Name = bookInfo.Title;
                        productImageDTO.ErrorMessage = bookInfo.ErrorMessage;

                    }

                    if (dbProductImage.Category == "Kruka")
                    {
                        categoryId = 7;
                        ProductName = "Kruka";

                        productProperties = @"[{ ""Key"":""Width"",""Value"":"""",""Type"":""number"",""Icon"":""panorama_horizontal""},{ ""Key"":""Height"",""Value"":"""",""Type"":""number"",""Icon"":""panorama_vertical""}]";

                        var imageDimensionResponse = ExtractDimensions(new FileInfo(path));

                        //Save processed images if they exists
                        if (imageDimensionResponse.ImageWithDimensions != null)
                        {
                            var dbProductImageWithDimensions = new ProductImage();

                            dbProductImageWithDimensions.Id = Guid.NewGuid();

                            path = ImagePath + dbProductImageWithDimensions.Id + "_" + fileName;

                            imageDimensionResponse.ImageWithDimensions.Save(path);

                            dbProductImageWithDimensions.ProductId = ProductId;
                            dbProductImageWithDimensions.PrimaryImage = false;
                            dbProductImageWithDimensions.Path = "Images/" + Path.GetFileName(path);
                            dbProductImageWithDimensions.ProductId = ProductId;
                            dbProductImageWithDimensions.PrimaryImage = false;
                            dbProductImageWithDimensions.SelectedForExport = true;
                            db.ProductImages.Add(dbProductImageWithDimensions);
                            db.SaveChanges();

                            var dbProductImageCropped = new ProductImage();
                            dbProductImageCropped.Id = Guid.NewGuid();

                            path = ImagePath + dbProductImageCropped.Id + "_crop" + fileName;

                            //imageDimensionResponse.CroppedImage.Save(path);

                            //Save with number
                            cvProvider.SaveImageWithProductNumber(new FileInfo(path), savedProduct.ProductNumber, imageDimensionResponse.CroppedImage);

                            dbProductImageCropped.ProductId = ProductId;
                            dbProductImageCropped.PrimaryImage = true;
                            dbProductImageCropped.Path = "Images/" + Path.GetFileName(path);
                            dbProductImageCropped.ProductId = ProductId;
                            dbProductImageCropped.PrimaryImage = false;
                            dbProductImageCropped.SelectedForExport = true;
                            db.ProductImages.Add(dbProductImageCropped);
                            db.SaveChanges();

                            //Todo: Set Primary image
                            SetPrimaryImage(dbProductImageCropped.Id);

                            //Return cropped image
                            imagePath = dbProductImageCropped.Path;

                            Decimal width = Math.Round((Decimal)imageDimensionResponse.Width);
                            Decimal height = Math.Round((Decimal)imageDimensionResponse.Height);
                            //productProperties = @"[{""Property"":""Höjd"",""Value"":""" + height.ToString() + @" cm""},{""Property"":""Bredd"",""Value"":""" + width.ToString() + @" cm""}]";
                            //Höjd: "panorama_vertical" Bredd: "panorama_horizontal"
                            productProperties = @"[{ ""Key"":""Height"",""Value"":""" + height.ToString() + @""",""Type"":""number"",""Icon"":""panorama_vertical""},{ ""Key"":""Width"",""Value"":""" + width.ToString() + @""",""Type"":""number"",""Icon"":""panorama_horizontal""}]";
                        }
                        else
                        {
                            productImageDTO.ErrorMessage = imageDimensionResponse.ErrorMessage;
                        }
                    }
                }
            }

            //Update Product information
            using (var db = new DitatEntities())
            {
                Product dbProduct = db.Products.Single(p => p.Id == ProductId);

                productImageDTO.Id = dbProductImage.Id;
                productImageDTO.Path = imagePath;
                //productImageDTO.PrimaryImage = dbProductImage.PrimaryImage;

                productImageDTO.ProductProperties = productProperties;
                productImageDTO.Name = ProductName;
                productImageDTO.Category = dbProduct.Category;

                if (ProductName != null || ProductName != "" || ProductName != String.Empty)
                {
                    if (ProductName == null) { ProductName = ""; }
                    if (ProductName.Length > 1) 
                    {
                        dbProduct.Name = ProductName;
                        if (!HasProperties(dbProduct.Properties)) { dbProduct.Properties = productProperties; }
                    }
                }
                if (dbProductImage.Category != null)
                {
                    dbProduct.Category = dbProductImage.Category;//classifierResponse.Category;                               
                }

                if (categoryId > 0) { dbProduct.CategoryId = categoryId; }

                //Fel:  Skriver över properties om man lägger till en odefinierad bil till en befintlig bok:
                if (productImageDTO.ErrorMessage == null || dbProduct.Properties == null)
                {
                    if (HasProperties(dbProduct.Properties) == false)
                    {
                        dbProduct.Properties = productProperties;
                    }                    
                }
                db.SaveChanges();
            }

            return productImageDTO;
        }

        private static void SaveOriginalImage(HttpPostedFile file, string imagePath, Guid productId)
        {
            // Save undistorted image with SelectedForExport = false
            var dbProductImage = new ProductImage
            {
                Id = Guid.NewGuid(),
                Inserted = DateTime.Now,
                SelectedForExport = false,
                ProductId = productId,                
            };

            var path = imagePath + dbProductImage.Id + ".jpg";

            dbProductImage.Path = "Images/" + Path.GetFileName(path);
            
            file.SaveAs(path);

            using (var db = new DitatEntities())
            {
                db.ProductImages.Add(dbProductImage);
                db.SaveChanges();
            }
        }


        private static string PublishProduct(Guid id)
        {

            var shopProvider = new ShopProvider();            
            var savedProduct = GetProduct(id);

            var productImages = GetProductImages(id);

            var imageList = new List<string>();

            foreach (var productImage in productImages)
            {
                if (productImage.SelectedForExport == true)
                {
                    imageList.Add(productImage.Path);
                }
            }

            //Validate
            int price = Convert.ToInt16(savedProduct.Price);
            if (price < 2) { throw new NotImplementedException(); }
            if (imageList.Count < 1) { throw new NotImplementedException(); }

            //Todo: savedProduct.ProductCondition

            string description = savedProduct.Name + "\n\n";
            
            var propertyList = new JavaScriptSerializer().Deserialize<List<ProductProperty>>(savedProduct.Properties);

            foreach (var prop in propertyList)
            {
                description += prop.Key + ": " + prop.Value + "\n";
            }
                        
            if (savedProduct.ProductConditionId.HasValue)
            {
                description += "\n\nSkick (1-5): " + savedProduct.ProductConditionId.ToString();
            }

            description += "\n\n" + savedProduct.Comment;


            //344480 = Testauktioner
            int shopCategoryId = 344480;

            //Get category info
            using (var db = new DitatEntities())
            {                
                Category category = db.Categories.Single(c => c.ClassifierTag == savedProduct.Category);

                //Todo: uncomment and test this: shopCategoryId = category.Id;   
                shopCategoryId = Int32.Parse( category.ExternalId);
            }


            var productUrl = shopProvider.PublishItem(id, savedProduct.Name, description, shopCategoryId, price, 1, 14, imageList, savedProduct.Shipping, savedProduct.ShippingCost, savedProduct.ProductNumber);

            return productUrl;
        }

        public static void DeleteProductImage(Guid id)
        {
            using (var db = new DitatEntities())
            {
                ProductImage productImage = db.ProductImages.Find(id);

                productImage.SelectedForExport = false;

                db.SaveChanges();
            }
        }

        public static bool HasProperties(string properties)
        {
            
            bool hasValue = false;

            if (properties != null)
            {
                var propertyList = new JavaScriptSerializer().Deserialize<List<ProductPropertyDTO>>(properties);

                foreach (var property in propertyList)
                {
                    if (property.Value.Length > 0)
                    {
                        hasValue = true;
                    }
                }
            }
            
            return hasValue;
        }

        public static void UpdateProductImage(ProductImage productImage)
        {
            using (var db = new DitatEntities())
            {
                ProductImage dbProductImage;
                dbProductImage = db.ProductImages.Single(p => p.Id == productImage.Id);
                dbProductImage.SelectedForExport = productImage.SelectedForExport;
                dbProductImage.PrimaryImage = productImage.PrimaryImage;

                db.SaveChanges();
            }

            if (productImage.PrimaryImage)
            {
                using (var db = new DitatEntities())
                {
                    db.SetPrimaryImage(productImage.Id);
                }
            }
            
        }

        public static void SetPrimaryImage(Guid id)
        {
            using (var db = new DitatEntities())
            {
                db.SetPrimaryImage(id);
            }
        }

        public static List<Category> GetCategories()
        {
            List<Category> categories;
            using (var db = new DitatEntities())
            {
                
                categories = db.Categories.ToList();
                
            }
            return categories;
        }

        public static void UpdateProduct(Product product, bool isAdmin)
        {
            int? initialStatus;

            using (var db = new DitatEntities())
            {
                Product dbProduct;
                dbProduct = db.Products.Single(p => p.Id == product.Id);
                initialStatus = dbProduct.ProductStatusId;

                dbProduct.Name = product.Name;
                dbProduct.Comment = product.Comment;
                dbProduct.Properties = product.Properties;
                dbProduct.Barcode = product.Barcode;
                dbProduct.Category = product.Category;
                dbProduct.Price = product.Price;
                dbProduct.CategoryId = product.CategoryId;
                dbProduct.ProductConditionId = product.ProductConditionId;
                dbProduct.ProductStatusId = product.ProductStatusId;
                dbProduct.Shipping = product.Shipping;
                dbProduct.ShippingCost = product.ShippingCost;

                db.SaveChanges();
            }
            //Publish to shop if status changes to 2
            if (product.ProductStatusId == 2 && initialStatus != 2)
            {
                if (!isAdmin) {
                    throw new UnauthorizedAccessException("Admin required");
                }
                product.ShopUrl = PublishProduct(product.Id);
                using (var db = new DitatEntities())
                {
                    Product dbProduct;
                    dbProduct = db.Products.Single(p => p.Id == product.Id);
                    dbProduct.ShopUrl = product.ShopUrl;                    
                    db.SaveChanges();
                }
            }
        }

        public static Guid InsertProduct(Guid id, string name)
        {
            using (var db = new DitatEntities())
            {
                Product dbProduct;

                if (db.Products.Any(p => p.Id == id))
                {
                    dbProduct = db.Products.Single(p => p.Id == id);
                    dbProduct.Name = name;
                }
                else
                {
                    var now = DateTime.Now;

                    dbProduct = new Product { Id = Guid.NewGuid(), Name = "", Inserted = now, Shipping = false};
                    db.Products.Add(dbProduct);
                }
                db.SaveChanges();
                return dbProduct.Id;
            }
        }

        public static List<Product> GetProducts()
        {            
            using (var db = new DitatEntities())
            {
                var products = db.Products.ToList();

                return products;
            }
        }

        public static Product GetProduct(Guid id)
        {
            Product product;
            using (var db = new DitatEntities())
            {
                product = db.Products.Find(id);

                return product;
            }
        }

        public static List<ProductImage> GetProductImages(Guid ProductId)
        {
            using (var db = new DitatEntities())
            {
                var productImages = db.ProductImages.Where(l => l.ProductId == ProductId && l.SelectedForExport == true);
                
                return productImages.OrderByDescending(x => x.PrimaryImage).ToList(); 
            }
        }

        public static void ProcessProduct()
        {
            throw new NotImplementedException();
        }

        public static ImageDimensionResponse ExtractDimensions(FileInfo imageFileInfo)
        {
            var cvProvider = new CVProvider();
            return cvProvider.ExtractDimensions(imageFileInfo);
        }

    }

    internal class ProductProperty
    {
        public string Key { get; set; }

        public string Value { get; set; }

        public string Type { get; set; }

        public string Icon { get; set; }
    }
}
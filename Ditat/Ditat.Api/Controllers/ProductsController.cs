using System.Web.Http;
using System.Web.Http.Description;
using NLog;
using Ditat.Api.Models;
using Ditat.Data.EntityModels;
using Ditat.Logic.Managers;
using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Net;
using System.Security.Claims;

namespace Ditat.Api.Controllers
{
    public class ProductsController : ApiController
    {
        private readonly Logger log = LogManager.GetCurrentClassLogger();

        // POST: api/Products
        [ResponseType(typeof(ProductModel))]
        [Authorize]
        public IHttpActionResult PostProduct(ProductModel productModel)
        {
            var id = ProductManager.InsertProduct(productModel.Id, productModel.Name);

            return Ok(id);            
        }

       

        //GET: api/Products/5
        //        [ResponseType(typeof(ProductModel))]
        [Authorize]
        public IHttpActionResult GetProduct(Guid id)
        {
            var product = GetProductDetail(id);

            return Ok(product);
        }

        private static ProductModel GetProductDetail(Guid id)
        {
            var dbProduct = ProductManager.GetProduct(id);
            var dbProductImageList = ProductManager.GetProductImages(id);

            var productImageModelList = new List<ProductImageModel>();

            foreach (var dbProductImage in dbProductImageList)
            {
                var productImageModel = new ProductImageModel
                {
                    Id = dbProductImage.Id,
                    Path = dbProductImage.Path,
                    PrimaryImage = dbProductImage.PrimaryImage
                };
                productImageModelList.Add(productImageModel);
            }

            List<ProductPropertyModel> propertyList = new List<ProductPropertyModel>();
            if (dbProduct.Properties != null)
            {
                propertyList = new JavaScriptSerializer().Deserialize<List<ProductPropertyModel>>(dbProduct.Properties);
            }

            int tmpCost;
            if (dbProduct.ShippingCost == null)
                tmpCost = 0;
            else
                tmpCost = (int)dbProduct.ShippingCost;

            //Todo: Fixa Category Icon!
            var product = new ProductModel
            {
                Id = dbProduct.Id,
                Name = dbProduct.Name,
                Category = dbProduct.Category,
                Price = dbProduct.Price,
                ProductImages = productImageModelList,
                ProductProperties = propertyList,
                CategoryIcon = "bookmark_border",
                Comment = dbProduct.Comment,
                Condition = dbProduct.ProductConditionId,
                Status = dbProduct.ProductStatusId,
                ProductNumber = dbProduct.ProductNumber,
                Shipping = dbProduct.Shipping,
                ShippingCost = tmpCost
            };
            return product;
        }

        // GET: api/Products
        [Authorize]
        public IHttpActionResult GetProduct()
        {
            var products = ProductManager.GetProducts();

            var productList = new List<ProductModel>() ;

            foreach (var product in products)
            {
                var productInfo = GetProductDetail(product.Id);

                productList.Add(productInfo);
            }

            return Ok(productList);
        }

        //PUT: api/Products/5
        [Authorize]
        public IHttpActionResult PutProduct(Guid id, ProductModel productModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != productModel.Id)
            {
                return BadRequest();
            }

            //Check if user is admin
            var identity = (ClaimsIdentity)User.Identity;
            IEnumerable<Claim> claims = identity.Claims;
            bool isAdmin = false;
            foreach (Claim c in claims)
            {
                if ((c.Type == "isAdmin") && (c.Value == "true"))
                {
                    isAdmin = true;
                    break;
                }
            }

            JavaScriptSerializer serializer = new JavaScriptSerializer();

            var prop = serializer.Serialize(productModel.ProductProperties);
            
            var dbProduct = new Product {
                Id = id,
                Name = productModel.Name,
                Category = productModel.Category,
                ProductStatusId = productModel.Status,
                Comment = productModel.Comment,
                Price = productModel.Price,
                ProductConditionId = productModel.Condition,                
                Properties = prop,
                Shipping = productModel.Shipping,
                ShippingCost = productModel.ShippingCost
            };

            try
            {
                ProductManager.UpdateProduct(dbProduct, isAdmin);
            }
            catch (UnauthorizedAccessException ex)
            {
                //Product was to be published but user is not an admin
                return StatusCode(HttpStatusCode.Unauthorized);
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

    }

    












    //    public class ProductsController : ApiController
    //    {
    //        private D_DBEntities db = new D_DBEntities();
    //        private readonly Logger log = LogManager.GetCurrentClassLogger();
    //        // GET: api/Products
    //        [EnableCors(origins: "*", headers: "*", methods: "*")]
    //        public IQueryable<Product> GetProducts()
    //        {
    //            log.Info("GET: api/Products");
    //            return db.Products;
    //        }

    //        // GET: api/Products/5
    //        [ResponseType(typeof(Product))]
    ////        [Authorize]
    //        public IHttpActionResult GetProduct(Guid id)
    //        {
    //            Product product = db.Products.Find(id);
    //            if (product == null)
    //            {
    //                return NotFound();
    //            }

    //            return Ok(product);
    //        }

    //        // PUT: api/Products/5
    //        [ResponseType(typeof(void))]
    ////        [Authorize]
    //        public IHttpActionResult PutProduct(Guid id, Product product)
    //        {
    //            if (!ModelState.IsValid)
    //            {
    //                return BadRequest(ModelState);
    //            }

    //            if (id != product.Id)
    //            {
    //                return BadRequest();
    //            }

    //            db.Entry(product).State = EntityState.Modified;

    //            try
    //            {
    //                db.SaveChanges();
    //            }
    //            catch (DbUpdateConcurrencyException)
    //            {
    //                if (!ProductExists(id))
    //                {
    //                    return NotFound();
    //                }
    //                else
    //                {
    //                    throw;
    //                }
    //            }

    //            return StatusCode(HttpStatusCode.NoContent);
    //        }


    //        // DELETE: api/Products/5
    //        [ResponseType(typeof(Product))]
    ////        [Authorize]
    //        public IHttpActionResult DeleteProduct(Guid id)
    //        {
    //            Product product = db.Products.Find(id);
    //            if (product == null)
    //            {
    //                return NotFound();
    //            }

    //            db.Products.Remove(product);
    //            db.SaveChanges();

    //            return Ok(product);
    //        }

    //        protected override void Dispose(bool disposing)
    //        {
    //            if (disposing)
    //            {
    //                db.Dispose();
    //            }
    //            base.Dispose(disposing);
    //        }

    //        private bool ProductExists(Guid id)
    //        {
    //            return db.Products.Count(e => e.Id == id) > 0;
    //        }
    //    }
}
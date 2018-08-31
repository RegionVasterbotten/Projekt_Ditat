using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using Ditat.Api.Models;
using System.Web;
using Ditat.Data.EntityModels;
using Ditat.Logic.Managers;
using System.Web.Script.Serialization;
using System.Net;

namespace Ditat.Api.Controllers
{
    public class ProductImagesController : ApiController
    {
        //PUT: api/ProductImages/5
        [Authorize]
        public IHttpActionResult PutProductImages(Guid id, ProductImageModel productImageModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != productImageModel.Id)
            {
                return BadRequest();
            }

            
            var dbProductImage = new ProductImage
            {
                Id = id,
                PrimaryImage = productImageModel.PrimaryImage,
                SelectedForExport = productImageModel.SelectedForExport
            };

            ProductManager.UpdateProductImage(dbProductImage);

            return StatusCode(HttpStatusCode.NoContent);
        }

        //        DELETE: api/ProductImages/5
        [Authorize]
        public IHttpActionResult DeleteProductImage(Guid id)
        {

            ProductManager.DeleteProductImage(id);

            return Ok();
        }


        // POST: api/ProductImages
        [ResponseType(typeof(ProductImage))]
        [Authorize]
        //public IHttpActionResult PostProductImage(Guid ProductId, bool PrimaryImage)
        public IHttpActionResult PostProductImage(Guid ProductId, bool PrimaryImage)
        {
            var file = HttpContext.Current.Request.Files.Count > 0 ?
            HttpContext.Current.Request.Files[0] : null;

            var productImageDTO = ProductManager.InsertProductImage(ProductId, file, PrimaryImage);

            var propertyList = new JavaScriptSerializer().Deserialize<List<ProductPropertyModel>>(productImageDTO.ProductProperties);

            var productImage = new ProductImageModel
            {
                Id = productImageDTO.Id,
                PrimaryImage = productImageDTO.PrimaryImage,
                SelectedForExport = productImageDTO.SelectedForExport,
                Path = productImageDTO.Path,                
                ProductProperties = propertyList,
                Name = productImageDTO.Name,
                Category = productImageDTO.Category,
                ErrorMessage = productImageDTO.ErrorMessage
            };

            //if (PrimaryImage)
            //{
            //    ProductManager.SetPrimaryImage(productImageDTO.Id);
            //}
            return Ok(productImage);            
        }

    }
}
//    {

//        // GET: api/ProductImages
////        [Authorize]
//        public IQueryable<ProductImage> GetProductImages()
//        {
//            return db.ProductImages;
//        }

//        // GET: api/ProductImages/5
//        [ResponseType(typeof(ProductImage))]
//        [Authorize]
//        public IHttpActionResult GetProductImage(Guid id)
//        {
//            ProductImage productImage = db.ProductImages.Find(id);
//            if (productImage == null)
//            {
//                return NotFound();
//            }

//            return Ok(productImage);
//        }

//        // PUT: api/ProductImages/5
//        [ResponseType(typeof(void))]
////        [Authorize]
//        public IHttpActionResult PutProductImage(Guid id, ProductImage productImage)
//        {
//            if (!ModelState.IsValid)
//            {
//                return BadRequest(ModelState);
//            }

//            if (id != productImage.Id)
//            {
//                return BadRequest();
//            }

//            db.Entry(productImage).State = EntityState.Modified;

//            try
//            {
//                db.SaveChanges();
//            }
//            catch (DbUpdateConcurrencyException)
//            {
//                if (!ProductImageExists(id))
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

//        private static PredictionEndpoint GetPredictionEndpoint()
//        {
//            var predictionEndpoint = new PredictionEndpoint();
//            predictionEndpoint.ApiKey = "4f0599xxxxxxxx";
//            return predictionEndpoint;
//        }
//        private static Guid GetProjectId()
//        {
//            var trainingApi = new TrainingApi();
//            trainingApi.ApiKey = "deb01f075bbe4d73a380c0d98e129714";

//            var projectName = "Dit";
//            var project = trainingApi.GetProjects().Single(p => p.Name == projectName);

//            return project.Id;
//        }

//        // POST: api/ProductImages
//        [ResponseType(typeof(ProductImage))]
////        [Authorize]

//        public IHttpActionResult PostProductImage(Guid ProductId, bool PrimaryImage)
//        {
//            string ImagePath = "C:\\inetpub\\wwwroot\\ditatapi\\Images\\";

//            if (!ModelState.IsValid)
//            {
//                return BadRequest(ModelState);
//            }

//            ProductImage productImage = new ProductImage();

//            //Save image to disk
//            var file = HttpContext.Current.Request.Files.Count > 0 ?
//            HttpContext.Current.Request.Files[0] : null;

//            if (file != null && file.ContentLength > 0)
//            {
//                var fileName = Path.GetFileName(file.FileName);

//                productImage.Id = Guid.NewGuid();

//                /*
//                var path = Path.Combine(
//                    HttpContext.Current.Server.MapPath("~/Images"),
//                    productImage.Id + "_" + fileName
//                );
//                */
//                var path = ImagePath + productImage.Id + "_" + fileName;

//                file.SaveAs(path);
//                productImage.ProductId = ProductId;
//                productImage.PrimaryImage = PrimaryImage;
//                productImage.Path = "Images/" + Path.GetFileName(path);

//                //Classify/categorize image here

//                var projectId = GetProjectId();

//                var endpoint = GetPredictionEndpoint();

//                var memoryStream = new MemoryStream(File.ReadAllBytes(path));

//                var prediction = endpoint.PredictImage(projectId, memoryStream);

//                var topPrediction = prediction.Predictions.First();

//                //Console.WriteLine($"{image} predicted to be {topPrediction.Tag} ({topPrediction.Probability:P1})");
//                //objWriter.Write(image + delimiter + topPrediction.Tag + delimiter + topPrediction.Probability + "\n");


//                //Check for barcode if category is book/cd/dvd, retrieve name, creator etc for product


//                Product product = db.Products.Find(ProductId);
//                product.Category = topPrediction.Tag;

//                if (product.Category != "Kruka")
//                {
//                    string format = string.Empty;
//                    string code = string.Empty;
//                    Zebra.GetBarcodeFromImageEmgu(path, out format, out code);

//                    if (code != string.Empty)
//                    {
//                        LibrisInfo info = Zebra.GetEAN13Info(code);
//                        if (info != null)
//                        {
//                            if (info.xsearch.list.Count > 0)
//                            {
//                                product.Name = info.xsearch.list[0].title;
//                                product.Creator = info.xsearch.list[0].creator;
//                                product.Created = info.xsearch.list[0].date;
//                                product.Barcode = code;
//                            }
//                        }
//                    }
//                }


//            }

//            db.ProductImages.Add(productImage);

//            try
//            {
//                db.SaveChanges();
//            }
//            catch (DbUpdateException)
//            {
//                if (ProductImageExists(productImage.Id))
//                {
//                    return Conflict();
//                }
//                else
//                {
//                    throw;
//                }
//            }

//            return CreatedAtRoute("DefaultApi", new { id = productImage.Id }, productImage);
//        }

//        // DELETE: api/ProductImages/5
//        [ResponseType(typeof(ProductImage))]
////        [Authorize]
//        public IHttpActionResult DeleteProductImage(Guid id)
//        {
//            ProductImage productImage = db.ProductImages.Find(id);
//            if (productImage == null)
//            {
//                return NotFound();
//            }

//            db.ProductImages.Remove(productImage);
//            db.SaveChanges();

//            return Ok(productImage);
//        }

//        protected override void Dispose(bool disposing)
//        {
//            if (disposing)
//            {
//                db.Dispose();
//            }
//            base.Dispose(disposing);
//        }

//        private bool ProductImageExists(Guid id)
//        {
//            return db.ProductImages.Count(e => e.Id == id) > 0;
//        }
//    }
//}
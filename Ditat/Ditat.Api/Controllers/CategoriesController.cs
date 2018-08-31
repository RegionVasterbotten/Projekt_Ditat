using Ditat.Api.Models;
using Ditat.Logic.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace Ditat.Api.Controllers
{
    public class CategoriesController : ApiController
    {
        //GET: api/Categories
        [Authorize]
        public IHttpActionResult GetCategories()
        {
            var dbCategory = ProductManager.GetCategories();
            var categoryList = new List<CategoryModel>();

            foreach (var c in dbCategory)
            {
                var propertyList = new JavaScriptSerializer().Deserialize<List<ProductPropertyModel>>(c.Properties);

                var category = new CategoryModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Icon = c.Icon,
                    ProductProperties = propertyList,
                    ExternalId = c.ExternalId,
                    ClassifierTag = c.ClassifierTag
                };
                categoryList.Add(category);
            }
            
            return Ok(categoryList);
        }
    }
}

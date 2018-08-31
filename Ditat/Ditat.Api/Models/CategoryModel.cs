using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ditat.Api.Models
{
    public class CategoryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ExternalId { get; set; }
        public List<ProductPropertyModel> ProductProperties { get; set; }
        public string Icon { get; set; }
        public string ClassifierTag { get; set; }
    }
}
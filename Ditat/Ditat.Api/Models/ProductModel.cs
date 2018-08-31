using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ditat.Api.Models
{
    public class ProductModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Category { get; set; }

        public string CategoryIcon { get; set; }

        public string Price { get; set; }

        public int? Condition { get; set; }

        public string Comment { get; set; }

        public int? Status { get; set; }

        public int ProductNumber { get; set; }

        public bool Shipping { get; set; }

        public string shopUrl { get; set; }

        public int ShippingCost { get; set; }

        public List<ProductPropertyModel> ProductProperties { get; set; }

        public List<ProductImageModel> ProductImages { get; set; }        
    }

    public class ProductPropertyModel
    {
        public string Key { get; set; }

        public string Value { get; set; }

        public string Type { get; set; }

        public string Icon { get; set; }
    }
}
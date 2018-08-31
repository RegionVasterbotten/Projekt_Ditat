using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ditat.Api.Models
{
    public class ProductImageModel
    {
        public Guid Id { get; set; }
        
        public bool PrimaryImage { get; set; }

        public string Name { get; set; }

        public string Category { get; set; }

        public bool? SelectedForExport { get; set; }

        public string Path { get; set; }

        public string ErrorMessage { get; set; }

        public List<ProductPropertyModel> ProductProperties { get; set; }
    }
    
}
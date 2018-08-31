using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ditat.Logic.Models
{
    public class ProductImageDTO
    {     
        public Guid Id { get; set; }

        public string Name { get; set; }

        public bool PrimaryImage { get; set; }

        public bool SelectedForExport { get; set; }

        public string Path { get; set; }

        public string ProductProperties { get; set; }

        public string Category { get; set; }
        public string ErrorMessage { get; internal set; }
    }
}

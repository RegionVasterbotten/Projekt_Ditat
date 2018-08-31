using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ditat.Logic.Models
{
    public class BookInfo
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string Date { get; set; }
        public string Barcode { get; set; }
        public string ProductProperties { get; set; }
        public string ErrorMessage { get; set; }
    }
}
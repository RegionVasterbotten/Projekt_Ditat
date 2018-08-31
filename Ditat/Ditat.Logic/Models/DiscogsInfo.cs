using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ditat.Logic.Models
{
    public class Pagination
    {
        public int per_page { get; set; }
        public int items { get; set; }
        public int page { get; set; }
        //        public Urls urls { get; set; }
        public int pages { get; set; }
    }

    public class Community
    {
        public int have { get; set; }
        public int want { get; set; }
    }

    public class Result
    {
        public List<object> style { get; set; }
        public string thumb { get; set; }
        public List<string> format { get; set; }
        public string country { get; set; }
        public List<string> barcode { get; set; }
        public string uri { get; set; }
        public Community community { get; set; }
        public List<string> label { get; set; }
        public string cover_image { get; set; }
        public string catno { get; set; }
        public string year { get; set; }
        public List<string> genre { get; set; }
        public string title { get; set; }
        public string resource_url { get; set; }
        public string type { get; set; }
        public int id { get; set; }
    }

    public class DiscogsInfo
    {
        public Pagination pagination { get; set; }
        public List<Result> results { get; set; }
    }
}

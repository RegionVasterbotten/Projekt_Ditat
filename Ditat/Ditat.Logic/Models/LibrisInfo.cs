using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ditat.Logic.Models
{
    //libris json
    public class XsearchList
    {
        public string title { get; set; }
        public string creator { get; set; }
        public string date { get; set; }
    }

    public class Xsearch
    {
        public int from { get; set; }
        public int to { get; set; }
        public int records { get; set; }
        public List<XsearchList> list { get; set; }
    }

    public class LibrisInfo
    {
        public Xsearch xsearch { get; set; }
    }
}
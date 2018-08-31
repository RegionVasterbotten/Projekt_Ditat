using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ditat.Logic.Models;
using Ditat.Logic.Interfaces;
using System.Net;
using System.Web.Script.Serialization;

namespace Ditat.Logic.Providers
{
    public class EANAttributeProvider : IEANAttributeProvider
    {
        //https://api.ean-search.org/api?token=xxxx&op=barcode-lookup&ean=5099750442227&format=json

        private const string eansearch_token = "xxxx"; //Register and get token here: https://www.ean-search.org/ean-database-api.html

        public EANInfo GetEANInfo(string path)
        {
            string format = string.Empty;
            string code = string.Empty;

            Zebra.GetBarcodeFromImageEmgu(path, out format, out code);

            string jsondata = string.Empty;
            EANInfo[] searchresults = null; // new EANInfo();
            EANInfo root = new EANInfo();

            if (code != string.Empty)
            {
                try
                {
                    using (WebClient client = new WebClient())
                    {
                        client.Encoding = Encoding.UTF8;
                        jsondata = client.DownloadString("https://api.ean-search.org/api?token=" + eansearch_token + "&op=barcode-lookup&format=json&ean=" + code);
                        //jsondata = "[{\"ean\":\"7391772321138\",\"name\":\"Mamma Mu & Kråkan (Import)\"}]\n";
                        //jsondata = "[{\"error\": \"Invalid barcode\"}]\n";
                    }
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    searchresults = serializer.Deserialize<EANInfo[]>(jsondata);
                    if (searchresults.Length > 0)
                    {
                        root = searchresults[0];
                        root.ErrorMessage = "";
                    }
                    else
                    {
                        root.ErrorMessage = "Could not find EAN information (no searchresults)";
                    }
                    if (!string.IsNullOrEmpty(root.error))
                    {
                        root.ErrorMessage = "Could not find EAN information. " + root.error;
                    }
                }
                catch (Exception ex)
                {
                    root.ErrorMessage = "Could not find EAN information. " + ex.Message;
                }
            }
            else
            {
                root.ErrorMessage = "Could not find barcode";
            }
            return root;
        }
    }
}

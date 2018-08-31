using System;
using System.Net;
using System.Web.Script.Serialization;
using System.Text;
using Ditat.Logic.Models;
using ZXing;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ditat.Logic.Providers
{
    public class CDAttributeProvider : ICDAttributeProvider
    {
        private const string discogs_token = "nruimgFCEQZXfqmqyFVXMrjKiIjoaPQwFvDNYaqe"; //Register and get token here: https://www.discogs.com/settings/developers 

        public CDInfo GetCDInfo(string path)
        {
            string format = string.Empty;
            string code = string.Empty;

            Zebra.GetBarcodeFromImageEmgu(path, out format, out code);

            string jsondata = string.Empty;
            DiscogsInfo root = null;
            CDInfo cd = new CDInfo();

            if (code != string.Empty)
            {
                try
                {
                    using (WebClient client = new WebClient())
                    {
                        client.Headers["User-Agent"] = "DitatUserAgent"; //If this header is omitted discogs throws protocol violation exception
                        client.Encoding = Encoding.UTF8;
                        jsondata = client.DownloadString("https://api.discogs.com/database/search?token=" + discogs_token + "&barcode=" + code);
                    }
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    root = serializer.Deserialize<DiscogsInfo>(jsondata);
                    if ((root != null) && (root.results.Count > 0))
                    {
                        cd.Title = root.results[0].title;
                        cd.Year = root.results[0].year;
                        cd.Barcode = code;
                    }
                }
                catch (Exception ex)
                {
                    cd.ErrorMessage = "Could not find CD information";
                }
            }
            else
            {
                cd.ErrorMessage = "Could not find barcode";
            }
            cd.ProductProperties = CDProperties(cd);
            return cd;
        }

        private static string CDProperties(CDInfo cd)
        {
            string year = "";

            if (cd.Year != null)
            {
                year = cd.Year;
            }
            return @"[{""Key"":""Year"",""Value"":"""
                + year + @""" ,""Type"":""text"",""Icon"":""info_outline""}]";
        }

    }
}

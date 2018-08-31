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


namespace Ditat.Logic.Providers
{
    public class BookAttributeProvider : IBookAttributeProvider
    {

        
        public BookInfo GetBookInfo(string path)
        {
            string format = string.Empty;
            string code = string.Empty;

            Zebra.GetBarcodeFromImageEmgu(path, out format, out code);

            string jsondata = string.Empty;
            LibrisInfo root = null;
            BookInfo book = new BookInfo();

            if (code != string.Empty)
            {
                try
                {
                    using (WebClient client = new WebClient())
                    {
                        client.Encoding = Encoding.UTF8;
                        jsondata = client.DownloadString("http://libris.kb.se/xsearch?query=isbn:" + code + "&format=json");
                    }
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    root = serializer.Deserialize<LibrisInfo>(jsondata);
                    if (root != null)
                    {
                        if (root.xsearch.list.Count > 0)
                        {
                            book.Title = root.xsearch.list[0].title;
                            book.Author = root.xsearch.list[0].creator;
                            book.Date = root.xsearch.list[0].date;
                            book.Barcode = code;

                        }
                    }
                }
                catch (Exception ex)
                {
                    book.ErrorMessage = "Could not find book information";
                }
            }
            else
            {
                book.ErrorMessage = "Could not find barcode";
            }
            book.ProductProperties = BookProperties(book);
            return book;
        }

        private static string BookProperties(BookInfo book)
        {
            string author = "";
            string date = "";

            if (book.Author != null) { author = book.Author; }
            if (book.Date != null) { date = book.Date; }

            return  @"[{""Key"":""Author"",""Value"":""" + author
                + @""" ,""Type"":""text"",""Icon"":""person_outline""},{""Key"":""Year"",""Value"":"""
                + date + @""" ,""Type"":""text"",""Icon"":""info_outline""}]";
        }
    }
    public class Zebra
    {
        static Zebra()
        {

        }

        public static void GetBarcodeFromImage(string fname, out string format, out string code)
        {
            using (var barcodeBitmap = (Bitmap)Image.FromFile(fname))
            {
                // detect and decode the barcode inside the bitmap
                IBarcodeReader reader = new BarcodeReader();
                var result = reader.Decode(barcodeBitmap);
                // do something with the result
                if (result != null)
                {
                    format = result.BarcodeFormat.ToString();
                    code = result.Text;
                }
                else
                {
                    format = "";
                    code = "";
                }
            }
        }

        //public static void GetBarcodeImage(string fname)
        //{
        //    //python detect_barcode.py --image C:\Projects\FreeBarcode\sourceCode\Barcode\TestImages\IMG_0204b.jpg
        //    System.Diagnostics.Process process = new System.Diagnostics.Process();
        //    System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
        //    //startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
        //    startInfo.FileName = "cmd.exe";
        //    startInfo.Arguments = "/C python C:\\Projects\\DitåtApi\\DitåtApi\\detect_barcode.py --image \"" + fname + "\"";
        //    startInfo.RedirectStandardOutput = true;
        //    startInfo.UseShellExecute = false;
        //    startInfo.CreateNoWindow = true;
        //    process.StartInfo = startInfo;
        //    process.Start();
        //    process.WaitForExit();
        //    string output = process.StandardOutput.ReadToEnd();
        //}

        private static Rectangle GetBoundingBox(PointF[] points)
        {
            float minx = points[0].X;
            float miny = points[0].Y;
            float maxx = points[0].X;
            float maxy = points[0].Y;
            foreach (PointF p in points)
            {
                if (minx > p.X) minx = p.X;
                if (miny > p.Y) miny = p.Y;
                if (maxx < p.X) maxx = p.X;
                if (maxy < p.Y) maxy = p.Y;
            }
            Rectangle BBox = new Rectangle(new Point((int)minx, (int)miny), new Size((int)(maxx - minx), (int)(maxy - miny)));
            return BBox;
        }

        public static void GetBarcodeFromImageEmgu(string fname, out string format, out string code)
        {
            Image<Bgr, Byte> orgimage = new Image<Bgr, byte>(fname);

            double scaleFactor = 1;
            if (orgimage.Height > 2048) scaleFactor = 2048 / (double)orgimage.Height;

            Image<Bgr, Byte> image = new Image<Bgr, byte>((int)(orgimage.Width * scaleFactor), (int)(orgimage.Height * scaleFactor));

            //image = cv2.resize(image, (0, 0), fx = scaleFactor, fy = scaleFactor, interpolation = cv2.INTER_AREA)
            CvInvoke.Resize(orgimage, image, new Size(0, 0), scaleFactor, scaleFactor, Inter.Area);

            orgimage.Dispose();


            UMat gray = new UMat();
            CvInvoke.CvtColor(image, gray, ColorConversion.Bgr2Gray);


            /*
            gradX = cv2.Sobel(gray, ddepth = cv2.cv.CV_32F, dx = 1, dy = 0, ksize = -1)
            gradY = cv2.Sobel(gray, ddepth = cv2.cv.CV_32F, dx = 0, dy = 1, ksize = -1)
            */
            UMat gradX = new UMat();
            UMat gradY = new UMat();
            CvInvoke.Sobel(gray, gradX, DepthType.Cv8U, 1, 0, -1);
            CvInvoke.Sobel(gray, gradY, DepthType.Cv8U, 0, 1, -1);

            gray.Dispose();

            //pictureBox1.Image = gradY.Bitmap;

            /*
            # subtract the y-gradient from the x-gradient
            gradient = cv2.subtract(gradX, gradY)
            gradient = cv2.convertScaleAbs(gradient)
            */
            UMat gradient = new UMat();
            CvInvoke.Subtract(gradX, gradY, gradient);
            CvInvoke.ConvertScaleAbs(gradient, gradient, 1, 0);

            gradX.Dispose();
            gradY.Dispose();

            //pictureBox1.Image = gradient.Bitmap;

            /*
            # blur and threshold the image
            blurred = cv2.blur(gradient, (9, 9))
            (_, thresh) = cv2.threshold(blurred, 225, 255, cv2.THRESH_BINARY)
            */
            UMat blurred = new UMat();
            UMat thresh = new UMat();
            CvInvoke.Blur(gradient, blurred, new Size(9, 9), new Point(-1, -1));
            CvInvoke.Threshold(blurred, thresh, 88, 255, ThresholdType.Binary);

            //pictureBox1.Image= thresh.Bitmap;
            //return;

            /*
            # construct a closing kernel and apply it to the thresholded image
            kernel = cv2.getStructuringElement(cv2.MORPH_RECT, (21, 7))
            closed = cv2.morphologyEx(thresh, cv2.MORPH_CLOSE, kernel)
            */
            UMat closed = new UMat();
            var kernel = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(21, 7), new Point(-1, -1));
            CvInvoke.MorphologyEx(thresh, closed, MorphOp.Close, kernel, new Point(-1, -1), 1, BorderType.Constant, CvInvoke.MorphologyDefaultBorderValue);

            blurred.Dispose();
            thresh.Dispose();



            //pictureBox1.Image= closed.Bitmap;
            //return;

            /*
            # perform a series of erosions and dilations
            closed = cv2.erode(closed, None, iterations = 4)
            closed = cv2.dilate(closed, None, iterations = 4)
            */
            UMat eroded = new UMat();
            UMat dilated = new UMat();
            CvInvoke.Erode(closed, eroded, null, new Point(-1, -1), 4, BorderType.Constant, CvInvoke.MorphologyDefaultBorderValue);
            CvInvoke.Dilate(eroded, dilated, null, new Point(-1, -1), 4, BorderType.Constant, CvInvoke.MorphologyDefaultBorderValue);

            //pictureBox1.Image= dilated.Bitmap;
            //return;


            /*
            (cnts, _) = cv2.findContours(closed.copy(), cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)
            c = sorted(cnts, key = cv2.contourArea, reverse = True)[0]
            */

            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            CvInvoke.FindContours(dilated, contours, null, RetrType.External, ChainApproxMethod.ChainApproxSimple);
            eroded.Dispose();
            dilated.Dispose();

            double largest_area = 0;
            int largest_contour_index = -1;
            for (int i = 0; i < contours.Size; i++)
            {
                var rect = CvInvoke.MinAreaRect(contours[i]);
                PointF[] points = rect.GetVertices();
                Rectangle BBox = GetBoundingBox(points);

                //Get largest bounding boxes that has width>height
                if (BBox.Width > BBox.Height)
                {
                    double a = CvInvoke.ContourArea(contours[i], false);
                    if (a > largest_area)
                    {
                        largest_area = a;
                        largest_contour_index = i;
                    }
                }
            }

            //PointF[] points = rect.GetVertices();
            var ROIrect = CvInvoke.MinAreaRect(contours[largest_contour_index]);
            PointF[] ROIpoints = ROIrect.GetVertices();
            Rectangle ROIBBox = GetBoundingBox(ROIpoints);

            var extraWidth = (int)(ROIBBox.Width * 0.2);
            var extraHeight = (int)(ROIBBox.Height * 0.2);

            ROIBBox.X -= extraWidth;
            ROIBBox.Y -= extraHeight;

            ROIBBox.Width += extraWidth * 2;
            ROIBBox.Height += extraHeight * 2;

            Bitmap ROIbmp = new Bitmap(ROIBBox.Width, ROIBBox.Height);
            Graphics g = Graphics.FromImage(ROIbmp);
            g.DrawImage(image.ToBitmap(), 0, 0, ROIBBox, GraphicsUnit.Pixel);

            IBarcodeReader reader = new BarcodeReader();
            var result = reader.Decode(ROIbmp);
            // do something with the result
            if (result != null)
            {
                format = result.BarcodeFormat.ToString();
                code = result.Text;
            }
            else
            {
                format = "";
                code = "";
            }

            //ROIbmp.Dispose();
            contours.Dispose();
            image.Dispose();
        }


        public static LibrisInfo GetEAN13Info(string code)
        {
            string jsondata = string.Empty;
            LibrisInfo root = null;
            if (code != string.Empty)
            {
                try
                {
                    using (WebClient client = new WebClient())
                    {
                        client.Encoding = Encoding.UTF8;
                        jsondata = client.DownloadString("http://libris.kb.se/xsearch?query=isbn:" + code + "&format=json");
                    }
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    root = serializer.Deserialize<LibrisInfo>(jsondata);
                }
                catch (Exception ex)
                {

                }
            }
            return root;
        }
    }
}
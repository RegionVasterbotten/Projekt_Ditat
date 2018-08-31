using Ditat.Logic.Interfaces;
using Ditat.Logic.Models;
using System.IO;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Ditat.Logic.Providers
{
    public class CVProvider : ICVProvider
    {
        public void SaveImageWithProductNumber(FileInfo imageFileInfo, int ProductNumber, Bitmap bitmap)
        {            
            SaveWithProductNumber(imageFileInfo, ProductNumber, bitmap);
        }
        public void SaveImageWithProductNumber(FileInfo imageFileInfo, int ProductNumber, Stream InputStream)
        {
            Bitmap bitmap = (Bitmap)Image.FromStream(InputStream);
            SaveWithProductNumber(imageFileInfo, ProductNumber, bitmap);
        }

        private static void SaveWithProductNumber(FileInfo imageFileInfo, int ProductNumber, Bitmap bitmap)
        {
            PointF textLocation = new PointF(230f, 64f);

            string imageFilePath = imageFileInfo.FullName;
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                var rectFToFill = new Rectangle(new Point(60, 60), new Size(340, 115));

                Color customColor = Color.FromArgb(100, Color.DarkGray);

                SolidBrush shadowBrush = new SolidBrush(customColor);
                graphics.FillRectangles(shadowBrush, new RectangleF[] { rectFToFill });

                using (Font arialFont = new Font("Arial", 80))
                {
                    var drawFormat = new StringFormat();
                    drawFormat.Alignment = StringAlignment.Center;

                    graphics.DrawString(ProductNumber.ToString(), arialFont, Brushes.White, textLocation, drawFormat);
                }
            }

            bitmap.Save(imageFilePath);//save the image file
            bitmap.Dispose();
        }

        public ImageDimensionResponse ExtractDimensions(FileInfo imageFileInfo)
        {
            var imageDimensionResponse = new ImageDimensionResponse
            {
                Width = 0,
                Height = 0,
                CroppedImage = null,
                ImageWithDimensions = null
            };

            double refObjectWidth = Properties.Settings.Default.RefObjectWidth;
            double refObjectHeight = Properties.Settings.Default.RefObjectHeight;            //10, 20 eller 30 cm
            double minObjectArea = Properties.Settings.Default.MinObjectArea;            

            //#load the image, convert it to grayscale, and blur it slightly

            //py: image = cv2.imread(args["image"])
            var image = CvInvoke.Imread(imageFileInfo.FullName);

            //py: gray = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)

            var gray = new UMat();
            CvInvoke.CvtColor(image, gray, ColorConversion.Bgr2Gray);

            //py: gray = cv2.GaussianBlur(gray, (7, 7), 0)
            CvInvoke.GaussianBlur(image, gray, new System.Drawing.Size(9, 9), 0);

            //# perform edge detection, then perform a dilation + erosion to close gaps in between object edges
            //py: edged = cv2.Canny(gray, 50, 100)
            UMat edged = new UMat();
            CvInvoke.Canny(gray, edged, 50, 100);

            IInputArray element = null;
            Point anchor = new Point(-1, -1);
            MCvScalar borderValue = new MCvScalar();

            // py:edged = cv2.dilate(edged, None, iterations = 1)
            CvInvoke.Dilate(edged, edged, element, anchor, 1, Emgu.CV.CvEnum.BorderType.Default, borderValue);

            //py: edged = cv2.erode(edged, None, iterations = 1)
            CvInvoke.Erode(edged, edged, element, anchor, 1, BorderType.Default, borderValue);

            //# find contours in the edge map
            // py: cnts = cv2.findContours(edged.copy(), cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)
            VectorOfVectorOfPoint cnts = new VectorOfVectorOfPoint();
            CvInvoke.FindContours(edged.Clone(), cnts, null, RetrType.External, ChainApproxMethod.ChainApproxSimple);

            // py: cnts = cnts[0] if imutils.is_cv2() else cnts[1]
            var cntsPoint = cnts[0]; //Obs! Kanske skall vara 1 isf 0


            //Create dict that and add left X-coordinates for sorting
            var dict = new Dictionary<string, int>();


            if (cnts.Size > 0)
            {
                for (int i = 0; i < cnts.Size; i++)
                {
                    double area = CvInvoke.ContourArea(cnts[i]);


                    var Rect = GetRectangleFomContour(cnts[i]);
                    int xPos = Rect.X;
                    int rectArea = 0;

                    var leadingZero = "0000000000000" + xPos.ToString();
                    var fixedLength = leadingZero.Substring(leadingZero.Length - 8);

                    //Treat small contours as far right
                    if (area > 40)
                    {
                        var contourRect = GetRectangleFomContour(cnts[i]);
                        rectArea = contourRect.Height * contourRect.Width;
                    }
                    //if ((rectArea < minObjectArea) )
                    //if ((rectArea < minObjectArea) || (Rect.Height < 500) || (Rect.Width < 150) )
                    if (rectArea < minObjectArea)
                    {
                        //Prefix too small objects with z to sort them after the interesting objects
                        dict.Add("z" + fixedLength + "-" + i.ToString(), i);
                    }
                    else
                    {
                        //dict.Add(i, xPos);
                        dict.Add(fixedLength + "-" + i.ToString(), i);
                    }

                }
            }

            //var Sorteditems = dict.OrderBy(v => v.Value);

            var list = dict.Keys.ToList();
            list.Sort();

            //// Debug: Loop through keys.
            //foreach (var key in list)
            //{
            //    Console.WriteLine("{0}: {1}", key, dict[key]);
            //    var itemNo = dict[key];
            //    var itemX = key;
            //}


            var objectToMeasureRect = new RectangleInfo(0, 0, 0, 0);

            int refHeightPx = 0;
            int refWidthPx = 0;
            int objectHeightPx = 0;
            int objectWidthPx = 0;
            int objectNo = 0;

            //# loop over the contours individually
            //for (int i = 0; i < cnts.Size; i++)
            foreach (var key in list)
            {
                //var rectNo = int.Parse(Sorteditems.ElementAt(i).Key.ToString());
                var rectNo = dict[key];

                var contour = cnts[rectNo];
                //var Area = CvInvoke.ContourArea(Contour);
                var contourRect = GetRectangleFomContour(contour);
                var area = contourRect.Height * contourRect.Width;

                //Skip contours with small area
                if (area > minObjectArea)
                {
                    objectNo++;

                    ////Debug - Spara en kopia på varge contour
                    //Rectangle rectCont = new Rectangle(contourRect.X, contourRect.Y, contourRect.Width, contourRect.Height);
                    //var imageCont = image.Clone();
                    //var contImage = new Mat(imageCont, rectCont);
                    //var fileName = imageFileInfo.FullName + "_rectNo" + rectNo.ToString() + "_i" + rectNo.ToString() + ".jpg";
                    //Console.WriteLine("bild " + fileName + " har X:" + rectCont.X.ToString());
                    //contImage.Save(fileName);

                    var heightPx = contourRect.Height;
                    var widthPx = contourRect.Width;

                    if (objectNo == 1)
                    {
                        refHeightPx = heightPx;
                        refWidthPx = widthPx;

                        double AspectRatio = contourRect.AspectRatio;
                        double RefAspectRatio = (double)refObjectWidth / (double)refObjectHeight;

                        //Toto: Kolla att det är grönt
                        //var avgColour = CvInvoke.Mean(contour);
                        //Console.WriteLine("Colour: " + avgColour.V0.ToString() + ", " + avgColour.V1.ToString() + ", " + avgColour.V2.ToString() + ", " + avgColour.V3.ToString());

                        if (AspectRatio > (1.4 * RefAspectRatio) || AspectRatio < (0.9 * RefAspectRatio))
                        {
                            objectNo = 0;
                            //Todo Log and handle error
                            //Console.WriteLine("Incorrect aspect ratio for reference object");
                        }
                        

                        //Debug, draw red lines
                        //CvInvoke.Line(image, new Point(contourRect.X, contourRect.Y), new Point(contourRect.X + contourRect.Width, contourRect.Y), new Bgr(80, 80, 250).MCvScalar, 1);
                        //CvInvoke.Line(image, new Point(rect[1].X, rect[1].Y), new Point(rect[2].X, rect[2].Y), new Bgr(80, 80, 250).MCvScalar, 1);

                        if (objectNo == 1)
                        {
                            //Draw a line on left side of ref object
                            CvInvoke.Line(image, contourRect.TopLeft, contourRect.BottomLeft, new Bgr(80, 180, 80).MCvScalar, 1);

                            //Debug
                            //Print Area
                            //CvInvoke.PutText(
                            //   image,
                            //   "Area: " + Math.Round((Decimal)area, 0, MidpointRounding.AwayFromZero).ToString() + " Px2",
                            //   new Point(contourRect.X + 10, contourRect.Y + 50),
                            //   FontFace.HersheyComplexSmall,
                            //   0.7,
                            //   new Bgr(90, 90, 255).MCvScalar);
                            ////Print Aspect
                            //CvInvoke.PutText(
                            //   image,
                            //   "Aspect: " + Math.Round((Decimal)RefAspectRatio, 2, MidpointRounding.AwayFromZero).ToString(),
                            //   new Point(contourRect.X + 10, contourRect.Y + 100),
                            //   FontFace.HersheyComplexSmall,
                            //   0.7,
                            //   new Bgr(90, 90, 255).MCvScalar);

                        }
                    }

                    //Check Flower Pot minimum height                    
                    int minObjectHeight = Properties.Settings.Default.MinObjectHeight;
                    int minObjectWidth = Properties.Settings.Default.MinObjectWidth;

                    if (objectNo > 1 && objectNo < 5 && contourRect.Height > minObjectHeight && contourRect.Width > minObjectWidth)
                    {
                        objectHeightPx = heightPx;
                        objectWidthPx = widthPx;
                        objectToMeasureRect = contourRect;

                        ////Debug
                        ////Print Area
                        //CvInvoke.PutText(
                        //   image,
                        //   "Area: " + Math.Round((Decimal)area, 0, MidpointRounding.AwayFromZero).ToString() + " Px2",
                        //   new Point(contourRect.X + 10, contourRect.Y + 50),
                        //   FontFace.HersheyComplexSmall,
                        //   0.7,
                        //   new Bgr(90, 90, 255).MCvScalar);
                    }
                }
            }
            double objectHeight = 0;
            double objectWidth = 0;

            if (objectNo > 1)
            {
                objectHeight = refObjectHeight * objectHeightPx / refHeightPx;
                //objectWidth = refObjectWidth * objectWidthPx / refWidthPx;

                //testa att använda höjden som referens. Lägg på 5% för att det oftast stämmer bättre.

                objectWidth = 1.05 * refObjectHeight * objectWidthPx / refHeightPx;

                //Crop image
                var cropX = (int)Math.Round(0.75 * objectToMeasureRect.X);
                var cropWidth = (int)(Math.Round(objectWidthPx * 1.9, 0));
                if (cropWidth > (image.Width - cropX)) { cropWidth = image.Width - cropX; }
                var cropY = (int)Math.Round(0.8 * objectToMeasureRect.Y);
                var cropHeight = (int)(Math.Round(objectHeightPx * 1.5, 0));
                if (cropHeight > (image.Height - cropY)) { cropHeight = image.Height - cropY; }

                Rectangle rectCrop = new Rectangle(cropX, cropY, cropWidth, cropHeight);

                var imageToCrop = image.Clone();
                var croppedImage = new Mat(imageToCrop, rectCrop);

                //py: edged = cv2.erode(edged, None, iterations = 1)

                //Print Height
                CvInvoke.PutText(
                   image,
                   "ca: " + Math.Round((Decimal)objectHeight, 0, MidpointRounding.AwayFromZero).ToString() + " cm",
                   new Point(objectToMeasureRect.X - 150, objectToMeasureRect.Y + (objectHeightPx) / 2),
                   FontFace.HersheyComplexSmall,
                   1.0,
                   new Bgr(80, 180, 80).MCvScalar);

                //Print width
                CvInvoke.PutText(
                   image,
                   "ca: " + Math.Round((Decimal)objectWidth, 0, MidpointRounding.AwayFromZero).ToString() + " cm",
                   new Point(objectToMeasureRect.X - 50 + (objectWidthPx) / 2, objectToMeasureRect.Y - 20),
                   FontFace.HersheyComplexSmall,
                   1.0,
                   new Bgr(80, 180, 80).MCvScalar);

                var colour = new Bgr(80, 180, 80).MCvScalar;
                int lineThickness = 2;

                var rec = new RectangleInfo(objectToMeasureRect.X, objectToMeasureRect.Y, objectToMeasureRect.Width, objectToMeasureRect.Height);

                //Draw a frame around the measured object
                CvInvoke.Line(image, rec.TopLeft, rec.TopRight, colour, lineThickness);
                CvInvoke.Line(image, rec.TopRight, rec.BottomRight, colour, lineThickness);
                CvInvoke.Line(image, rec.BottomRight, rec.BottomLeft, colour, lineThickness);
                CvInvoke.Line(image, rec.BottomLeft, rec.TopLeft, colour, lineThickness);

                //    CroppedImage = croppedImage.ToImage<Bgr, Byte>().ToBitmap(),
                //  ImageWithDimensions = image.ToImage<Bgr, Byte>().ToBitmap()

                imageDimensionResponse.Width = objectWidth;
                imageDimensionResponse.Height = objectHeight;
                if (objectWidth > 0)
                {
                    imageDimensionResponse.CroppedImage = croppedImage.ToImage<Bgr, Byte>().ToBitmap();
                    imageDimensionResponse.ImageWithDimensions = image.ToImage<Bgr, Byte>().ToBitmap();
                }
                
                image.Dispose();
                imageToCrop.Dispose();

                return imageDimensionResponse;
            }
            else
            {
                imageDimensionResponse.Width = objectWidth;
                imageDimensionResponse.Height = objectHeight;
                imageDimensionResponse.ErrorMessage = "Could not extract dimensions";
                image.Dispose();
                return imageDimensionResponse;
            }

            
        }

        private static RectangleInfo GetRectangleFomContour(VectorOfPoint contour)
        {
            var minX = contour[0].X;
            var maxX = contour[0].X;
            var minY = contour[0].Y;
            var maxY = contour[0].Y;

            //Loop points and find max and min
            for (int PointNo = 0; PointNo < contour.Size; PointNo++)
            {
                if (contour[PointNo].X < minX) { minX = contour[PointNo].X; }
                if (contour[PointNo].X > maxX) { maxX = contour[PointNo].X; }
                if (contour[PointNo].Y < minY) { minY = contour[PointNo].Y; }
                if (contour[PointNo].Y > maxY) { maxY = contour[PointNo].Y; }
            }

            var rect = new RectangleInfo(minX, minY, maxX - minX, maxY - minY);
            return rect;
        }
    }
}

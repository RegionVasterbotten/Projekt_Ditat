using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace Ditat.Api.Helpers
{
    public class Dimensions
    {
        private static int midpoint(int ptA, int ptB)
        {
            //def midpoint(ptA, ptB):
            // return ((ptA[0] + ptB[0]) * 0.5, (ptA[1] + ptB[1]) * 0.5)

            return 100;
        }

        public static void ExtractDimensions(string imageFileName, out double width, out double height)
        {
            width = 0;
            height = 0;
            //# construct the argument parse and parse the arguments
            //ap = argparse.ArgumentParser()
            //ap.add_argument("-i", "--image", required= True,
            //    help= "path to the input image")
            //ap.add_argument("-s", "--height", type=float, required=True,
            // help="h of the left-most object in the image (in inches)")

            //#load the image, convert it to grayscale, and blur it slightly
            
            //py: image = cv2.imread(args["image"])
            Mat image = CvInvoke.Imread(imageFileName);

            //py: gray = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)
            
            UMat gray = new UMat();
            CvInvoke.CvtColor(image, gray, ColorConversion.Bgr2Gray);

            //py: gray = cv2.GaussianBlur(gray, (7, 7), 0)
            CvInvoke.GaussianBlur(image, gray, new Size(9, 9), 0);

            //# perform edge detection, then perform a dilation + erosion to close gaps in between object edges
            //py: edged = cv2.Canny(gray, 50, 100)
            UMat edged = new UMat();
            CvInvoke.Canny(gray, edged, 50, 100);

            IInputArray element = null;
            Point anchor = new Point(-1, -1);
            MCvScalar borderValue = new MCvScalar();

            // py:edged = cv2.dilate(edged, None, iterations = 1)
            CvInvoke.Dilate(edged, edged, element, anchor, 1, Emgu.CV.CvEnum.BorderType.Default, borderValue);

            //Test:
            CvInvoke.Imwrite("test_edged.jpg", edged);


            //py: edged = cv2.erode(edged, None, iterations = 1)
            
            
        }

    }





}
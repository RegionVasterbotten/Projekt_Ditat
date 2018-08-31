using System.Drawing;

namespace Ditat.Logic.Models
{
    public class ImageDimensionResponse
    {
        public double Width { get; set; }
        public double Height { get; set; }

        public Bitmap ImageWithDimensions { get; set; }

        public Bitmap CroppedImage { get; set; }
        public string ErrorMessage { get; set; }
    }
}

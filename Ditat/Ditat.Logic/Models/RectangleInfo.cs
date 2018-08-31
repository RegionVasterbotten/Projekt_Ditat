using System.Drawing;

namespace Ditat.Logic.Models
{
    class RectangleInfo
    {
        public RectangleInfo(int x, int y, int width, int height)
        {            
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Area = width * Height;
        }

        public int X;

        public int Y { get; set; }

        public int Height { get; set; }

        public int Width { get; set; }

        public Point TopLeft
        {
            get => new Point(X, Y);
        }
        public Point TopRight
        {
            get => new Point(X + Width, Y);
        }
        public Point BottomRight
        {
            get => new Point(X + Width, Y + Height);
        }
        public Point BottomLeft
        {
            get => new Point(X, Y + Height);
        }

        public int Area;

        public double AspectRatio
        {
            get => (double)Width / (double)Height;
        }
    }
}

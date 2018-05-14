using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ImageProcessing.MagicWand.Magic;

namespace ImageProcessing.MagicWand
{
    public class ImageMagicWand
    {
        public static Bitmap MagicCutOut(Bitmap sourceImage, Color color, Point[] points)
        {
            byte toll = 40;

            FloodFiller filter = new FloodFiller
            {
                FillStyle = FloodFillStyle.Queue,
                Tolerance =
                {
                    [0] = toll,
                    [1] = toll,
                    [2] = toll
                },
                FillColor = color,
                Bmp = sourceImage
            };

            points.ToList().ForEach(point => filter.FloodFill(filter.Bmp, point));

            return filter.Bmp;
        }
    }
}
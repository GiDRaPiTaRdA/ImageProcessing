using System.Drawing;
using ImageProcessing.Resize;

namespace ImageProcessing.Resize
{
    public static class ImageResize
    {
        public static Bitmap Crop(Bitmap b, Rectangle r)
        {
            Bitmap nb = new Bitmap(r.Width, r.Height);
            Graphics g = Graphics.FromImage(nb);
            g.DrawImage(b, -r.X, -r.Y);
            return nb;
        }

        public static Bitmap ForceFormatBitmap(Bitmap sourceBitmap, Resolution targetResoutionAspectRatio)
        {
            Resolution target = new Resolution(sourceBitmap.Width, sourceBitmap.Height);

            // Define target resolution and margins
            var targetResolution = targetResoutionAspectRatio.GetResolution(target);
            int marginY = (targetResolution.Y - target.Y) / 2;
            int marginX = (targetResolution.X - target.X) / 2;


            Bitmap resultBitmap = null;

            // Any margins? - Apply margins
            if (marginY != 0 || marginX != 0)
            {
                // X Axis margin
                if (marginX != 0 && marginY == 0)
                {
                    resultBitmap = ImageProcessor.PixelTraversal(new Bitmap(targetResolution.X, targetResolution.Y),
                        (c, x, y) =>
                        {
                            Color color = Color.Transparent;

                            if (x > marginX && x < target.X + marginX)
                            {
                                color = sourceBitmap.GetPixel(x - marginX, y);
                            }

                            return color;
                        });
                }

                // Y Axis margin
                else if (marginY != 0 && marginX == 0)
                {
                    resultBitmap = ImageProcessor.PixelTraversal(new Bitmap(targetResolution.X, targetResolution.Y),
                        (c, x, y) =>
                        {
                            Color color = Color.Transparent;

                            if (y > marginY && y < target.Y + marginY)
                            {
                                color = sourceBitmap.GetPixel(x, y - marginY);
                            }

                            return color;
                        });
                }
            }
            // No margins needed
            else
            {
                resultBitmap = new Bitmap(sourceBitmap);
            }

            return resultBitmap;
        }
    }
}
using System;
using System.Drawing;
using System.Windows.Forms;
using ImageProcessing.Resize;

namespace ImageProcessing.Resize
{
    public static class ImageResize
    {
        /// <summary>
        /// Fixes out of memory exception addin black pixels
        /// </summary>
        /// <param name="img"></param>
        /// <param name="space"></param>
        /// <returns>cropped image</returns>
        public static Bitmap CroppOut(this Bitmap img, Rectangle space)
        {
            Bitmap nb = new Bitmap(space.Width, space.Height);
            Graphics g = Graphics.FromImage(nb);
            g.DrawImage(img, -space.X, -space.Y);
            return nb;
        }

        /// <summary>
        /// Cropes image, in case out of bounds returns null
        /// </summary>
        /// <param name="img"></param>
        /// <param name="cropArea"></param>
        /// <returns></returns>
        public static Bitmap Crop(Image img, Rectangle cropArea)
        {
            Bitmap bmpImage = new Bitmap(img);

            try
            {
                bmpImage = bmpImage.Clone(cropArea, bmpImage.PixelFormat);
            }
            catch (OutOfMemoryException)
            {
                bmpImage = null;
            }

            return bmpImage;
        }

        /// <summary>
        /// Forces image to format via adding space arround
        /// </summary>
        /// <param name="sourceBitmap"></param>
        /// <param name="targetResoutionAspectRatio"></param>
        /// <returns></returns>
        [Obsolete("Need to be rewritten")]
        public static Bitmap ForceFormatBitmapMaximize(Bitmap sourceBitmap, Resolution targetResoutionAspectRatio)
        {
            Resolution target = new Resolution(sourceBitmap.Width, sourceBitmap.Height);

            // Define target resolution and margins
            var targetResolution = targetResoutionAspectRatio.FitMax(target);
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

        /// <summary>
        /// Forces image to format via removing space arround
        /// </summary>
        /// <param name="sourceBitmap"></param>
        /// <param name="ratio"></param>
        /// <returns></returns>
        public static Bitmap ForceFormatBitmapMinimize(Bitmap sourceBitmap, float ratio)
        {
            Resolution target = new Resolution(sourceBitmap.Width, sourceBitmap.Height);

            // Define target resolution and margins
            var targetResolution = Resolution.FitMin(target, ratio);
            int marginY = (targetResolution.Y - target.Y) / 2;
            int marginX = (targetResolution.X - target.X) / 2;

            Rectangle temp = new Rectangle(
               -marginX, // X
               -marginY, // Y 
               targetResolution.X, // Width
               targetResolution.Y // Height
            );

            Bitmap resultBitmap = Crop(sourceBitmap, temp);

            return resultBitmap;
        }
    }
}
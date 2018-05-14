using System;
using System.Drawing;

namespace ImageProcessing.Pixels
{
    public static class PixelSimlifier
    {
        public static Bitmap Simlify(Bitmap bitmap,int simplificationSquareSide)
        {
            double sizeDX = (double)bitmap.Width / simplificationSquareSide;
            double sizeDY = (double)bitmap.Height / simplificationSquareSide;

            int sizeX =  (int)((int)sizeDX != sizeDX ? (int)sizeDX + 1:sizeDX);
            int sizeY = (int)((int)sizeDY != sizeDY ? (int)sizeDY + 1 : sizeDY);

            Bitmap bitmapResult = new Bitmap(sizeX, sizeY);

            for(int y = 0; y< bitmap.Height ;y+=simplificationSquareSide)
            {
                for (int x = 0; x < bitmap.Width; x +=simplificationSquareSide)
                {
                    Color color = GetPixelFromArea(x, y, bitmap, simplificationSquareSide);

                    bitmapResult.SetPixel(x / simplificationSquareSide, y / simplificationSquareSide, color);  
                }
            }

            return bitmapResult;
        }

        private static Color GetPixelFromArea(int x,int y, Bitmap bitmap, int simplificationSquareSide)
        {
            Color color = new Color();


            for(int iy = 0; iy < simplificationSquareSide; iy++)
            {
                for(int ix = 0; ix < simplificationSquareSide; ix++)
                {
                    int realX = ix + x;
                    int realY = iy + y;



                    if (realX < bitmap.Width && realY < bitmap.Height)
                    {
                        Color colorFromBitmap = bitmap.GetPixel(realX, realY);

                        color = MergeColors(
                            color, colorFromBitmap, (brightness1, brightness2) 
                            => (int)(brightness1 + brightness2) /2);
                    }
                }
            }

            return color;
        }

        private static Color MergeColors(Color color1,Color color2,Func<int,int,int> func)
        {
            int a = func(color1.A, color2.A);
            int r = func(color1.R, color2.R);
            int g = func(color1.G, color2.G);
            int b = func(color1.B, color2.B);

            return Color.FromArgb(a, r, g, b);
        }

        private static Color MergeColors(Color color1, Color color2, Func<Color, Color, Color> func)
        {
            Color resultingColor = func(color1, color2);

            return resultingColor;
        }

        public static Bitmap Blur(Bitmap image, int blurSize)
        {
            return Blur(image, new Rectangle(0, 0, image.Width, image.Height), blurSize);
        }

        private static Bitmap Blur(Bitmap image, Rectangle rectangle, int blurSize)
        {
            Bitmap blurred = new Bitmap(image.Width, image.Height);

            // make an exact copy of the bitmap provided
            using (Graphics graphics = Graphics.FromImage(blurred))
                graphics.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height),
                    new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);

            // look at every pixel in the blur rectangle
            for (int xx = rectangle.X; xx < rectangle.X + rectangle.Width; xx++)
            {
                for (int yy = rectangle.Y; yy < rectangle.Y + rectangle.Height; yy++)
                {
                    int avgR = 0, avgG = 0, avgB = 0;
                    int blurPixelCount = 0;

                    // average the color of the red, green and blue for each pixel in the
                    // blur size while making sure you don't go outside the image bounds
                    for (int x = xx; (x < xx + blurSize && x < image.Width); x++)
                    {
                        for (int y = yy; (y < yy + blurSize && y < image.Height); y++)
                        {
                            Color pixel = blurred.GetPixel(x, y);

                            avgR += pixel.R;
                            avgG += pixel.G;
                            avgB += pixel.B;

                            blurPixelCount++;
                        }
                    }

                    avgR = avgR / blurPixelCount;
                    avgG = avgG / blurPixelCount;
                    avgB = avgB / blurPixelCount;

                    // now that we know the average for the blur size, set each pixel to that color
                    for (int x = xx; x < xx + blurSize && x < image.Width && x < rectangle.Width; x++)
                        for (int y = yy; y < yy + blurSize && y < image.Height && y < rectangle.Height; y++)
                            blurred.SetPixel(x, y, Color.FromArgb(avgR, avgG, avgB));
                }
            }

            return blurred;
        }
    }
}

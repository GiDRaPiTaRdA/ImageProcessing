using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace ImageProcessing
{
    public static class ImageProcessor
    {
        internal static int GetOffset(this BitmapData bitmapData, int x, int y)
        {
            var byteOffset = x * 4 +
                             y * bitmapData.Stride;

            return byteOffset;
        }

        public static void PixelTraversal(Bitmap bitmap, Action<Color, int, int> func)
        {
            int byteStepsAxisX = bitmap.Width;
            int byteStepsAxisY = bitmap.Height;

            BitmapData sourceData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];

            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);
            bitmap.UnlockBits(sourceData);

            // Image traversal
            for (int x = 0; x < byteStepsAxisX; x++)
            {
                for (int y = 0; y < byteStepsAxisY; y++)
                {
                    int channelR = pixelBuffer[GetOffset(sourceData, x, y) + 0];
                    int channelG = pixelBuffer[GetOffset(sourceData, x, y) + 1];
                    int channelB = pixelBuffer[GetOffset(sourceData, x, y) + 2];
                    int channelA = pixelBuffer[GetOffset(sourceData, x, y) + 3];

                    func(Color.FromArgb(channelA, channelR, channelG, channelB), x, y);
                }
            }
        }

        public static Bitmap PixelTraversal(Bitmap bitmap, Func<Color,int,int,Color> func)
        {
            int byteStepsAxisX = bitmap.Width;
            int byteStepsAxisY = bitmap.Height;


            BitmapData sourceData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];
            byte[] resultBuffer = new byte[sourceData.Stride * sourceData.Height];

            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);
            bitmap.UnlockBits(sourceData);

            // Image traversal
            for (int x = 0; x < byteStepsAxisX; x++)
            {
                for (int y = 0; y < byteStepsAxisY; y++)
                {
                    int channelR = pixelBuffer[GetOffset(sourceData, x, y) + 0];
                    int channelG = pixelBuffer[GetOffset(sourceData, x, y) + 1];
                    int channelB = pixelBuffer[GetOffset(sourceData, x, y) + 2];
                    int channelA = pixelBuffer[GetOffset(sourceData, x, y) + 3];

                    Color color = func(Color.FromArgb(channelA,channelR,channelG,channelB),x,y);

                    resultBuffer[GetOffset(sourceData, x, y) + 0] = color.R;
                    resultBuffer[GetOffset(sourceData, x, y) + 1] = color.G;
                    resultBuffer[GetOffset(sourceData, x, y) + 2] = color.B;
                    resultBuffer[GetOffset(sourceData, x, y) + 3] = color.A;
                }
            }

            Bitmap resultBitmap = new Bitmap(bitmap.Width, bitmap.Height);

            BitmapData resultData = resultBitmap.LockBits(new Rectangle(0, 0, resultBitmap.Width, resultBitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            Marshal.Copy(resultBuffer, 0, resultData.Scan0, resultBuffer.Length);
            resultBitmap.UnlockBits(resultData);

            return resultBitmap;
        }

        public static void PixelTraversalAdvanced(Bitmap bitmap, Action<Color, BitmapData, byte[], int, int> func, int offsetX = 0, int offsetY = 0)
        {
            int byteStepsAxisX = bitmap.Width - offsetX;
            int byteStepsAxisY = bitmap.Height - offsetY;


            BitmapData sourceData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];

            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);
            bitmap.UnlockBits(sourceData);

            // Image traversal
            for (int x = 0; x < byteStepsAxisX; x++)
            {
                for (int y = 0; y < byteStepsAxisY; y++)
                {
                    int channelR = pixelBuffer[GetOffset(sourceData, x, y) + 0];
                    int channelG = pixelBuffer[GetOffset(sourceData, x, y) + 1];
                    int channelB = pixelBuffer[GetOffset(sourceData, x, y) + 2];
                    int channelA = pixelBuffer[GetOffset(sourceData, x, y) + 3];

                    func(Color.FromArgb(channelA, channelR, channelG, channelB), sourceData, pixelBuffer, x, y);
                }
            }
        }

        public static Bitmap PixelTraversalAdvanced(Bitmap bitmap, Func<Color,BitmapData,byte[], int, int, Color> func, int offsetX = 0, int offsetY = 0)
        {
            int byteStepsAxisX = bitmap.Width - offsetX;
            int byteStepsAxisY = bitmap.Height - offsetY;


            BitmapData sourceData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];
            byte[] resultBuffer = new byte[sourceData.Stride * sourceData.Height];

            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);
            bitmap.UnlockBits(sourceData);

            // Image traversal
            for (int x = 0; x < byteStepsAxisX; x++)
            {
                for (int y = 0; y < byteStepsAxisY; y++)
                {
                    int channelR = pixelBuffer[GetOffset(sourceData, x, y) + 0];
                    int channelG = pixelBuffer[GetOffset(sourceData, x, y) + 1];
                    int channelB = pixelBuffer[GetOffset(sourceData, x, y) + 2];
                    int channelA = pixelBuffer[GetOffset(sourceData, x, y) + 3];

                    Color color = func(Color.FromArgb(channelA,channelR, channelG, channelB), sourceData,pixelBuffer, x, y);

                    resultBuffer[GetOffset(sourceData, x, y) + 0] = color.R;
                    resultBuffer[GetOffset(sourceData, x, y) + 1] = color.G;
                    resultBuffer[GetOffset(sourceData, x, y) + 2] = color.B;
                    resultBuffer[GetOffset(sourceData, x, y) + 3] = color.A;
                }
            }

            Bitmap resultBitmap = new Bitmap(bitmap.Width, bitmap.Height);

            BitmapData resultData = resultBitmap.LockBits(new Rectangle(0, 0, resultBitmap.Width, resultBitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            Marshal.Copy(resultBuffer, 0, resultData.Scan0, resultBuffer.Length);
            resultBitmap.UnlockBits(resultData);

            return resultBitmap;
        }
    }
}
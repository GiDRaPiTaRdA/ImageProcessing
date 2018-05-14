using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using ImageProcessing.Colors;

namespace ImageProcessing.Edges
{
    public static class EdgeDetection
    {
        public static Bitmap Scan(Bitmap bitmap)
            => DefineEdges(bitmap, Mask.Laplacian3X3);
        public static Bitmap ScanDouble(Bitmap bitmap)
            => DefineEdges(bitmap, Mask.Prewitt3X3Horizontal, Mask.Prewitt3X3Vertical);

        public static byte[,] DefineEdges(byte[,] binaryArray, sbyte[,] mask)
        {
            int maskLengthAxisX = mask.GetLength(0);
            int maskLengthAxisY = mask.GetLength(1);

            int byteStepsAxisX = binaryArray.GetLength(0) - maskLengthAxisX;
            int byteStepsAxisY = binaryArray.GetLength(1) - maskLengthAxisY;

            var analizedField = new byte[byteStepsAxisX, byteStepsAxisY];

            // Image traversal
            for (int x = 0; x < byteStepsAxisX; x++)
            {
                for (int y = 0; y < byteStepsAxisY; y++)
                {
                    double color = 0;

                    for (int mx = 0; mx < maskLengthAxisX; mx++)
                    {
                        for (int my = 0; my < maskLengthAxisY; my++)
                        {
                            color += binaryArray[x + mx, y + my] * mask[mx, my];
                        }
                    }

                    analizedField[x, y] = color.StabelizeColorChanel();
                }
            }

            return analizedField;

        }

        public static byte[,] DefineEdges(byte[,] binaryArray, sbyte[,] maskX, sbyte[,] maskY)
        {
            int maskLengthAxisX = maskX.GetLength(0);
            int maskLengthAxisY = maskX.GetLength(1);

            int byteStepsAxisX = binaryArray.GetLength(0) - maskLengthAxisX;
            int byteStepsAxisY = binaryArray.GetLength(1) - maskLengthAxisY;

            var analizedField = new byte[byteStepsAxisX, byteStepsAxisY];

            // Image traversal
            for (int x = 0; x < byteStepsAxisX; x++)
            {
                for (int y = 0; y < byteStepsAxisY; y++)
                {

                    double colorX = 0;
                    double colorY = 0;

                    for (int mx = 0; mx < maskLengthAxisX; mx++)
                    {
                        for (int my = 0; my < maskLengthAxisY; my++)
                        {
                            colorX += binaryArray[x + mx, y + my] * maskX[mx, my];

                            colorY += binaryArray[x + mx, y + my] * maskY[mx, my];
                        }
                    }

                    double totalColor = Math.Sqrt(Math.Pow(colorX, 2) + Math.Pow(colorY, 2));

                    analizedField[x, y] = totalColor.StabelizeColorChanel();

                }
            }

            return analizedField;

        }

        public static Bitmap DefineEdges(Bitmap bitmap, sbyte[,] mask)
        {
            var watch = Stopwatch.StartNew();

            // Mask length
            int maskLengthAxisX = mask.GetLength(0);
            int maskLengthAxisY = mask.GetLength(1);

            // Byte steps to proceed
            int byteStepsAxisX = bitmap.Width - maskLengthAxisX;
            int byteStepsAxisY = bitmap.Height - maskLengthAxisY;

            BitmapData sourceData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            var stride = sourceData.Stride;

            // Buffers
            byte[] pixelBuffer = new byte[stride * sourceData.Height];
            byte[] resultBuffer = new byte[stride * sourceData.Height];

            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);
            bitmap.UnlockBits(sourceData);

            // Image traversal
            for (int x = 0; x < byteStepsAxisX; x++)
            {
                for (int y = 0; y < byteStepsAxisY; y++)
                {
                    int channelR = 0;
                    int channelG = 0;
                    int channelB = 0;

                    // Mask traversal
                    for (int mx = 0; mx < maskLengthAxisX; mx++)
                    {
                        for (int my = 0; my < maskLengthAxisY; my++)
                        {
                            // Get offset by - var offset = x * 4 + y * stride;
                            var offset = (x + mx) * 4 + (y + my) * stride;

                            var maskValue = mask[mx, my];

                            var r = pixelBuffer[offset + 0];
                            var g = pixelBuffer[offset + 1];
                            var b = pixelBuffer[offset + 2];

                            channelR += r * maskValue;
                            channelG += g * maskValue;
                            channelB += b * maskValue;

                        }
                    }

                    // Get offset by - var offset = x * 4 + y * stride;
                    var offsetT = x * 4 + y * stride;

                    // Stabelize colors
                    if (channelR > byte.MaxValue) { channelR = byte.MaxValue; }
                    else if (channelR < byte.MinValue) { channelR = byte.MinValue; }

                    if (channelG > byte.MaxValue) { channelG = byte.MaxValue; }
                    else if (channelG < byte.MinValue) { channelG = byte.MinValue; }

                    if (channelB > byte.MaxValue) { channelB = byte.MaxValue; }
                    else if (channelB < byte.MinValue) { channelB = byte.MinValue; }

                    // Set result
                    resultBuffer[offsetT + 0] = (byte)channelR;
                    resultBuffer[offsetT + 1] = (byte)channelG;
                    resultBuffer[offsetT + 2] = (byte)channelB;
                    resultBuffer[offsetT + 3] = byte.MaxValue;

                }
            }

            // Create image
            Bitmap resultBitmap = new Bitmap(bitmap.Width, bitmap.Height);
            BitmapData resultData = resultBitmap.LockBits(new Rectangle(0, 0, resultBitmap.Width, resultBitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            // Copy bytes to image
            Marshal.Copy(resultBuffer, 0, resultData.Scan0, resultBuffer.Length);
            resultBitmap.UnlockBits(resultData);

            Debug.WriteLine(watch.ElapsedMilliseconds);

            return resultBitmap;

        }

        public static Bitmap DefineEdges(Bitmap bitmap, sbyte[,] maskX, sbyte[,] maskY)
        {
            var watch = Stopwatch.StartNew();

            // Mask length
            int maskLengthAxisX = maskX.GetLength(0);
            int maskLengthAxisY = maskX.GetLength(1);

            // Byte steps to proceed
            int byteStepsAxisX = bitmap.Width - maskLengthAxisX;
            int byteStepsAxisY = bitmap.Height - maskLengthAxisY;

            BitmapData sourceData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            var stride = sourceData.Stride;

            // Buffers
            byte[] pixelBuffer = new byte[stride * sourceData.Height];
            byte[] resultBuffer = new byte[stride * sourceData.Height];

            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);
            bitmap.UnlockBits(sourceData);

            // Image traversal
            for (int x = 0; x < byteStepsAxisX; x++)
            {
                for (int y = 0; y < byteStepsAxisY; y++)
                {
                    int channelXr = 0;
                    int channelXg = 0;
                    int channelXb = 0;

                    int channelYr = 0;
                    int channelYg = 0;
                    int channelYb = 0;

                    // Mask traversal
                    for (int mx = 0; mx < maskLengthAxisX; mx++)
                    {
                        for (int my = 0; my < maskLengthAxisY; my++)
                        {
                            // Get offset by - var offset = x * 4 + y * stride;
                            var offset = (x + mx) * 4 + (y + my) * stride;

                            var maskXvalue = maskX[mx, my];
                            var maskYvalue = maskY[mx, my];

                            var r = pixelBuffer[offset + 0];
                            var g = pixelBuffer[offset + 1];
                            var b = pixelBuffer[offset + 2];

                            channelXr += r * maskXvalue;
                            channelXg += g * maskXvalue;
                            channelXb += b * maskXvalue;

                            channelYr += r * maskYvalue;
                            channelYg += g * maskYvalue;
                            channelYb += b * maskYvalue;
                        }
                    }

                    // Mean square
                    double channelR = Math.Sqrt(channelXr * channelXr + channelYr * channelYr);
                    double channelG = Math.Sqrt(channelXg * channelXg + channelYg * channelYg);
                    double channelB = Math.Sqrt(channelXb * channelXb + channelYb * channelYb);

                    // Get offset by - var offset = x * 4 + y * stride;
                    var offsetT = x * 4 + y * stride;

                    // Stabelize colors
                    if (channelR > byte.MaxValue) { channelR = byte.MaxValue; }
                    else if (channelR < byte.MinValue) { channelR = byte.MinValue; }

                    if (channelG > byte.MaxValue) { channelG = byte.MaxValue; }
                    else if (channelG < byte.MinValue) { channelG = byte.MinValue; }

                    if (channelB > byte.MaxValue) { channelB = byte.MaxValue; }
                    else if (channelB < byte.MinValue) { channelB = byte.MinValue; }


                    // Set result
                    resultBuffer[offsetT + 0] = (byte)channelR;
                    resultBuffer[offsetT + 1] = (byte)channelG;
                    resultBuffer[offsetT + 2] = (byte)channelB;
                    resultBuffer[offsetT + 3] = byte.MaxValue;

                }
            }

            // Create image
            Bitmap resultBitmap = new Bitmap(bitmap.Width, bitmap.Height);
            BitmapData resultData = resultBitmap.LockBits(new Rectangle(0, 0, resultBitmap.Width, resultBitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            // Copy bytes to image
            Marshal.Copy(resultBuffer, 0, resultData.Scan0, resultBuffer.Length);
            resultBitmap.UnlockBits(resultData);

            Debug.WriteLine(watch.ElapsedMilliseconds);

            return resultBitmap;

        }
    }




}
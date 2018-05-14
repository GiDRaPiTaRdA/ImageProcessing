using System;
using System.Drawing;

namespace ImageProcessing.Colors
{
    public static class ColorAnalizer
    {
        internal static byte StabelizeColorChanel(this double chanel)
        {
            if (chanel > byte.MaxValue) { chanel = byte.MaxValue; }
            else if (chanel < byte.MinValue) { chanel = byte.MinValue; }

            return (byte)chanel;
        }

        public static bool[,] AnalizeBool(Bitmap bitmap, Func<Color, bool> func)
        {
            bool[,] byteArray = new bool[bitmap.Width, bitmap.Height];

            ImageProcessor.PixelTraversal(bitmap, (c, x, y) => { byteArray[x, y] = func(c); });

            return byteArray;
        }

        public static byte[,] AnalizeByte(Bitmap bitmap, Func<Color,byte> func)
        {
            byte[,] byteArray = new byte[bitmap.Width, bitmap.Height];

            ImageProcessor.PixelTraversal(bitmap, (c, x, y) => { byteArray[x, y] = func(c); });

            return byteArray;
        }

        public static byte[,] AnalizeByte(Bitmap bitmap, byte filter = byte.MaxValue/2) =>
            AnalizeByte(bitmap, c => Convert.ToByte(GetBrightness(c) >= filter ? byte.MaxValue : byte.MinValue));
        
        public static byte[,] AnalizeByteLikeBool(Bitmap bitmap, byte filter = byte.MaxValue / 2)=>
            AnalizeByte(bitmap, c => Convert.ToByte(GetBrightness(c) >= filter));

        public static bool[,] AnalizeBool(Bitmap bitmap, byte filter = byte.MaxValue / 2)=>
            AnalizeBool(bitmap, c => GetBrightness(c) >= filter);

        public static Bitmap AnalizeShades(Bitmap bitmap, byte shadesNumber = byte.MaxValue)
        {
            if (shadesNumber < 2)
                throw new Exception(nameof(shadesNumber) + " must have value in range [2-255]");

            if (shadesNumber == byte.MaxValue)
                return bitmap;

            Bitmap result = ImageProcessor.PixelTraversal(bitmap, (color, x, y) =>
            {
                byte shadeR = GetShade(color.R, shadesNumber);
                byte shadeG = GetShade(color.G, shadesNumber);
                byte shadeB = GetShade(color.B, shadesNumber);

                return Color.FromArgb(shadeR, shadeG, shadeB);
            });

            return result;
        }

        public static Bitmap AnalizeBackWhite(Bitmap bitmap,double factorR = 1, double factorG = 1, double factorB = 1)
        {
            Bitmap result = ImageProcessor.PixelTraversal(bitmap, (color, x, y) =>
            {
                byte briteness = GetBrightness(color, factorR, factorG, factorB);
                return Color.FromArgb(briteness,briteness,briteness);
            });

            return result;
        }

        #region Help
        private static byte GetShade(byte brightness,byte shadesNumber)
        {
            if (shadesNumber < 2)
                throw new Exception(nameof(shadesNumber) + " must have value in range [2-255]");

            float range = byte.MaxValue / (shadesNumber - 1);

            double floatingStep = brightness / range;
            int step = (int)Math.Round(floatingStep);

            byte shade;
            if (step * range <= byte.MaxValue)
                shade = Convert.ToByte(step * range);
            else
                shade = byte.MaxValue;

            return shade;
        }
        private static byte GetBrightness(Color color, double factorR = 1, double factorG = 1, double factorB = 1)
        {
            byte r = color.R;
            byte g = color.G;
            byte b = color.B;

            byte brightness = Convert.ToByte((r + g + b) / 3);

            return brightness;
        }

        public static string ToString(byte[,] bitmas)
        {
            string result = "";
            for (int x = 0; x < bitmas.GetLength(0); x++)
            {
                if (result != "")
                    result += '\t';
                for (int y = 0; y < bitmas.GetLength(1); y++)
                    result += "[" + bitmas[x, y] + "]";
            }
            result += '\n';
            return result;
        }
        #endregion
    }
}

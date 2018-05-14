namespace ImageProcessing.Edges
{

    internal static class Mask
    {
        public static sbyte[,] Laplacian3X3 => new sbyte[,]
        {
                {-1, -1, -1,},
                {-1, 8, -1,},
                {-1, -1, -1,},
        };

        public static int[,] Laplacian5X5 => new int[,]
        {
                {-1, -1, -1, -1, -1,},
                {-1, -1, -1, -1, -1,},
                {-1, -1, 24, -1, -1,},
                {-1, -1, -1, -1, -1,},
                {-1, -1, -1, -1, -1},
        };

        public static sbyte[,] LaplacianOfGaussian => new sbyte[,]
        {
                {0, 0, -1, 0, 0},
                {0, -1, -2, -1, 0},
                {-1, -2, 16, -2, -1},
                {0, -1, -2, -1, 0},
                {0, 0, -1, 0, 0},
        };

        public static sbyte[,] Gaussian3X3 => new sbyte[,]
        {
                {1, 2, 1,},
                {2, 4, 2,},
                {1, 2, 1,},
        };

        public static int[,] Gaussian5X5Type1 => new int[,]
        {
                {2, 04, 05, 04, 2},
                {4, 09, 12, 09, 4},
                {5, 12, 15, 12, 5},
                {4, 09, 12, 09, 4},
                {2, 04, 05, 04, 2},
        };

        public static sbyte[,] Gaussian5X5Type2 => new sbyte[,]
        {
                {1, 4, 6, 4, 1},
                {4, 16, 24, 16, 4},
                {6, 24, 36, 24, 6},
                {4, 16, 24, 16, 4},
                {1, 4, 6, 4, 1},
        };

        public static sbyte[,] Sobel3X3Horizontal => new sbyte[,]
        {
                {-1, 0, 1,},
                {-2, 0, 2,},
                {-1, 0, 1,},
        };

        public static sbyte[,] Sobel3X3Vertical => new sbyte[,]
        {
                {1, 2, 1,},
                {0, 0, 0,},
                {-1, -2, -1,},
        };

        public static sbyte[,] Prewitt3X3Horizontal => new sbyte[,]
        {
                {-1, 0, 1,},
                {-1, 0, 1,},
                {-1, 0, 1,},
        };

        public static sbyte[,] Prewitt3X3Vertical => new sbyte[,]
        {
                {1, 1, 1,},
                {0, 0, 0,},
                {-1, -1, -1,},
        };


        public static sbyte[,] Kirsch3X3Horizontal => new sbyte[,]
        {
                {5, 5, 5,},
                {-3, 0, -3,},
                {-3, -3, -3,},
        };

        public static sbyte[,] Kirsch3X3Vertical => new sbyte[,]
        {
                {5, -3, -3,},
                {5, 0, -3,},
                {5, -3, -3,},
        };
    }
}
namespace ImageProcessing.Resize
{
    public class Resolution
    {
        public int X { get; }
        public int Y { get; }

        public float AspectRatio => this.X / (float)this.Y;

        public Resolution(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public Resolution GetResolution(Resolution ratioTarget) => GetResolution(this, ratioTarget);

        public Resolution GetResolution(float ratio) => GetResolution(this, ratio);

        public static Resolution GetResolution(Resolution ratioSource, Resolution ratioTarget) =>
            GetResolution(ratioTarget, ratioSource.AspectRatio);

        public static Resolution GetResolution(Resolution ratioTarget, float aspectRatio)
        {
            Resolution resultResolution = ratioTarget;

            if (aspectRatio > ratioTarget.AspectRatio)
                resultResolution = new Resolution((int)(ratioTarget.Y * aspectRatio), ratioTarget.Y);
            else if (aspectRatio < ratioTarget.AspectRatio)
                resultResolution = new Resolution(ratioTarget.X, (int)(ratioTarget.X / aspectRatio));

            return resultResolution;
        }
    }
}
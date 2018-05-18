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

        public Resolution FitMax(Resolution ratioTarget) => FitMax(this, ratioTarget);

        public Resolution FitMax(float ratio) => FitMax(this, ratio);

        public static Resolution FitMax(Resolution ratioSource, Resolution ratioTarget) =>
            FitMax(ratioTarget, ratioSource.AspectRatio);

        public static Resolution FitMax(Resolution ratioTarget, float aspectRatio)
        {
            Resolution resultResolution = ratioTarget;

            if (aspectRatio > ratioTarget.AspectRatio)
                resultResolution = new Resolution((int)(ratioTarget.Y * aspectRatio), ratioTarget.Y);
            else if (aspectRatio < ratioTarget.AspectRatio)
                resultResolution = new Resolution(ratioTarget.X, (int)(ratioTarget.X / aspectRatio));

            return resultResolution;
        }


        public Resolution FitMin(Resolution ratioTarget) => FitMin(this, ratioTarget);

        public Resolution FitMin(float ratio) => FitMin(this, ratio);

        public static Resolution FitMin(Resolution ratioSource, Resolution ratioTarget) =>
            FitMin(ratioTarget, ratioSource.AspectRatio);

        public static Resolution FitMin(Resolution ratioTarget, float ratio)
        {
            Resolution resultResolution = ratioTarget;

            if (ratio > ratioTarget.AspectRatio)
                resultResolution = new Resolution(ratioTarget.X, (int)(ratioTarget.X / ratio));
            else if (ratio < ratioTarget.AspectRatio)
                resultResolution = new Resolution((int)(ratioTarget.Y * ratio), ratioTarget.Y);

            return resultResolution;
        }
    }
}
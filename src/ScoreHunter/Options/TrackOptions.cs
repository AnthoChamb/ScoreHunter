namespace ScoreHunter.Options
{
    public class TrackOptions
    {
        public TrackOptions()
        {
            SustainLength = 0.023;
            MaxHeroPowerCount = -1;
        }

        public TrackOptions(double sustainLength, int maxHeroPowerCount)
        {
            SustainLength = sustainLength;
            MaxHeroPowerCount = maxHeroPowerCount;
        }

        public double SustainLength { get; set; }
        public int MaxHeroPowerCount { get; set; }
    }
}

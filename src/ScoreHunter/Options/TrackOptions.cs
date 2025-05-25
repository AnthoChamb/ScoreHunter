namespace ScoreHunter.Options
{
    public class TrackOptions
    {
        public TrackOptions()
        {
            SustainLength = 0.023;
            SustainBurstLength = 0.2;
            MaxHeroPowerCount = -1;
        }

        public TrackOptions(double sustainLength, double sustainBurstLength, int maxHeroPowerCount)
        {
            SustainLength = sustainLength;
            SustainBurstLength = sustainBurstLength;
            MaxHeroPowerCount = maxHeroPowerCount;
        }

        public double SustainLength { get; set; }
        public double SustainBurstLength { get; set; }
        public int MaxHeroPowerCount { get; set; }
    }
}

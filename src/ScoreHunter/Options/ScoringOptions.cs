namespace ScoreHunter.Options
{
    public class ScoringOptions
    {
        public double PointsPerNote { get; set; } = 50;
        public double PointsPerSustain { get; set; } = 1;
        public int MaxMultiplier { get; set; } = 4;
        public int StreakPerMultiplier { get; set; } = 10;
    }
}

namespace ScoreHunter.Options
{
    public class ScoringOptions
    {
        public ScoringOptions()
        {
            PointsPerNote = 50;
            PointsPerSustain = 1;
            MaxMultiplier = 4;
            StreakPerMultiplier = 10;
        }

        public ScoringOptions(double pointsPerNote, double pointsPerSustain, int maxMultiplier, int streakPerMultiplier)
        {
            PointsPerNote = pointsPerNote;
            PointsPerSustain = pointsPerSustain;
            MaxMultiplier = maxMultiplier;
            StreakPerMultiplier = streakPerMultiplier;
        }

        public double PointsPerNote { get; set; }
        public double PointsPerSustain { get; set; }
        public int MaxMultiplier { get; set; }
        public int StreakPerMultiplier { get; set; }
    }
}

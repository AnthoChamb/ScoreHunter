namespace ScoreHunter.Core.Interfaces
{
    public interface ITempo
    {
        int Ticks { get; }
        double Start { get; }
        int MicroSecondsPerQuarterNote { get; }
        double BeatsPerMinute { get; }

        double TicksToSeconds(int ticks, int ticksPerQuarterNote);
        int SecondsToTicks(double start, int ticksPerQuarterNote);
    }
}

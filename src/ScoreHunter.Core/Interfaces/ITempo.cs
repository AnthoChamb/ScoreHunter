namespace ScoreHunter.Core.Interfaces
{
    public interface ITempo
    {
        int Ticks { get; }
        double Start { get; }
        int MicroSecondsPerQuarterNote { get; }
    }
}

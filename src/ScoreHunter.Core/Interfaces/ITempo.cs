namespace ScoreHunter.Core.Interfaces
{
    public interface ITempo
    {
        int Ticks { get; }
        int MicroSecondsPerQuarterNote { get; }
    }
}

using ScoreHunter.Core.Interfaces;

namespace ScoreHunter.Core
{
    public class Tempo : ITempo
    {
        public Tempo(int ticks, double start, int microSecondsPerQuarterNote)
        {
            Ticks = ticks;
            Start = start;
            MicroSecondsPerQuarterNote = microSecondsPerQuarterNote;
        }

        public int Ticks { get; }
        public double Start { get; }
        public int MicroSecondsPerQuarterNote { get; }
    }
}

using ScoreHunter.Core.Interfaces;

namespace ScoreHunter.Core
{
    public class Tempo : ITempo
    {
        public Tempo(int ticks, int microSecondsPerQuarterNote)
        {
            Ticks = ticks;
            MicroSecondsPerQuarterNote = microSecondsPerQuarterNote;
        }

        public int Ticks { get; }
        public int MicroSecondsPerQuarterNote { get; }
    }
}

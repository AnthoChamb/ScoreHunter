using ScoreHunter.Core.Interfaces;
using System;

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
        private double SecondsPerQuarterNote => MicroSecondsPerQuarterNote / 1_000_000.0;
        public double BeatsPerMinute => 60_000_000.0 / MicroSecondsPerQuarterNote;

        public double TicksToSeconds(int ticks, int ticksPerQuarterNote)
        {
            return Start + (ticks - Ticks) * SecondsPerTicks(ticksPerQuarterNote);
        }

        public int SecondsToTicks(double start, int ticksPerQuarterNote)
        {
            return Ticks + (int)Math.Round((start - Start) / SecondsPerTicks(ticksPerQuarterNote));
        }

        private double SecondsPerTicks(int ticksPerQuarterNote)
        {
            return SecondsPerQuarterNote / ticksPerQuarterNote;
        }
    }
}

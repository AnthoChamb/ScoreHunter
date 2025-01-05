using ScoreHunter.Core.Interfaces;

namespace ScoreHunter.Core
{
    public class TimeSignature : ITimeSignature
    {
        public TimeSignature(int ticks, int numerator, int denominator)
        {
            Ticks = ticks;
            Numerator = numerator;
            Denominator = denominator;
        }

        public int Ticks { get; }
        public int Numerator { get; }
        public int Denominator { get; }
    }
}

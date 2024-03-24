using ScoreHunter.Core.Interfaces;

namespace ScoreHunter.Core
{
    public class Phrase : IPhrase
    {
        public Phrase(double start, double end)
        {
            Start = start;
            End = end;
        }

        public double Start { get; }
        public double End { get; }
    }
}

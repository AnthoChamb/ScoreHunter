using ScoreHunter.Drawing.Abstractions.Interfaces;

namespace ScoreHunter.Drawing
{
    public class Beat : IBeat
    {
        public Beat(int ticks, double start)
        {
            Ticks = ticks;
            Start = start;
        }

        public int Ticks { get; }
        public double Start { get; }
    }
}

using ScoreHunter.Drawing.Abstractions.Interfaces;

namespace ScoreHunter.Drawing
{
    public class Beat : IBeat
    {
        public Beat(int ticks)
        {
            Ticks = ticks;
        }

        public int Ticks { get; }
    }
}

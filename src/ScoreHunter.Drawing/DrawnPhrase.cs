using ScoreHunter.Core.Interfaces;
using ScoreHunter.Drawing.Abstractions.Interfaces;

namespace ScoreHunter.Drawing
{
    public class DrawnPhrase : IDrawnPhrase
    {
        private readonly IPhrase _phrase;

        public DrawnPhrase(IPhrase phrase, int startTicks)
        {
            _phrase = phrase;
            StartTicks = startTicks;
            EndTicks = -1;
        }

        public DrawnPhrase(DrawnPhrase phrase, int startTicks)
        {
            _phrase = phrase._phrase;
            StartTicks = startTicks;
            EndTicks = phrase.EndTicks;
        }

        public int StartTicks { get; }
        public int EndTicks { get; set; }
        public double Start => _phrase.Start;
        public double End => _phrase.End;
    }
}

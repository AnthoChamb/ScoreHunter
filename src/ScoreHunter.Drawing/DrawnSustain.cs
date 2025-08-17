using ScoreHunter.Core;
using ScoreHunter.Core.Interfaces;
using ScoreHunter.Drawing.Abstractions.Interfaces;

namespace ScoreHunter.Drawing
{
    public class DrawnSustain : IDrawnSustain
    {
        private readonly INote _note;

        public DrawnSustain(INote note, int startTicks)
        {
            _note = note;
            StartTicks = startTicks;
            EndTicks = -1;
        }

        public DrawnSustain(DrawnSustain sustain, int startTicks)
        {
            _note = sustain._note;
            StartTicks = startTicks;
            EndTicks = sustain.EndTicks;
        }

        public int StartTicks { get; }
        public int EndTicks { get; set; }
        public double Start => _note.Start;
        public double End => _note.End;
        public Frets Frets => _note.Frets;
        public bool IsSustain => _note.IsSustain;
        public bool IsHopo => _note.IsHopo;
    }
}

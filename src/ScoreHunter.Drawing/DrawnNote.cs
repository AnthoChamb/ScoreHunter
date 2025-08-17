using ScoreHunter.Core;
using ScoreHunter.Core.Interfaces;
using ScoreHunter.Drawing.Abstractions.Interfaces;

namespace ScoreHunter.Drawing
{
    public class DrawnNote : IDrawnNote
    {
        private readonly INote _note;

        public DrawnNote(INote note, int ticks)
        {
            _note = note;
            Ticks = ticks;
        }

        public int Ticks { get; }
        public double Start => _note.Start;
        public double End => _note.End;
        public Frets Frets => _note.Frets;
        public bool IsSustain => _note.IsSustain;
        public bool IsHopo => _note.IsHopo;
    }
}

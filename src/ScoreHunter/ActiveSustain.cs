using ScoreHunter.Core.Interfaces;

namespace ScoreHunter
{
    public class ActiveSustain
    {
        private readonly double _sustainLength;

        public ActiveSustain(INote note, double sustainLength)
        {
            Note = note;
            Position = note.Start;
            _sustainLength = sustainLength;
        }

        public INote Note { get; }
        public double Position { get; private set; }

        public bool MoveNext()
        {
            Position += _sustainLength;
            return Position < Note.End;
        }
    }
}

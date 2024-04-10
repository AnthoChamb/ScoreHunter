using ScoreHunter.Core.Enums;
using ScoreHunter.Core.Interfaces;

namespace ScoreHunter.Core
{
    public class Note : INote
    {
        private readonly NoteFlags _flags;

        public Note(double start, double end, Frets frets) : this(start, end, frets, NoteFlags.None)
        {
        }

        public Note(double start, double end, Frets frets, NoteFlags flags)
        {
            Start = start;
            End = end;
            Frets = frets;
            _flags = flags;
        }

        public double Start { get; }
        public double End { get; }
        public Frets Frets { get; }
        public bool IsSustain => _flags.HasFlag(NoteFlags.Sustain);
        public bool IsHopo => _flags.HasFlag(NoteFlags.Hopo);
    }
}

using ScoreHunter.Core.Interfaces;
using ScoreHunter.Core;
using System.Collections.Generic;
using System.Linq;

namespace ScoreHunter
{
    public class ActiveSustains
    {
        private readonly IList<INote> _notes;

        public ActiveSustains(INote note)
        {
            _notes = new List<INote>();
            Position = note.Start;
            AddNote(note);
        }

        public Frets Frets => _notes.Aggregate(new Frets(), (frets, note) => frets.Add(note.Frets));
        public double Position { get; private set; }
        public double End { get; private set; }

        public void AddNote(INote note)
        {
            _notes.Add(note);

            if (note.End > End)
            {
                End = note.End;
            }
        }

        public bool MoveNext(double sustainLength)
        {
            Position += sustainLength;

            for (var i = 0; i < _notes.Count; i++)
            {
                if (_notes[i].End < Position)
                {
                    _notes.RemoveAt(i);
                    i--;
                }
            }

            return Position < End;
        }
    }
}

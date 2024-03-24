using ScoreHunter.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace ScoreHunter.Core
{
    public class Chord : IChord, IChordNode
    {
        private readonly INoteNode _first;
        private readonly INoteNode _last;

        public Chord(INoteNode first, INoteNode last)
        {
            _first = first;
            _last = last;
        }

        public IEnumerable<INote> Notes
        {
            get
            {
                var current = _first;
                yield return current.Value;

                while (current != _last)
                {
                    current = current.Next;
                    yield return current.Value;
                }
            }
        }

        public double Start => _first.Value.Start;
        public Frets Frets => Notes.Aggregate(new Frets(), (frets, note) => frets.Add(note.Frets));

        public IChord Value => this;
        public IChordNode Next => _last.Next?.GetChordNode();
        public IChordNode Previous => _first.Previous?.GetChordNode();
    }
}

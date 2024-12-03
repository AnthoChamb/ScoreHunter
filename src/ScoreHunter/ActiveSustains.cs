using ScoreHunter.Core.Interfaces;
using ScoreHunter.Core;
using System.Collections.Generic;

namespace ScoreHunter
{
    public class ActiveSustains
    {
        private readonly LinkedList<ActiveSustain> _activeSustains;
        private readonly double _sustainLength;

        public ActiveSustains(INote note, double sustainLength)
        {
            _activeSustains = new LinkedList<ActiveSustain>();
            Position = note.Start;
            _sustainLength = sustainLength;
            AddNote(note);
        }

        public Frets Frets { get; private set; }
        public double Position { get; private set; }
        public double End { get; private set; }

        public void AddNote(INote note)
        {
            var activeSustain = new ActiveSustain(note, _sustainLength);
            _activeSustains.AddFirst(activeSustain);

            if (note.End > End)
            {
                End = note.End;
            }
        }

        public bool MoveNext()
        {
            var activeSustainNode = _activeSustains.First;

            if (activeSustainNode != null)
            {
                var activeSustain = activeSustainNode.Value;
                var frets = new Frets();

                do
                {
                    frets = frets.Add(activeSustain.Note.Frets);
                    _activeSustains.RemoveFirst();
                    if (activeSustain.MoveNext())
                    {
                        _activeSustains.AddLast(activeSustain);
                    }
                } while ((activeSustainNode = activeSustainNode.Next) != null &&
                         (activeSustain = activeSustainNode.Value).Position == Position);

                Frets = frets;
                Position = activeSustain.Position;
            }

            return Position < End;
        }
    }
}

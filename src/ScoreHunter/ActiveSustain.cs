using ScoreHunter.Core.Interfaces;
using System;

namespace ScoreHunter
{
    public class ActiveSustain
    {
        private readonly double _sustainLength;
        private readonly double _sustainBurstPosition;
        private int _count;

        public ActiveSustain(INote note, double sustainLength, double sustainBurstLength)
        {
            Position = note.Start;
            _sustainLength = sustainLength;
            _sustainBurstPosition = note.End - sustainBurstLength;
            _count = (int)Math.Floor((note.End - note.Start) / sustainLength) * note.Frets.Count;
        }

        public int Count { get; private set; }
        public double Position { get; private set; }

        public bool MoveNext()
        {
            if (_count > 0)
            {
                Position += _sustainLength;
                if (Position < _sustainBurstPosition)
                {
                    Count = 1;
                    _count--;
                }
                else
                {
                    Count = _count;
                    _count = 0;
                }
                return true;
            }
            return false;
        }
    }
}

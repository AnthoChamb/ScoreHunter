using ScoreHunter.Core.Interfaces;
using System.Collections.Generic;

namespace ScoreHunter
{
    public class ActiveSustains
    {
        private readonly Queue<ActiveSustain> _activeSustains;
        private readonly double _sustainLength;
        private readonly double _sustainBurstLength;

        public ActiveSustains(double sustainLength, double sustainBurstLength)
        {
            _activeSustains = new Queue<ActiveSustain>(3);
            _sustainLength = sustainLength;
            _sustainBurstLength = sustainBurstLength;
        }

        public int Count { get; private set; }
        public double Position { get; private set; }

        public void AddNote(INote note)
        {
            var activeSustain = new ActiveSustain(note, _sustainLength, _sustainBurstLength);
            _activeSustains.Enqueue(activeSustain);
        }

        public bool MoveNext()
        {
            if (_activeSustains.Count > 0)
            {
                Count = 0;
                ActiveSustain activeSustain;

                do
                {
                    activeSustain = _activeSustains.Dequeue();
                    if (activeSustain.MoveNext())
                    {
                        Count += activeSustain.Count;
                        _activeSustains.Enqueue(activeSustain);
                    }
                } while (_activeSustains.Count > 0 &&
                         _activeSustains.Peek().Position == Position);

                Position = activeSustain.Position;
                return _activeSustains.Count > 0;
            }

            return false;
        }
    }
}

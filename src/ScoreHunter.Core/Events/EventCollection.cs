using System.Collections.Generic;

namespace ScoreHunter.Core.Events
{
    public class EventCollection
    {
        private readonly EventNode _first;

        public EventCollection(EventNode first)
        {
            _first = first;
        }

        public IEnumerable<Event> Events
        {
            get
            {
                var current = _first;

                while (current != null)
                {
                    yield return current.Value;
                    current = current.Next;
                }
            }
        }

        public EventNode GetFirstEventNode() => _first;
    }
}

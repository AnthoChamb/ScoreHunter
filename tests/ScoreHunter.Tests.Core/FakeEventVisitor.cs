using ScoreHunter.Core.Events;
using ScoreHunter.Core.Interfaces;
using System.Collections.Generic;

namespace ScoreHunter.Tests.Core
{
    public class FakeEventVisitor : IEventVisitor
    {
        private readonly IList<Event> _visited = new List<Event>();
        private readonly IList<NoteEvent> _visitedNotes = new List<NoteEvent>();
        private readonly IList<SustainEvent> _visitedSustains = new List<SustainEvent>();
        private readonly IList<HighwayEvent> _visitedHighways = new List<HighwayEvent>();

        public IEnumerable<Event> Visited => _visited;
        public IEnumerable<NoteEvent> VisitedNotes => _visitedNotes;
        public IEnumerable<SustainEvent> VisitedSustains => _visitedSustains;
        public IEnumerable<HighwayEvent> VisitedHighways => _visitedHighways;

        public void Visit(NoteEvent note)
        {
            _visited.Add(note);
            _visitedNotes.Add(note);
        }

        public void Visit(SustainEvent sustain)
        {
            _visited.Add(sustain);
            _visitedSustains.Add(sustain);
        }

        public void Visit(HighwayEvent highway)
        {
            _visited.Add(highway);
            _visitedHighways.Add(highway);
        }
    }
}

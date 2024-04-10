using ScoreHunter.Core.Events;

namespace ScoreHunter.Core.Interfaces
{
    public interface IEventVisitor
    {
        void Visit(NoteEvent note);
        void Visit(SustainEvent sustain);
        void Visit(HighwayEvent highway);
    }
}

using ScoreHunter.Core.Interfaces;

namespace ScoreHunter.Core.Events
{
    public class SustainEvent : Event
    {
        public SustainEvent(double start, int count) : base(start)
        {
            Count = count;
        }

        public int Count { get; }

        public override void Accept(IEventVisitor visitor) => visitor.Visit(this);
    }
}

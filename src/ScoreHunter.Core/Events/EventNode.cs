namespace ScoreHunter.Core.Events
{
    public class EventNode
    {
        public EventNode(Event value)
        {
            Value = value;
        }

        public Event Value { get; }
        public EventNode Next { get; set; }
    }
}

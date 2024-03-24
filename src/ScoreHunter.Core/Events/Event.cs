namespace ScoreHunter.Core.Events
{
    public abstract class Event
    {
        public Event(double start)
        {
            Start = start;
        }

        public double Start { get; }
    }
}

namespace ScoreHunter.Core.Events
{
    public class SustainEvent : Event
    {
        public SustainEvent(double start, Frets frets) : base(start)
        {
            Frets = frets;
        }

        public Frets Frets { get; }
    }
}

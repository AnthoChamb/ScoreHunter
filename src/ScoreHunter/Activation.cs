using ScoreHunter.Core.Events;
using ScoreHunter.Core.Interfaces;

namespace ScoreHunter
{
    public class Activation : IActivation
    {
        public Activation(IHeroPower heroPower, Event @event, int streak, bool isChained)
            : this(heroPower, @event, streak, isChained, @event.Start + heroPower.Duration)
        {
        }

        private Activation(IHeroPower heroPower, Event @event, int streak, bool isChained, double end)
        {
            HeroPower = heroPower;
            Event = @event;
            Streak = streak;
            IsChained = isChained;
            End = end;
        }

        public double Start => Event.Start;
        public double End { get; private set; }
        public IHeroPower HeroPower { get; }
        public Event Event { get; }
        public int Streak { get; }
        public bool IsChained { get; }

        public Activation EndActivation(double end)
        {
            return new Activation(HeroPower, Event, Streak, IsChained, end);
        }

        public void ExtendActivation(double duration)
        {
            End += duration;
        }
    }
}

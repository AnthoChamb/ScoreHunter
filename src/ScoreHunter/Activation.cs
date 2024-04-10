using ScoreHunter.Core.Events;
using ScoreHunter.Core.Interfaces;

namespace ScoreHunter
{
    public class Activation : IActivation
    {
        public Activation(IHeroPower heroPower, Event @event, int streak, bool isChained)
        {
            HeroPower = heroPower;
            Event = @event;
            Streak = streak;
            IsChained = isChained;
        }

        public IHeroPower HeroPower { get; }
        public Event Event { get; }
        public int Streak { get; }
        public bool IsChained { get; }
    }
}

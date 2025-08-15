using ScoreHunter.Core.Events;

namespace ScoreHunter.Core.Interfaces
{
    public interface IActivation
    {
        double Start { get; }
        double End { get; }
        IHeroPower HeroPower { get; }
        Event Event { get; }
        int Streak { get; }
        bool IsChained { get; }
    }
}

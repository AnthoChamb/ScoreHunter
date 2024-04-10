using ScoreHunter.Core.Events;

namespace ScoreHunter.Core.Interfaces
{
    public interface IActivation
    {
        IHeroPower HeroPower { get; }
        Event Event { get; }
        int Streak { get; }
        bool IsChained { get; }
    }
}

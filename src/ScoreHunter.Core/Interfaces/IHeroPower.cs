using ScoreHunter.Core.Events;

namespace ScoreHunter.Core.Interfaces
{
    public interface IHeroPower
    {
        double Duration { get; }
        int Multiplier(int multiplier);
        int MaxMultiplier(int maxMultiplier);
        bool CanActivate(ICandidate candidate, NoteEvent note);
        bool CanActivate(ICandidate candidate, SustainEvent sustain);
    }
}

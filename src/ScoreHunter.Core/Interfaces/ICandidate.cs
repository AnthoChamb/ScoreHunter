using ScoreHunter.Core.Events;
using System.Collections.Generic;

namespace ScoreHunter.Core.Interfaces
{
    public interface ICandidate
    {
        int Score { get; }
        int Streak { get; }
        int Multiplier { get; }
        int MaxMultiplier { get; }
        int StreakPerMultiplier { get; }
        int Miss { get; }

        IEnumerable<IActivation> Activations { get; }

        ICandidate Advance(Event @event);
        ICandidate HitNote(NoteEvent note);
        ICandidate HoldSustain(SustainEvent sustain);
        ICandidate Highway(HighwayEvent highway);

        bool TryActivate(NoteEvent note, out ICandidate candidate);
        bool TryActivate(SustainEvent sustain, out ICandidate candidate);
        bool TryMiss(NoteEvent note, out ICandidate candidate);
        bool TrySetHeroPower(IHeroPower heroPower, out ICandidate candidate);

        ICacheKey GetCacheKey();
    }
}

using ScoreHunter.Core.Events;
using ScoreHunter.Core.Interfaces;
using System.Collections.Generic;

namespace ScoreHunter.Tests.Core
{
    public class FakeCandidate : ICandidate
    {
        public int Score { get; set; }
        public int Streak { get; set; }
        public int Multiplier { get; set; }
        public int MaxMultiplier { get; set; }
        public int StreakPerMultiplier { get; set; }
        public int Miss { get; set; }

        public ICollection<IActivation> Activations { get; set; } = new List<IActivation>();
        IEnumerable<IActivation> ICandidate.Activations => Activations;

        public ICandidate Advance(Event @event) => this;
        public ICandidate Highway(HighwayEvent highway) => this;
        public ICandidate HitNote(NoteEvent note) => this;
        public ICandidate HoldSustain(SustainEvent sustain) => this;

        public bool TryActivate(NoteEvent note, out ICandidate candidate)
        {
            candidate = null;
            return false;
        }

        public bool TryActivate(SustainEvent sustain, out ICandidate candidate)
        {
            candidate = null;
            return false;
        }

        public bool TryMiss(NoteEvent note, out ICandidate candidate)
        {
            candidate = null;
            return false;
        }

        public bool TrySetHeroPower(IHeroPower heroPower, out ICandidate candidate)
        {
            candidate = null;
            return false;
        }

        public ICacheKey GetCacheKey() => null;
    }
}

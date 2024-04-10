using ScoreHunter.Core.Events;
using ScoreHunter.Core.Interfaces;

namespace ScoreHunter.HeroPowers
{
    public class DoubleHeroPower : IHeroPower
    {
        public double Duration => 7;
        public int Multiplier(int multiplier) => multiplier * 2;
        public int MaxMultiplier(int maxMultiplier) => maxMultiplier;
        public bool CanActivate(ICandidate candidate, NoteEvent note) => true;
        public bool CanActivate(ICandidate candidate, SustainEvent sustain) => true;
    }
}

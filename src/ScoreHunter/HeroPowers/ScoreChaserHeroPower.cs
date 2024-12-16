using ScoreHunter.Core.Events;
using ScoreHunter.Core.Interfaces;

namespace ScoreHunter.HeroPowers
{
    public class ScoreChaserHeroPower : IHeroPower
    {
        public double Duration => 14;
        public int Multiplier(int multiplier) => multiplier;
        public int MaxMultiplier(int maxMultiplier) => maxMultiplier * 2;

        public bool CanActivate(ICandidate candidate, NoteEvent note)
        {
            return candidate.Multiplier > candidate.MaxMultiplier ||
                   (candidate.Multiplier == candidate.MaxMultiplier &&
                   (candidate.Streak % candidate.StreakPerMultiplier) == (candidate.StreakPerMultiplier - 1));
        }

        public bool CanActivate(ICandidate candidate, SustainEvent sustain) => false;
    }
}

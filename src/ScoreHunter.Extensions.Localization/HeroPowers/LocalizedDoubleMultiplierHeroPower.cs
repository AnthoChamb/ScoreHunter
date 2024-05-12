using Microsoft.Extensions.Localization;
using ScoreHunter.Core.Events;
using ScoreHunter.Core.Interfaces;
using ScoreHunter.Extensions.Localization.Resources;

namespace ScoreHunter.Extensions.Localization.HeroPowers
{
    public class LocalizedDoubleMultiplierHeroPower : IHeroPower
    {
        private readonly IHeroPower _heroPower;
        private readonly IStringLocalizer<LocalizedDoubleMultiplierHeroPower> _stringLocalizer;

        public LocalizedDoubleMultiplierHeroPower(IHeroPower heroPower, IStringLocalizer<LocalizedDoubleMultiplierHeroPower> stringLocalizer)
        {
            _heroPower = heroPower;
            _stringLocalizer = stringLocalizer;
        }

        public double Duration => _heroPower.Duration;
        public int Multiplier(int multiplier) => _heroPower.Multiplier(multiplier);
        public int MaxMultiplier(int maxMultiplier) => _heroPower.MaxMultiplier(maxMultiplier);
        public bool CanActivate(ICandidate candidate, NoteEvent note) => _heroPower.CanActivate(candidate, note);
        public bool CanActivate(ICandidate candidate, SustainEvent sustain) => _heroPower.CanActivate(candidate, sustain);

        public override string ToString() => _stringLocalizer.GetString(nameof(HeroPowers_LocalizedDoubleMultiplierHeroPower.Name));
    }
}

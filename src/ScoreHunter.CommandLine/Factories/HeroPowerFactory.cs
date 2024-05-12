using Microsoft.Extensions.Localization;
using ScoreHunter.CommandLine.Enums;
using ScoreHunter.Core.Interfaces;
using ScoreHunter.Extensions.Localization.HeroPowers;
using ScoreHunter.HeroPowers;
using System;

namespace ScoreHunter.CommandLine.Factories
{
    public class HeroPowerFactory
    {
        private readonly IStringLocalizerFactory _stringLocalizerFactory;

        public HeroPowerFactory(IStringLocalizerFactory stringLocalizerFactory)
        {
            _stringLocalizerFactory = stringLocalizerFactory;
        }

        public IHeroPower Create(HeroPowerOption option)
        {
            return option switch
            {
                HeroPowerOption.ScoreChaser => new LocalizedScoreChaserHeroPower(
                    new ScoreChaserHeroPower(),
                    new StringLocalizer<LocalizedScoreChaserHeroPower>(_stringLocalizerFactory)),
                HeroPowerOption.DoubleMultiplier => new LocalizedDoubleMultiplierHeroPower(
                    new DoubleMultiplierHeroPower(),
                    new StringLocalizer<LocalizedDoubleMultiplierHeroPower>(_stringLocalizerFactory)),
                _ => throw new ArgumentException(null, nameof(option)),
            };
        }
    }
}

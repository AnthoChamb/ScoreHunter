using ScoreHunter.CommandLine.Enums;
using ScoreHunter.Core.Interfaces;
using ScoreHunter.HeroPowers;
using System;

namespace ScoreHunter.CommandLine.Factories
{
    public class HeroPowerFactory
    {
        public IHeroPower Create(HeroPowerOption option)
        {
            return option switch
            {
                HeroPowerOption.ScoreChaser => new ScoreChaserHeroPower(),
                HeroPowerOption.Double => new DoubleHeroPower(),
                _ => throw new ArgumentException(null, nameof(option)),
            };
        }
    }
}

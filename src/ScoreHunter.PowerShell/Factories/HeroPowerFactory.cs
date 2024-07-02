using ScoreHunter.Core.Interfaces;
using ScoreHunter.HeroPowers;
using ScoreHunter.PowerShell.Enums;
using System;

namespace ScoreHunter.PowerShell.Factories
{
    public class HeroPowerFactory
    {
        public IHeroPower Create(HeroPowerParameter parameter)
        {
            switch (parameter)
            {
                case HeroPowerParameter.ScoreChaser:
                    return new ScoreChaserHeroPower();
                case HeroPowerParameter.DoubleMultiplier:
                    return new DoubleMultiplierHeroPower();
                default:
                    throw new ArgumentException(null, nameof(parameter));
            }
        }
    }
}

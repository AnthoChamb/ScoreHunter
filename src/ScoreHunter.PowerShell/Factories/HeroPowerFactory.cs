using CommunityToolkit.Diagnostics;
using ScoreHunter.Core.Interfaces;
using ScoreHunter.HeroPowers;
using ScoreHunter.PowerShell.Enums;

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
                    return ThrowHelper.ThrowArgumentException<IHeroPower>(nameof(parameter), null, null);
            }
        }
    }
}

using ScoreHunter.Core.Interfaces;

namespace ScoreHunter.HeroPowers
{
    public class ScoreChaserHeroPower : IHeroPower
    {
        public int Multiplier(int multiplier) => multiplier;
        public int MaxMultiplier(int maxMultiplier) => maxMultiplier * 2;
    }
}

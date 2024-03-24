using ScoreHunter.Core.Interfaces;

namespace ScoreHunter.HeroPowers
{
    public class DoubleHeroPower : IHeroPower
    {
        public int Multiplier(int multiplier) => multiplier * 2;
        public int MaxMultiplier(int maxMultiplier) => maxMultiplier;
    }
}

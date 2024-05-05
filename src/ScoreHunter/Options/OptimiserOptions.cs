using ScoreHunter.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace ScoreHunter.Options
{
    public class OptimiserOptions
    {
        public OptimiserOptions()
        {
            HeroPowers = Enumerable.Empty<IHeroPower>();
            MaxMiss = -1;
        }

        public OptimiserOptions(IEnumerable<IHeroPower> heroPowers, int maxMiss)
        {
            HeroPowers = heroPowers;
            MaxMiss = maxMiss;
        }

        public IEnumerable<IHeroPower> HeroPowers { get; set; }
        public int MaxMiss { get; set; }
    }
}

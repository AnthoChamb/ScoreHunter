using ScoreHunter.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace ScoreHunter.Options
{
    public class OptimiserOptions
    {
        public IEnumerable<IHeroPower> HeroPowers { get; set; } = Enumerable.Empty<IHeroPower>();
        public int MaxMiss { get; set; } = -1;
    }
}

using ScoreHunter.Drawing.Abstractions.Interfaces;
using System.Collections.Generic;

namespace ScoreHunter.Drawing
{
    public class Tablature : ITablature
    {
        public Tablature(IEnumerable<IStaff> staves)
        {
            Staves = staves;
        }

        public IEnumerable<IStaff> Staves { get; }
    }
}

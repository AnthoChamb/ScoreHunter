using ScoreHunter.Drawing.Abstractions.Interfaces;
using System.Collections.Generic;

namespace ScoreHunter.Drawing
{
    public class Tablature : ITablature
    {
        public Tablature(int ticksPerQuarterNote, int ticksPerStaff, IEnumerable<IStaff> staves)
        {
            TicksPerQuarterNote = ticksPerQuarterNote;
            TicksPerStaff = ticksPerStaff;
            Staves = staves;
        }

        public int TicksPerQuarterNote { get; }
        public int TicksPerStaff { get; }
        public IEnumerable<IStaff> Staves { get; }
    }
}

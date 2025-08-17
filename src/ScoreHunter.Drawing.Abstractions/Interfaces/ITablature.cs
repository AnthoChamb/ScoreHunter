using System.Collections.Generic;

namespace ScoreHunter.Drawing.Abstractions.Interfaces
{
    public interface ITablature
    {
        int TicksPerQuarterNote { get; }
        int TicksPerStaff { get; }
        IEnumerable<IStaff> Staves { get; }
    }
}

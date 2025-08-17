using System.Collections.Generic;

namespace ScoreHunter.Drawing.Abstractions.Interfaces
{
    public interface ITablature
    {
        IEnumerable<IStaff> Staves { get; }
    }
}

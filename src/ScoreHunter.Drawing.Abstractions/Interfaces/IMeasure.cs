using ScoreHunter.Core.Interfaces;
using System.Collections.Generic;

namespace ScoreHunter.Drawing.Abstractions.Interfaces
{
    public interface IMeasure
    {
        int StartTicks { get; }
        int EndTicks { get; }
        ITimeSignature TimeSignature { get; }
        IEnumerable<ITempo> Tempos { get; }
        IEnumerable<IBeat> Beats { get; }
        IEnumerable<IDrawnNote> Notes { get; }
    }
}

using ScoreHunter.Core.Interfaces;
using System.Collections.Generic;

namespace ScoreHunter.Drawing.Abstractions.Interfaces
{
    public interface IMeasure
    {
        int Ticks { get; }
        double Start { get; }
        double End { get; }
        ITimeSignature TimeSignature { get; }
        IEnumerable<ITempo> Tempos { get; }
        IEnumerable<IBeat> Beats { get; }
        IEnumerable<INote> Notes { get; }
        IEnumerable<IPhrase> HeroPowerPhrases { get; }
        IEnumerable<IPhrase> HighwayPhrases { get; }
    }
}

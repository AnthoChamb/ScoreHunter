using System.Collections.Generic;

namespace ScoreHunter.Drawing.Abstractions.Interfaces
{
    public interface IStaff
    {
        int StartTicks { get; }
        int EndTicks { get; }
        IEnumerable<IMeasure> Measures { get; }
        IEnumerable<IDrawnSustain> Sustains { get; }
        IEnumerable<IDrawnPhrase> HeroPowerPhrases { get; }
        IEnumerable<IDrawnPhrase> HighwayPhrases { get; }
        IEnumerable<IDrawnActivation> Activations { get; }
    }
}

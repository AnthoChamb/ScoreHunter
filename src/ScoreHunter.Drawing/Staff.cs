using ScoreHunter.Drawing.Abstractions.Interfaces;
using System.Collections.Generic;

namespace ScoreHunter.Drawing
{
    public class Staff : IStaff
    {
        public Staff(int startTicks,
                     int endTicks,
                     IEnumerable<IMeasure> measures,
                     IEnumerable<IDrawnSustain> sustains,
                     IEnumerable<IDrawnPhrase> heroPowerPhrases,
                     IEnumerable<IDrawnPhrase> highwayPhrases,
                     IEnumerable<IDrawnActivation> activations)
        {
            StartTicks = startTicks;
            EndTicks = endTicks;
            Measures = measures;
            Sustains = sustains;
            HeroPowerPhrases = heroPowerPhrases;
            HighwayPhrases = highwayPhrases;
            Activations = activations;
        }

        public int StartTicks { get; }
        public int EndTicks { get; }
        public IEnumerable<IMeasure> Measures { get; }
        public IEnumerable<IDrawnSustain> Sustains { get; }
        public IEnumerable<IDrawnPhrase> HeroPowerPhrases { get; }
        public IEnumerable<IDrawnPhrase> HighwayPhrases { get; }
        public IEnumerable<IDrawnActivation> Activations { get; }
    }
}

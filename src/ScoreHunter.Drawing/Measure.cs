using ScoreHunter.Core.Interfaces;
using ScoreHunter.Drawing.Abstractions.Interfaces;
using System.Collections.Generic;

namespace ScoreHunter.Drawing
{
    public class Measure : IMeasure
    {
        public Measure(int ticks,
                       double start,
                       double end,
                       ITimeSignature timeSignature,
                       IEnumerable<ITempo> tempos,
                       IEnumerable<IBeat> beats,
                       IEnumerable<INote> notes,
                       IEnumerable<IPhrase> heroPowerPhrases,
                       IEnumerable<IPhrase> highwayPhrases)
        {
            Ticks = ticks;
            Start = start;
            End = end;
            TimeSignature = timeSignature;
            Tempos = tempos;
            Beats = beats;
            Notes = notes;
            HeroPowerPhrases = heroPowerPhrases;
            HighwayPhrases = highwayPhrases;
        }

        public int Ticks { get; }
        public double Start { get; }
        public double End { get; }
        public ITimeSignature TimeSignature { get; }
        public IEnumerable<ITempo> Tempos { get; }
        public IEnumerable<IBeat> Beats { get; }
        public IEnumerable<INote> Notes { get; }
        public IEnumerable<IPhrase> HeroPowerPhrases { get; }
        public IEnumerable<IPhrase> HighwayPhrases { get; }
    }
}

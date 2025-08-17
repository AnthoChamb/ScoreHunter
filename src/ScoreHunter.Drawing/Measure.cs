using ScoreHunter.Core.Interfaces;
using ScoreHunter.Drawing.Abstractions.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace ScoreHunter.Drawing
{
    public class Measure : IMeasure
    {
        public Measure(int startTicks,
                       int endTicks,
                       ITimeSignature timeSignature,
                       IEnumerable<ITempo> tempos,
                       IEnumerable<IDrawnNote> notes)
        {
            StartTicks = startTicks;
            EndTicks = endTicks;
            TimeSignature = timeSignature;
            Tempos = tempos;
            Notes = notes;
        }

        public int StartTicks { get; }
        public int EndTicks { get; }
        public ITimeSignature TimeSignature { get; }
        public IEnumerable<ITempo> Tempos { get; }
        public IEnumerable<IBeat> Beats { get; } = Enumerable.Empty<IBeat>(); // TODO: Generate beats
        public IEnumerable<IDrawnNote> Notes { get; }
    }
}

using ScoreHunter.Core.Interfaces;
using ScoreHunter.Drawing.Abstractions.Interfaces;
using System.Collections.Generic;

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
        public IEnumerable<IBeat> Beats
        {
            get
            {
                var ticks = StartTicks;
                var ticksPerBeat = (EndTicks - StartTicks) / TimeSignature.Denominator;
                for (var i = 1; i < TimeSignature.Denominator; i++)
                {
                    ticks += ticksPerBeat;
                    yield return new Beat(ticks);
                }
            }
        }
        public IEnumerable<IDrawnNote> Notes { get; }
    }
}

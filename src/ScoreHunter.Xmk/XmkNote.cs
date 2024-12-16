using FsgXmk.Abstractions.Enums;
using ScoreHunter.Core;
using ScoreHunter.Core.Interfaces;

namespace ScoreHunter.Xmk
{
    public class XmkNote : INote
    {
        private readonly EventType _xmkEventType;

        public XmkNote(double start, double end, Frets frets, EventType xmkEventType)
        {
            Start = start;
            End = end;
            Frets = frets;
            _xmkEventType = xmkEventType;
        }

        public double Start { get; }
        public double End { get; }
        public Frets Frets { get; }
        public bool IsSustain => _xmkEventType.HasFlag(EventType.Sustain);
        public bool IsHopo => _xmkEventType.HasFlag(EventType.Hopo);
    }
}

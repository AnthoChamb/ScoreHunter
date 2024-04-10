using ScoreHunter.Core.Enums;
using ScoreHunter.Core.Interfaces;

namespace ScoreHunter.Core.Events
{
    public class NoteEvent : Event
    {
        private readonly HeroPowerFlags _heroPowerFlags;

        public NoteEvent(double start, Frets frets) : this(start, frets, HeroPowerFlags.None)
        {
        }

        public NoteEvent(double start, Frets frets, HeroPowerFlags heroPowerFlags) : base(start)
        {
            Frets = frets;
            _heroPowerFlags = heroPowerFlags;
        }

        public Frets Frets { get; }
        public bool IsHeroPowerStart => _heroPowerFlags.HasFlag(HeroPowerFlags.HeroPowerStart);
        public bool IsHeroPowerEnd => _heroPowerFlags.HasFlag(HeroPowerFlags.HeroPowerEnd);

        public override void Accept(IEventVisitor visitor) => visitor.Visit(this);
    }
}

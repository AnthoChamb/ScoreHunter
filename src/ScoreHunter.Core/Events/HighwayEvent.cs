﻿using ScoreHunter.Core.Interfaces;

namespace ScoreHunter.Core.Events
{
    public class HighwayEvent : Event
    {
        public HighwayEvent(double start, double end) : base(start)
        {
            End = end;
        }

        public double End { get; }

        public override void Accept(IEventVisitor visitor) => visitor.Visit(this);
    }
}

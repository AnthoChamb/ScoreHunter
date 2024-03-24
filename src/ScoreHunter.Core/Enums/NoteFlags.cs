using System;

namespace ScoreHunter.Core.Enums
{
    [Flags]
    public enum NoteFlags : byte
    {
        None = 0b0000_0000,
        Sustain = 0b0000_0001,
        Hopo = 0b0000_0010
    }
}

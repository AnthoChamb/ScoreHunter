using System;

namespace ScoreHunter.Core.Enums
{
    [Flags]
    public enum HeroPowerFlags : byte
    {
        None = 0b0000_0000,
        HeroPowerStart = 0b_0000_0001,
        HeroPowerEnd = 0b0000_0010
    }
}

using System;

namespace ScoreHunter.Core.Enums
{
    [Flags]
    public enum FretFlags : byte
    {
        None = 0b0000_0000,
        Open = 0b0000_0001,
        Black1 = 0b0000_0010,
        Black2 = 0b0000_0100,
        Black3 = 0b0000_1000,
        White1 = 0b0001_0000,
        White2 = 0b0010_0000,
        White3 = 0b0100_0000
    }
}

using ScoreHunter.Core.Enums;
using System.Collections.Generic;

namespace ScoreHunter.Core.Interfaces
{
    public interface ITrack
    {
        int TicksPerQuarterNote { get; }
        IEnumerable<ITempo> Tempos { get; }
        IEnumerable<ITimeSignature> TimeSignatures { get; }
        IReadOnlyDictionary<Difficulty, IDifficultyTrack> Difficulties { get; }
        IEnumerable<IPhrase> HighwayPhrases { get; }
    }
}

using ScoreHunter.Core.Enums;
using System.Collections.Generic;

namespace ScoreHunter.Core.Interfaces
{
    public interface ITrack
    {
        IReadOnlyDictionary<Difficulty, IDifficultyTrack> Difficulties { get; }
        IEnumerable<IPhrase> HighwayPhrases { get; }
    }
}

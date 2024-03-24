using ScoreHunter.Core.Enums;
using ScoreHunter.Core.Interfaces;
using System.Collections.Generic;

namespace ScoreHunter.Core
{
    public class Track : ITrack
    {
        public Track(IReadOnlyDictionary<Difficulty, IDifficultyTrack> difficulties, IEnumerable<IPhrase> highwayPhrases)
        {
            Difficulties = difficulties;
            HighwayPhrases = highwayPhrases;
        }

        public IReadOnlyDictionary<Difficulty, IDifficultyTrack> Difficulties { get; }
        public IEnumerable<IPhrase> HighwayPhrases { get; }
    }
}

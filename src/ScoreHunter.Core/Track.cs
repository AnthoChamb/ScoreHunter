using ScoreHunter.Core.Enums;
using ScoreHunter.Core.Interfaces;
using System.Collections.Generic;

namespace ScoreHunter.Core
{
    public class Track : ITrack
    {
        public Track(int ticksPerQuarterNote, IEnumerable<ITempo> tempos, IEnumerable<ITimeSignature> timeSignatures, IReadOnlyDictionary<Difficulty, IDifficultyTrack> difficulties, IEnumerable<IPhrase> highwayPhrases)
        {
            TicksPerQuarterNote = ticksPerQuarterNote;
            Tempos = tempos;
            TimeSignatures = timeSignatures;
            Difficulties = difficulties;
            HighwayPhrases = highwayPhrases;
        }

        public int TicksPerQuarterNote { get; }
        public IEnumerable<ITempo> Tempos { get; }
        public IEnumerable<ITimeSignature> TimeSignatures { get; }
        public IReadOnlyDictionary<Difficulty, IDifficultyTrack> Difficulties { get; }
        public IEnumerable<IPhrase> HighwayPhrases { get; }
    }
}

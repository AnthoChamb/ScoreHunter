using System.Collections.Generic;

namespace ScoreHunter.Core.Interfaces
{
    public interface IDifficultyTrack
    {
        IEnumerable<INote> Notes { get; }
        IEnumerable<IPhrase> HeroPowerPhrases { get; }
        IChordNode GetFirstChordNode();
    }
}

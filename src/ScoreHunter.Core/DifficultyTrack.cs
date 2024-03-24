using ScoreHunter.Core.Interfaces;
using System.Collections.Generic;

namespace ScoreHunter.Core
{
    public class DifficultyTrack : IDifficultyTrack
    {
        private readonly IChordNode _first;

        public DifficultyTrack(IChordNode first, IEnumerable<IPhrase> heroPowerPhrases)
        {
            _first = first;
            HeroPowerPhrases = heroPowerPhrases;
        }

        public IEnumerable<INote> Notes
        {
            get
            {
                var current = _first;

                while (current != null)
                {
                    foreach (var note in current.Value.Notes)
                    {
                        yield return note;
                    }
                    current = current.Next;
                }
            }
        }

        public IEnumerable<IPhrase> HeroPowerPhrases { get; }

        public IChordNode GetFirstChordNode() => _first;
    }
}

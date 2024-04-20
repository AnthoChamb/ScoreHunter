using ScoreHunter.Core.Interfaces;
using System.Collections.Generic;

namespace ScoreHunter.Core.Builders
{
    public class DifficultyTrackBuilder
    {
        private NoteNode _first;
        private NoteNode _previous;
        private readonly ICollection<IPhrase> _heroPowerPhrases;

        public DifficultyTrackBuilder()
        {
            _heroPowerPhrases = new List<IPhrase>();
        }

        public DifficultyTrackBuilder AddNote(INote note)
        {
            var node = new NoteNode(note);

            if (_first == null)
            {
                _first = _previous = node;
            }
            else
            {
                node.Previous = _previous;
                _previous.Next = node;
                _previous = node;
            }

            return this;
        }

        public DifficultyTrackBuilder AddHeroPowerPhrase(IPhrase phrase)
        {
            _heroPowerPhrases.Add(phrase);
            return this;
        }

        public DifficultyTrack Build() => new DifficultyTrack(_first?.GetChordNode(), _heroPowerPhrases);
    }
}

using ScoreHunter.Core.Enums;
using ScoreHunter.Core.Interfaces;
using System.Collections.Generic;

namespace ScoreHunter.Core.Builders
{
    public class TrackBuilder
    {
        private readonly IDictionary<Difficulty, DifficultyTrackBuilder> _difficulties;
        private readonly ICollection<IPhrase> _highwayPhrases;

        public TrackBuilder()
        {
            _difficulties = new Dictionary<Difficulty, DifficultyTrackBuilder>();
            _highwayPhrases = new List<IPhrase>();
        }

        public TrackBuilder AddNote(Difficulty difficulty, INote note)
        {
            if (_difficulties.TryGetValue(difficulty, out var builder))
            {
                builder.AddNote(note);
            }
            else
            {
                var newBuilder = new DifficultyTrackBuilder();
                newBuilder.AddNote(note);
                _difficulties.Add(difficulty, newBuilder);
            }

            return this;
        }

        public TrackBuilder AddHeroPowerPhrase(Difficulty difficulty, IPhrase phrase)
        {
            if (_difficulties.TryGetValue(difficulty, out var builder))
            {
                builder.AddHeroPowerPhrase(phrase);
            }
            else
            {
                var newBuilder = new DifficultyTrackBuilder();
                newBuilder.AddHeroPowerPhrase(phrase);
                _difficulties.Add(difficulty, newBuilder);
            }

            return this;
        }

        public TrackBuilder AddHighwayPhrase(IPhrase phrase)
        {
            _highwayPhrases.Add(phrase);
            return this;
        }

        public Track Build()
        {
            var difficulties = new Dictionary<Difficulty, IDifficultyTrack>(_difficulties.Count);

            foreach (var difficulty in _difficulties)
            {
                difficulties.Add(difficulty.Key, difficulty.Value.Build());
            }

            return new Track(difficulties, _highwayPhrases);
        }
    }
}

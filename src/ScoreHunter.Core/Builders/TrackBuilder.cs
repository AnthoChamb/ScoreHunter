using ScoreHunter.Core.Enums;
using ScoreHunter.Core.Interfaces;
using System.Collections.Generic;

namespace ScoreHunter.Core.Builders
{
    public class TrackBuilder
    {
        private int _ticksPerQuarterNote;
        private readonly ICollection<ITempo> _tempos;
        private readonly ICollection<ITimeSignature> _timeSignatures;
        private readonly IDictionary<Difficulty, DifficultyTrackBuilder> _difficulties;
        private readonly ICollection<IPhrase> _highwayPhrases;

        public TrackBuilder()
        {
            _tempos = new List<ITempo>();
            _timeSignatures = new List<ITimeSignature>();
            _difficulties = new Dictionary<Difficulty, DifficultyTrackBuilder>();
            _highwayPhrases = new List<IPhrase>();
        }

        public TrackBuilder WithTicksPerQuarterNote(int ticksPerQuarterNote)
        {
            _ticksPerQuarterNote = ticksPerQuarterNote;
            return this;
        }

        public TrackBuilder AddTempo(ITempo tempo)
        {
            _tempos.Add(tempo);
            return this;
        }

        public TrackBuilder AddTimeSignature(ITimeSignature timeSignature)
        {
            _timeSignatures.Add(timeSignature);
            return this;
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

            return new Track(_ticksPerQuarterNote, _tempos, _timeSignatures, difficulties, _highwayPhrases);
        }
    }
}

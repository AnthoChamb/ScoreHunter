using ScoreHunter.Core.Events;
using ScoreHunter.Core.Interfaces;
using ScoreHunter.Options;
using System.Collections.Generic;
using System.Linq;

namespace ScoreHunter
{
    public class OptimiserEventVisitor : IEventVisitor
    {
        private IReadOnlyCollection<ICandidate> _candidates;
        private readonly OptimiserOptions _options;

        public OptimiserEventVisitor(ICandidate candidate, OptimiserOptions options)
        {
            _candidates = new[] { candidate };
            _options = options;
        }

        public IPath GetMaxScore()
        {
            var maxScore = _candidates.FirstOrDefault();
            foreach (var candidate in _candidates)
            {
                if (candidate.Score > maxScore.Score)
                {
                    maxScore = candidate;
                }
            }
            return maxScore;
        }

        public void Visit(NoteEvent note)
        {
            var cache = new Dictionary<ICacheKey, ICandidate>(_candidates.Count);
            foreach (var candidate in _candidates)
            {
                Optimize(candidate, note, cache);
            }
            _candidates = cache.Values;
        }

        public void Visit(SustainEvent sustain)
        {
            var cache = new Dictionary<ICacheKey, ICandidate>(_candidates.Count);
            foreach (var candidate in _candidates)
            {
                Optimize(candidate, sustain, cache);
            }
            _candidates = cache.Values;
        }

        public void Visit(HighwayEvent highway)
        {
            var cache = new Dictionary<ICacheKey, ICandidate>(_candidates.Count);
            foreach (var candidate in _candidates)
            {
                Optimize(candidate, highway, cache);
            }
            _candidates = cache.Values;
        }

        private void Optimize(ICandidate candidate, NoteEvent note, Dictionary<ICacheKey, ICandidate> cache)
        {
            candidate = candidate.Advance(note);

            if ((_options.MaxMiss == -1 || candidate.Streak < _options.MaxMiss) && candidate.TryMiss(note, out var missScore))
            {
                // Optimize note again as note has not been hit yet
                Optimize(missScore, note, cache);
            }

            if (candidate.TryActivate(note, out var activationScore))
            {
                activationScore = activationScore.HitNote(note);
                Cache(activationScore, cache);
            }

            if (note.IsHeroPowerStart)
            {
                foreach (var heroPower in _options.HeroPowers)
                {
                    if (candidate.TrySetHeroPower(heroPower, out var heroPowerScore))
                    {
                        heroPowerScore = heroPowerScore.HitNote(note);
                        Cache(heroPowerScore, cache);
                    }
                }
            }

            candidate = candidate.HitNote(note);
            Cache(candidate, cache);
        }

        private void Optimize(ICandidate candidate, SustainEvent sustain, Dictionary<ICacheKey, ICandidate> cache)
        {
            candidate = candidate.Advance(sustain);

            if (candidate.TryActivate(sustain, out var activationScore))
            {
                activationScore = activationScore.HoldSustain(sustain);
                Cache(activationScore, cache);
            }

            candidate = candidate.HoldSustain(sustain);
            Cache(candidate, cache);
        }

        private void Optimize(ICandidate candidate, HighwayEvent highway, Dictionary<ICacheKey, ICandidate> cache)
        {
            candidate = candidate.Advance(highway).Highway(highway);
            Cache(candidate, cache);
        }

        private void Cache(ICandidate candidate, Dictionary<ICacheKey, ICandidate> cache)
        {
            var cacheKey = candidate.GetCacheKey();
            if (cache.TryGetValue(cacheKey, out var value))
            {
                if (candidate.Score > value.Score)
                {
                    cache[cacheKey] = candidate;
                }
            }
            else
            {
                cache.Add(cacheKey, candidate);
            }
        }
    }
}

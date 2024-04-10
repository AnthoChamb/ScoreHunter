using ScoreHunter.Core.Events;
using ScoreHunter.Core.Interfaces;
using ScoreHunter.Options;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScoreHunter
{
    public class OptimiserEventVisitor : IEventVisitor
    {
        private IEnumerable<ICandidate> _candidates;
        private readonly OptimiserOptions _options;

        public OptimiserEventVisitor(ICandidate candidate, OptimiserOptions options)
        {
            _candidates = new[] { candidate };
            _options = options;
        }

        public ICandidate GetMaxScore()
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
            var cache = new ConcurrentDictionary<ICacheKey, ICandidate>();
            Parallel.ForEach(_candidates, (candidate) => Optimize(candidate, note, cache));
            _candidates = cache.Values;
        }

        public void Visit(SustainEvent sustain)
        {
            var cache = new ConcurrentDictionary<ICacheKey, ICandidate>();
            Parallel.ForEach(_candidates, (candidate) => Optimize(candidate, sustain, cache));
            _candidates = cache.Values;
        }

        public void Visit(HighwayEvent highway)
        {
            var cache = new ConcurrentDictionary<ICacheKey, ICandidate>();
            Parallel.ForEach(_candidates, (candidate) => Optimize(candidate, highway, cache));
            _candidates = cache.Values;
        }

        private void Optimize(ICandidate candidate, NoteEvent note, ConcurrentDictionary<ICacheKey, ICandidate> cache)
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

        private void Optimize(ICandidate candidate, SustainEvent sustain, ConcurrentDictionary<ICacheKey, ICandidate> cache)
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

        private void Optimize(ICandidate candidate, HighwayEvent highway, ConcurrentDictionary<ICacheKey, ICandidate> cache)
        {
            candidate = candidate.Advance(highway).Highway(highway);
            Cache(candidate, cache);
        }

        private void Cache(ICandidate candidate, ConcurrentDictionary<ICacheKey, ICandidate> cache)
        {
            var cacheKey = candidate.GetCacheKey();
            cache.AddOrUpdate(cacheKey, candidate, (key, value) =>
            {
                if (candidate.Score > value.Score)
                {
                    return candidate;
                }
                return value;
            });
        }
    }
}

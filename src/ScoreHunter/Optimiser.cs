using ScoreHunter.Core.Enums;
using ScoreHunter.Core.Interfaces;
using ScoreHunter.Factories;
using ScoreHunter.Options;

namespace ScoreHunter
{
    public class Optimiser : IOptimizer
    {
        private readonly OptimiserOptions _optimiserOptions;
        private readonly ScoringOptions _scoringOptions;
        private readonly TrackEventCollectionFactory _trackEventCollectionFactory;

        public Optimiser(OptimiserOptions optimiserOptions, ScoringOptions scoringOptions, TrackOptions trackOptions)
        {
            _optimiserOptions = optimiserOptions;
            _scoringOptions = scoringOptions;
            _trackEventCollectionFactory = new TrackEventCollectionFactory(trackOptions);
        }

        public IPath Optimize(ITrack track, Difficulty difficulty)
        {
            var eventCollection = _trackEventCollectionFactory.Create(track, difficulty);
            var scoring = new Candidate(_scoringOptions);
            var visitor = new OptimiserEventVisitor(scoring, _optimiserOptions);

            foreach (var @event in eventCollection.Events)
            {
                @event.Accept(visitor);
            }

            return visitor.GetMaxScore();
        }
    }
}

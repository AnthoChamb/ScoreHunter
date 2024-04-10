using ScoreHunter.Core.Enums;

namespace ScoreHunter.Core.Interfaces
{
    public interface IOptimizer
    {
        ICandidate Optimize(ITrack track, Difficulty difficulty);
    }
}

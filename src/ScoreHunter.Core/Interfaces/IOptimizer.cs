using ScoreHunter.Core.Enums;

namespace ScoreHunter.Core.Interfaces
{
    public interface IOptimizer
    {
        IPath Optimize(ITrack track, Difficulty difficulty);
    }
}

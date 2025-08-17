using ScoreHunter.Core.Interfaces;

namespace ScoreHunter.Drawing.Abstractions.Interfaces
{
    public interface IDrawnActivation : IActivation
    {
        int StartTicks { get; }
        int EndTicks { get; }
    }
}

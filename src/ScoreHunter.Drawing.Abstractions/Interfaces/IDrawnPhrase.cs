using ScoreHunter.Core.Interfaces;

namespace ScoreHunter.Drawing.Abstractions.Interfaces
{
    public interface IDrawnPhrase : IPhrase
    {
        int StartTicks { get; }
        int EndTicks { get; }
    }
}

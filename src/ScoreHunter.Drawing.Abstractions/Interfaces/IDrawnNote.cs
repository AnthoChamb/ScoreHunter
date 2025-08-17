using ScoreHunter.Core.Interfaces;

namespace ScoreHunter.Drawing.Abstractions.Interfaces
{
    public interface IDrawnNote : INote
    {
        int Ticks { get; }
    }
}

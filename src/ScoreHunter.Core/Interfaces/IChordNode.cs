namespace ScoreHunter.Core.Interfaces
{
    public interface IChordNode
    {
        IChord Value { get; }
        IChordNode Next { get; }
        IChordNode Previous { get; }
    }
}

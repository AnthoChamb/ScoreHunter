namespace ScoreHunter.Core.Interfaces
{
    public interface INoteNode
    {
        INote Value { get; }
        INoteNode Next { get; }
        INoteNode Previous { get; }
        IChordNode GetChordNode();
    }
}

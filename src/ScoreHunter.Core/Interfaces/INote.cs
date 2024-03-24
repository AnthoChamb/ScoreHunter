namespace ScoreHunter.Core.Interfaces
{
    public interface INote
    {
        double Start { get; }
        double End { get; }
        Frets Frets { get; }
        bool IsSustain { get; }
        bool IsHopo { get; }
    }
}

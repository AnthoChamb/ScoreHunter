namespace ScoreHunter.Core.Interfaces
{
    public interface ITimeSignature
    {
        int Ticks { get; }
        int Numerator { get; }
        int Denominator { get; }
    }
}

using System.Collections.Generic;

namespace ScoreHunter.Core.Interfaces
{
    public interface IChord
    {
        IEnumerable<INote> Notes { get; }
        double Start { get; }
        Frets Frets { get; }
    }
}

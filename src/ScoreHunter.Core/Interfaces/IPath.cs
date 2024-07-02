using System.Collections.Generic;

namespace ScoreHunter.Core.Interfaces
{
    public interface IPath
    {
        int Score { get; }
        int Miss { get; }
        IEnumerable<IActivation> Activations { get; }
    }
}

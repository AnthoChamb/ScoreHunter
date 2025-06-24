using System;
using System.Threading;
using System.Threading.Tasks;

namespace ScoreHunter.Core.Interfaces.IO
{
    public interface ITrackReader : IDisposable
    {
        ITrack Read();
        Task<ITrack> ReadAsync(CancellationToken cancellationToken = default);
    }
}

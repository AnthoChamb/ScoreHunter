using System;
using System.Threading;
using System.Threading.Tasks;

namespace ScoreHunter.Drawing.Abstractions.Interfaces.IO
{
    public interface ITablatureWriter : IDisposable
    {
        void Write(ITablature tablature);
        Task WriteAsync(ITablature tablature, CancellationToken cancellationToken = default);
    }
}

using ScoreHunter.Core.Interfaces.IO;
using System.IO;

namespace ScoreHunter.Core.Interfaces.Factories
{
    public interface ITrackStreamReaderFactory
    {
        ITrackReader Create(Stream stream, bool leaveOpen);
    }
}

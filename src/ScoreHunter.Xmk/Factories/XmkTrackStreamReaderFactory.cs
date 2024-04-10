using FsgXmk.Core.Interfaces.Factories;
using ScoreHunter.Core.Interfaces.Factories;
using ScoreHunter.Core.Interfaces.IO;
using ScoreHunter.Xmk.IO;
using System.IO;

namespace ScoreHunter.Xmk.Factories
{
    public class XmkTrackStreamReaderFactory : ITrackStreamReaderFactory
    {
        private readonly IXmkStreamReaderFactory _factory;

        public XmkTrackStreamReaderFactory(IXmkStreamReaderFactory factory)
        {
            _factory = factory;
        }

        public ITrackReader Create(Stream stream, bool leaveOpen)
        {
            return new XmkTrackReader(_factory.Create(stream, leaveOpen), leaveOpen);
        }
    }
}

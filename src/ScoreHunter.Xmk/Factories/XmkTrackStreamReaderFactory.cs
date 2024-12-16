using FsgXmk.Abstractions.Interfaces.Factories;
using ScoreHunter.Core.Interfaces.Factories;
using ScoreHunter.Core.Interfaces.IO;
using ScoreHunter.Xmk.IO;
using System.IO;

namespace ScoreHunter.Xmk.Factories
{
    public class XmkTrackStreamReaderFactory : ITrackStreamReaderFactory
    {
        private readonly IXmkHeaderStreamReaderFactory _headerStreamReaderFactory;
        private readonly IXmkEventStreamReaderFactory _eventStreamReaderFactory;

        public XmkTrackStreamReaderFactory(IXmkHeaderStreamReaderFactory headerStreamReaderFactory, IXmkEventStreamReaderFactory eventStreamReaderFactory)
        {
            _headerStreamReaderFactory = headerStreamReaderFactory;
            _eventStreamReaderFactory = eventStreamReaderFactory;
        }

        public ITrackReader Create(Stream stream, bool leaveOpen)
        {
            return new XmkTrackStreamReader(stream, _headerStreamReaderFactory, _eventStreamReaderFactory, leaveOpen);
        }
    }
}

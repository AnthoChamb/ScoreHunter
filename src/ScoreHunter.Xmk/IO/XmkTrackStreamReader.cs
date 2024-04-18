using FsgXmk.Core;
using FsgXmk.Core.Interfaces;
using FsgXmk.Core.Interfaces.Factories;
using ScoreHunter.Core.Interfaces;
using ScoreHunter.Core.Interfaces.IO;
using ScoreHunter.Xmk.Builders;
using System.IO;
using System.Threading.Tasks;

namespace ScoreHunter.Xmk.IO
{
    public class XmkTrackStreamReader : ITrackReader
    {
        private readonly Stream _stream;
        private readonly IXmkHeaderStreamReaderFactory _headerStreamReaderFactory;
        private readonly IXmkEventStreamReaderFactory _eventStreamReaderFactory;

        private readonly bool _leaveOpen;
        private bool _disposed;

        public XmkTrackStreamReader(
            Stream stream,
            IXmkHeaderStreamReaderFactory headerStreamReaderFactory,
            IXmkEventStreamReaderFactory eventStreamReaderFactory)
            : this(stream, headerStreamReaderFactory, eventStreamReaderFactory, false)
        {
        }

        public XmkTrackStreamReader(
            Stream stream,
            IXmkHeaderStreamReaderFactory headerStreamReaderFactory,
            IXmkEventStreamReaderFactory eventStreamReaderFactory,
            bool leaveOpen)
        {
            _stream = stream;
            _headerStreamReaderFactory = headerStreamReaderFactory;
            _eventStreamReaderFactory = eventStreamReaderFactory;
            _leaveOpen = leaveOpen;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                if (!_leaveOpen)
                {
                    _stream.Dispose();
                }
                _disposed = true;
            }
        }

        public ITrack Read()
        {
            IXmkHeader header;

            using (var headerReader = _headerStreamReaderFactory.Create(_stream, true))
            {
                header = headerReader.Read();
            }

            var offset = XmkConstants.XmkTempoSize * header.TempoCount + XmkConstants.XmkTimeSignatureSize * header.TimeSignatureCount;
            _stream.Seek(offset, SeekOrigin.Current);

            var builder = new XmkTrackBuilder();

            using (var eventReader = _eventStreamReaderFactory.Create(_stream, true))
            {
                for (var i = 0; i < header.EventCount; i++)
                {
                    var xmkEvent = eventReader.Read();
                    builder.AddXmkEvent(xmkEvent);
                }
            }

            return builder.Build();
        }

        public async Task<ITrack> ReadAsync()
        {
            IXmkHeader header;

            using (var headerReader = _headerStreamReaderFactory.Create(_stream, true))
            {
                header = await headerReader.ReadAsync();
            }

            var offset = XmkConstants.XmkTempoSize * header.TempoCount + XmkConstants.XmkTimeSignatureSize * header.TimeSignatureCount;
            _stream.Seek(offset, SeekOrigin.Current);

            var builder = new XmkTrackBuilder();

            using (var eventReader = _eventStreamReaderFactory.Create(_stream, true))
            {
                for (var i = 0; i < header.EventCount; i++)
                {
                    var xmkEvent = await eventReader.ReadAsync();
                    builder.AddXmkEvent(xmkEvent);
                }
            }

            return builder.Build();
        }
    }
}

using FsgXmk.Abstractions.Interfaces;
using FsgXmk.Abstractions.Interfaces.Factories;
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
        private readonly IXmkTempoStreamReaderFactory _tempoStreamReaderFactory;
        private readonly IXmkTimeSignatureStreamReaderFactory _timeSignatureStreamReaderFactory;
        private readonly IXmkEventStreamReaderFactory _eventStreamReaderFactory;

        private readonly bool _leaveOpen;
        private bool _disposed;

        public XmkTrackStreamReader(Stream stream,
                                    IXmkHeaderStreamReaderFactory headerStreamReaderFactory,
                                    IXmkTempoStreamReaderFactory tempoStreamReaderFactory,
                                    IXmkTimeSignatureStreamReaderFactory timeSignatureStreamReaderFactory,
                                    IXmkEventStreamReaderFactory eventStreamReaderFactory)
            : this(stream,
                   headerStreamReaderFactory,
                   tempoStreamReaderFactory,
                   timeSignatureStreamReaderFactory,
                   eventStreamReaderFactory,
                   false)
        {
        }

        public XmkTrackStreamReader(Stream stream,
                                    IXmkHeaderStreamReaderFactory headerStreamReaderFactory,
                                    IXmkTempoStreamReaderFactory tempoStreamReaderFactory,
                                    IXmkTimeSignatureStreamReaderFactory timeSignatureStreamReaderFactory,
                                    IXmkEventStreamReaderFactory eventStreamReaderFactory,
                                    bool leaveOpen)
        {
            _stream = stream;
            _headerStreamReaderFactory = headerStreamReaderFactory;
            _tempoStreamReaderFactory = tempoStreamReaderFactory;
            _timeSignatureStreamReaderFactory = timeSignatureStreamReaderFactory;
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

            var builder = new XmkTrackBuilder();

            using (var tempoReader = _tempoStreamReaderFactory.Create(_stream, true))
            {
                for (var i = 0; i < header.TempoCount; i++)
                {
                    var xmkTempo = tempoReader.Read();
                    builder.AddXmkTempo(xmkTempo);
                }
            }

            using (var timeSignatureReader = _timeSignatureStreamReaderFactory.Create(_stream, true))
            {
                for (var i = 0; i < header.TimeSignatureCount; i++)
                {
                    var xmkTimeSignature = timeSignatureReader.Read();
                    builder.AddXmkTimeSignature(xmkTimeSignature);
                }
            }

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

            var builder = new XmkTrackBuilder();

            using (var tempoReader = _tempoStreamReaderFactory.Create(_stream, true))
            {
                for (var i = 0; i < header.TempoCount; i++)
                {
                    var xmkTempo = await tempoReader.ReadAsync();
                    builder.AddXmkTempo(xmkTempo);
                }
            }

            using (var timeSignatureReader = _timeSignatureStreamReaderFactory.Create(_stream, true))
            {
                for (var i = 0; i < header.TimeSignatureCount; i++)
                {
                    var xmkTimeSignature = await timeSignatureReader.ReadAsync();
                    builder.AddXmkTimeSignature(xmkTimeSignature);
                }
            }

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

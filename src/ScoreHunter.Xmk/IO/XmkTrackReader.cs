using FsgXmk.Core.Interfaces;
using FsgXmk.Core.Interfaces.IO;
using ScoreHunter.Core.Interfaces;
using ScoreHunter.Core.Interfaces.IO;
using ScoreHunter.Xmk.Builders;
using System.Threading.Tasks;

namespace ScoreHunter.Xmk.IO
{
    public class XmkTrackReader : ITrackReader
    {
        private readonly IXmkReader _reader;
        private readonly bool _leaveOpen;
        private bool _disposed;

        public XmkTrackReader(IXmkReader reader) : this(reader, false)
        {
        }

        public XmkTrackReader(IXmkReader reader, bool leaveOpen)
        {
            _reader = reader;
            _leaveOpen = leaveOpen;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                if (!_leaveOpen)
                {
                    _reader.Dispose();
                }
                _disposed = true;
            }
        }

        public ITrack Read()
        {
            var xmk = _reader.Read();
            return BuildXmkTrack(xmk);
        }

        public async Task<ITrack> ReadAsync()
        {
            var xmk = await _reader.ReadAsync();
            return BuildXmkTrack(xmk);
        }

        private ITrack BuildXmkTrack(IXmk xmk)
        {
            var builder = new XmkTrackBuilder();

            foreach (var xmkEvent in xmk.Events)
            {
                builder.AddXmkEvent(xmkEvent);
            }

            return builder.Build();
        }
    }
}

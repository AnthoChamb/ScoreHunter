using FsgXmk.Factories;
using ScoreHunter.Core.Interfaces;
using ScoreHunter.Xmk.Factories;
using System.IO;
using System.Management.Automation;

namespace ScoreHunter.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "ScoreHunterTrack")]
    [OutputType(typeof(ITrack))]
    public class GetScoreHunterTrackCommand : PSCmdlet
    {
        [Alias("PSPath")]
        [Parameter(Position = 0, Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string[] LiteralPath { get; set; }

        protected override void ProcessRecord()
        {
            var headerStreamReaderFactory = new XmkHeaderStreamReaderFactory(new XmkHeaderByteArrayReaderFactory());
            var tempoStreamReaderFactory = new XmkTempoStreamReaderFactory(new XmkTempoByteArrayReaderFactory());
            var timeSignatureReaderFactory = new XmkTimeSignatureStreamReaderFactory(new XmkTimeSignatureByteArrayReaderFactory());
            var eventStreamReaderFactory = new XmkEventStreamReaderFactory(new XmkEventByteArrayReaderFactory());
            var trackStreamReaderFactory = new XmkTrackStreamReaderFactory(headerStreamReaderFactory,
                                                                           tempoStreamReaderFactory,
                                                                           timeSignatureReaderFactory,
                                                                           eventStreamReaderFactory);

            foreach (var path in LiteralPath)
            {
                using (var stream = File.OpenRead(SessionState.Path.GetUnresolvedProviderPathFromPSPath(path)))
                using (var reader = trackStreamReaderFactory.Create(stream, true))
                {
                    var track = reader.Read();
                    WriteObject(track);
                }
            }
        }
    }
}

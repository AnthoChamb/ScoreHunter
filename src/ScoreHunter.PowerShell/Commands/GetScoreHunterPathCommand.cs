using FsgXmk.Factories;
using ScoreHunter.Core.Enums;
using ScoreHunter.Core.Interfaces;
using ScoreHunter.Options;
using ScoreHunter.PowerShell.Enums;
using ScoreHunter.PowerShell.Factories;
using ScoreHunter.Xmk.Factories;
using System.IO;
using System.Linq;
using System.Management.Automation;

namespace ScoreHunter.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "ScoreHunterPath")]
    [OutputType(typeof(IPath))]
    public class GetScoreHunterPathCommand : PSCmdlet
    {
        [Alias("PSPath")]
        [Parameter(Position = 0, Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string[] LiteralPath { get; set; }

        [Parameter]
        public Difficulty Difficulty { get; set; } = Difficulty.Expert;

        [Parameter]
        public HeroPowerParameter[] HeroPower { get; set; } = new[] { HeroPowerParameter.ScoreChaser };

        [Parameter]
        public int MaxMiss { get; set; }

        [Parameter]
        public double PointsPerNote { get; set; } = 50 * 1.45;

        [Parameter]
        public double PointsPerSustain { get; set; } = 1.45;

        [Parameter]
        public int MaxMultiplier { get; set; } = 7;

        [Parameter]
        public int StreakPerMultiplier { get; set; } = 6;

        [Parameter]
        public double SustainLength { get; set; } = 0.023;

        [Parameter]
        public double SustainBurstLength { get; set; } = 0.2;

        [Parameter]
        public int MaxHeroPowerCount { get; set; } = -1;

        protected override void ProcessRecord()
        {
            var heroPowerFactory = new HeroPowerFactory();
            var heroPowers = HeroPower?.Select(heroPowerFactory.Create).ToArray() ?? Enumerable.Empty<IHeroPower>();
            var optimiserOptions = new OptimiserOptions(heroPowers, MaxMiss);
            var scoringOptions = new ScoringOptions(PointsPerNote, PointsPerSustain, MaxMultiplier, StreakPerMultiplier);
            var trackOptions = new TrackOptions(SustainLength, SustainBurstLength, MaxHeroPowerCount);
            var optimiser = new Optimiser(optimiserOptions, scoringOptions, trackOptions);

            var headerStreamReaderFactory = new XmkHeaderStreamReaderFactory(new XmkHeaderByteArrayReaderFactory());
            var tempoStreamReaderFactory = new XmkTempoStreamReaderFactory(new XmkTempoByteArrayReaderFactory());
            var timeSignatureReaderFactory = new XmkTimeSignatureStreamReaderFactory(new XmkTimeSignatureByteArrayReaderFactory());
            var eventStreamReaderFactory = new XmkEventStreamReaderFactory(new XmkEventByteArrayReaderFactory());
            var trackStreamReaderFactory = new XmkTrackStreamReaderFactory(headerStreamReaderFactory,
                                                                           tempoStreamReaderFactory,
                                                                           timeSignatureReaderFactory,
                                                                           eventStreamReaderFactory);

            ITrack track;

            foreach (var path in LiteralPath)
            {
                using (var stream = File.OpenRead(SessionState.Path.GetUnresolvedProviderPathFromPSPath(path)))
                using (var reader = trackStreamReaderFactory.Create(stream, true))
                {
                    track = reader.Read();
                }

                var optimalPath = optimiser.Optimize(track, Difficulty);
                WriteObject(optimalPath);
            }
        }
    }
}

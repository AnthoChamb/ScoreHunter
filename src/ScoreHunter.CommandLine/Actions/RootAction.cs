using FsgXmk.Factories;
using ScoreHunter.CommandLine.Binding;
using ScoreHunter.Core.Enums;
using ScoreHunter.Core.Interfaces;
using ScoreHunter.Drawing.Factories;
using ScoreHunter.Drawing.Svg;
using ScoreHunter.Drawing.Svg.IO;
using ScoreHunter.Xmk.Factories;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace ScoreHunter.CommandLine.Actions
{
    public class RootAction : AsynchronousCommandLineAction
    {
        private readonly Argument<FileInfo> _fileArgument;
        private readonly Option<FileInfo> _outputOption;
        private readonly Option<Difficulty> _difficultyOption;
        private readonly OptimiserOptionsBinder _optimiserOptionsBinder;
        private readonly ScoringOptionsBinder _scoringOptionsBinder;
        private readonly TrackOptionsBinder _trackOptionsBinder;
        private readonly TablatureOptionsBinder _tablatureOptionsBinder;

        public RootAction(Argument<FileInfo> fileArgument,
                          Option<FileInfo> outputOption,
                          Option<Difficulty> difficultyOption,
                          OptimiserOptionsBinder optimiserOptionsBinder,
                          ScoringOptionsBinder scoringOptionsBinder,
                          TrackOptionsBinder trackOptionsBinder,
                          TablatureOptionsBinder tablatureOptionsBinder)
        {
            _fileArgument = fileArgument;
            _outputOption = outputOption;
            _difficultyOption = difficultyOption;
            _optimiserOptionsBinder = optimiserOptionsBinder;
            _scoringOptionsBinder = scoringOptionsBinder;
            _trackOptionsBinder = trackOptionsBinder;
            _tablatureOptionsBinder = tablatureOptionsBinder;
        }

        public override async Task<int> InvokeAsync(ParseResult parseResult, CancellationToken cancellationToken = default)
        {
            var file = parseResult.GetRequiredValue(_fileArgument);
            var output = parseResult.GetValue(_outputOption) ?? new FileInfo(Path.ChangeExtension(file.FullName, SvgConstants.SvgExtension));
            var difficulty = parseResult.GetRequiredValue(_difficultyOption);
            var optimiserOptions = _optimiserOptionsBinder.GetBoundValue(parseResult);
            var scoringOptions = _scoringOptionsBinder.GetBoundValue(parseResult);
            var trackOptions = _trackOptionsBinder.GetBoundValue(parseResult);
            var tablatureOptions = _tablatureOptionsBinder.GetBoundValue(parseResult);

            var optimiser = new Optimiser(optimiserOptions, scoringOptions, trackOptions);
            var headerStreamReaderFactory = new XmkHeaderStreamReaderFactory(new XmkHeaderByteArrayReaderFactory());
            var tempoStreamReaderFactory = new XmkTempoStreamReaderFactory(new XmkTempoByteArrayReaderFactory());
            var timeSignatureReaderFactory = new XmkTimeSignatureStreamReaderFactory(new XmkTimeSignatureByteArrayReaderFactory());
            var eventStreamReaderFactory = new XmkEventStreamReaderFactory(new XmkEventByteArrayReaderFactory());
            var trackStreamReaderFactory = new XmkTrackStreamReaderFactory(headerStreamReaderFactory,
                                                                           tempoStreamReaderFactory,
                                                                           timeSignatureReaderFactory,
                                                                           eventStreamReaderFactory);
            var tablatureFactory = new TablatureFactory(tablatureOptions);

            ITrack track;

            using (var stream = file.OpenRead())
            using (var reader = trackStreamReaderFactory.Create(stream, true))
            {
                track = await reader.ReadAsync(cancellationToken);
            }

            var optimalPath = optimiser.Optimize(track, difficulty);

            parseResult.Configuration.Output.WriteLine("Estimated score: " + optimalPath.Score);
            parseResult.Configuration.Output.WriteLine("Miss: " + optimalPath.Miss);

            foreach (var activation in optimalPath.Activations)
            {
                parseResult.Configuration.Output.WriteLine("Hero Power: " + activation.HeroPower + ", Streak: " + activation.Streak + ", IsChained: " + activation.IsChained);
            }

            var tablature = tablatureFactory.Create(track, difficulty, optimalPath);

            using (var stream = output.Create())
            using (var xmlWriter = XmlWriter.Create(stream))
            using (var writer = new SvgTablatureWriter(xmlWriter, true))
            {
                writer.Write(tablature);
            }

            return 0;
        }
    }
}

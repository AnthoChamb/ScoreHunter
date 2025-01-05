using BenchmarkDotNet.Attributes;
using FsgXmk.Factories;
using ScoreHunter.Core.Enums;
using ScoreHunter.Core.Interfaces;
using ScoreHunter.HeroPowers;
using ScoreHunter.Options;
using ScoreHunter.Xmk.Factories;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ScoreHunter.Benchmarks
{
    public class OptimiserBenchmarks
    {
        private readonly ITrack _track;
        private readonly ScoringOptions _scoringOptions = new ScoringOptions(50 * 1.45, 1.45, 7, 6);
        private readonly TrackOptions _trackOptions = new TrackOptions();

        private Optimiser? _optimiser;

        public OptimiserBenchmarks()
        {
            static string CallerFilePath([CallerFilePath] string callerFilePath = "") => callerFilePath;

            var path = Path.Combine(Path.GetDirectoryName(CallerFilePath())!, "guitar_3x2.xmk");
            var headerStreamReaderFactory = new XmkHeaderStreamReaderFactory(new XmkHeaderByteArrayReaderFactory());
            var tempoStreamReaderFactory = new XmkTempoStreamReaderFactory(new XmkTempoByteArrayReaderFactory());
            var timeSignatureReaderFactory = new XmkTimeSignatureStreamReaderFactory(new XmkTimeSignatureByteArrayReaderFactory());
            var eventStreamReaderFactory = new XmkEventStreamReaderFactory(new XmkEventByteArrayReaderFactory());
            var trackStreamReaderFactory = new XmkTrackStreamReaderFactory(headerStreamReaderFactory,
                                                                           tempoStreamReaderFactory,
                                                                           timeSignatureReaderFactory,
                                                                           eventStreamReaderFactory);

            using var stream = File.OpenRead(path);
            using var reader = trackStreamReaderFactory.Create(stream, true);
            _track = reader.Read();
        }

        [ParamsSource(nameof(ValuesForHeroPowers))]
        public IEnumerable<IHeroPower> HeroPowers { get; set; } = Enumerable.Empty<IHeroPower>();

        [Params(0, -1)]
        public int MaxMiss { get; set; }

        public IEnumerable<IEnumerable<IHeroPower>> ValuesForHeroPowers
        {
            get
            {
                var scoreChaser = new ScoreChaserHeroPower();
                yield return new IHeroPower[] { scoreChaser };
                yield return new IHeroPower[] { scoreChaser, new DoubleMultiplierHeroPower() };
            }
        }

        [GlobalSetup]
        public void GlobalSetup()
        {
            var optimiserOptions = new OptimiserOptions(HeroPowers, MaxMiss);
            _optimiser = new Optimiser(optimiserOptions, _scoringOptions, _trackOptions);
        }

        [Benchmark]
        public IPath Optimize()
        {
            return _optimiser!.Optimize(_track, Difficulty.Expert);
        }
    }
}

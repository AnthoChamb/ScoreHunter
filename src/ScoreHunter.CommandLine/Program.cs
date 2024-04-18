using FsgXmk.Kaitai.Factories;
using ScoreHunter.Core.Enums;
using ScoreHunter.Core.Interfaces;
using ScoreHunter.HeroPowers;
using ScoreHunter.Options;
using ScoreHunter.Xmk.Factories;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace ScoreHunter.CommandLine
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var heroPowers = new[] { new ScoreChaserHeroPower() };
            var optimiserOptions = new OptimiserOptions
            {
                HeroPowers = heroPowers,
                MaxMiss = 0
            };

            var scoringOptions = new ScoringOptions
            {
                PointsPerNote = 50 * 1.45,
                PointsPerSustain = 1.45,
                MaxMultiplier = 7,
                StreakPerMultiplier = 6
            };
            var trackOptions = new TrackOptions();
            var difficulty = Difficulty.Expert;

            var optimiser = new Optimiser(optimiserOptions, scoringOptions, trackOptions);
            var headerStreamReaderFactory = new KaitaiXmkHeaderStreamReaderFactory();
            var eventStreamReaderFactory = new KaitaiXmkEventStreamReaderFactory();
            var trackStreamReaderFactory = new XmkTrackStreamReaderFactory(headerStreamReaderFactory, eventStreamReaderFactory);

            if (args.Length > 0)
            {
                var path = args[0];
                ITrack track;

                using (var stream = File.OpenRead(path))
                using (var reader = trackStreamReaderFactory.Create(stream, true))  
                {
                    track = await reader.ReadAsync();
                }

                var optimalPath = optimiser.Optimize(track, difficulty);

                Console.WriteLine("Estimated score: " + optimalPath.Score);
                Console.WriteLine("Miss: " + optimalPath.Miss);

                foreach (var activation in optimalPath.Activations)
                {
                    Console.WriteLine("Streak: " + activation.Streak + ", IsChained: " + activation.IsChained);
                }
            }

            stopWatch.Stop();
            Console.WriteLine("Elapsed time: " + stopWatch.Elapsed);
        }
    }
}

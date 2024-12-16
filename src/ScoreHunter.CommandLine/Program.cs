using FsgXmk.Factories;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ScoreHunter.CommandLine.Binding;
using ScoreHunter.CommandLine.Enums;
using ScoreHunter.CommandLine.Factories;
using ScoreHunter.Core.Enums;
using ScoreHunter.Core.Interfaces;
using ScoreHunter.Options;
using ScoreHunter.Xmk.Factories;
using System;
using System.Collections.Generic;
using System.CommandLine;
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

            var rootCommand = new RootCommand();

            var fileArgument = new Argument<FileInfo>(
                name: "file");

            var difficultyOption = new Option<Difficulty>(
                aliases: ["-d", "--difficulty"],
                getDefaultValue: () => Difficulty.Expert);

            var heroPowersOption = new Option<IEnumerable<HeroPowerOption>>(
                aliases: ["--hp", "--hero-power", "--hero-powers"],
                getDefaultValue: () => [HeroPowerOption.ScoreChaser]);

            var maxMissOption = new Option<int>(
                name: "--max-miss");

            var pointsPerNoteOption = new Option<double>(
                aliases: ["--ppn", "--points-per-note"],
                getDefaultValue: () => 50 * 1.45);

            var pointsPerSustainOption = new Option<double>(
                aliases: ["--pps", "--points-per-sustain"],
                getDefaultValue: () => 1.45);

            var maxMultiplierOption = new Option<int>(
                aliases: ["--mm", "--max-multiplier"],
                getDefaultValue: () => 7);

            var streakPerMultiplierOption = new Option<int>(
                aliases: ["--spm", "--streak-per-multiplier"],
                getDefaultValue: () => 6);

            var sustainLengthOption = new Option<double>(
                aliases: ["--sl", "--sustain-length"],
                getDefaultValue: () => 0.023);

            var maxHeroPowerCountOption = new Option<int>(
                aliases: ["--mhpc", "--max-hero-power-count"],
                getDefaultValue: () => -1);

            rootCommand.AddArgument(fileArgument);
            rootCommand.AddOption(difficultyOption);
            rootCommand.AddOption(heroPowersOption);
            rootCommand.AddOption(maxMissOption);
            rootCommand.AddOption(pointsPerNoteOption);
            rootCommand.AddOption(pointsPerSustainOption);
            rootCommand.AddOption(maxMultiplierOption);
            rootCommand.AddOption(streakPerMultiplierOption);
            rootCommand.AddOption(sustainLengthOption);
            rootCommand.AddOption(maxHeroPowerCountOption);

            var localizationOptions = new LocalizationOptions { ResourcesPath = "Resources" };
            var stringLocalizerFactory = new ResourceManagerStringLocalizerFactory(new OptionsWrapper<LocalizationOptions>(localizationOptions), new LoggerFactory());
            var heroPowerFactory = new HeroPowerFactory(stringLocalizerFactory);

            var optimiserOptionsBinder = new OptimiserOptionsBinder(heroPowersOption, maxMissOption, heroPowerFactory);
            var scoringOptionsBinder = new ScoringOptionsBinder(pointsPerNoteOption, pointsPerSustainOption, maxMultiplierOption, streakPerMultiplierOption);
            var trackOptionsBinder = new TrackOptionsBinder(sustainLengthOption, maxHeroPowerCountOption);

            rootCommand.SetHandler(ExecuteAsync, fileArgument, difficultyOption, optimiserOptionsBinder, scoringOptionsBinder, trackOptionsBinder);

            await rootCommand.InvokeAsync(args);

            stopWatch.Stop();
            Console.WriteLine("Elapsed time: " + stopWatch.Elapsed);
        }

        public static async void ExecuteAsync(FileInfo file, Difficulty difficulty, OptimiserOptions optimiserOptions, ScoringOptions scoringOptions, TrackOptions trackOptions)
        {
            var optimiser = new Optimiser(optimiserOptions, scoringOptions, trackOptions);
            var headerStreamReaderFactory = new XmkHeaderStreamReaderFactory(new XmkHeaderByteArrayReaderFactory());
            var eventStreamReaderFactory = new XmkEventStreamReaderFactory(new XmkEventByteArrayReaderFactory());
            var trackStreamReaderFactory = new XmkTrackStreamReaderFactory(headerStreamReaderFactory, eventStreamReaderFactory);

            ITrack track;

            using (var stream = file.OpenRead())
            using (var reader = trackStreamReaderFactory.Create(stream, true))
            {
                track = await reader.ReadAsync();
            }

            var optimalPath = optimiser.Optimize(track, difficulty);

            Console.WriteLine("Estimated score: " + optimalPath.Score);
            Console.WriteLine("Miss: " + optimalPath.Miss);

            foreach (var activation in optimalPath.Activations)
            {
                Console.WriteLine("Hero Power: " + activation.HeroPower + ", Streak: " + activation.Streak + ", IsChained: " + activation.IsChained);
            }
        }
    }
}

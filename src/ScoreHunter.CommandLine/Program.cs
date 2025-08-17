using FsgXmk.Abstractions;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ScoreHunter.CommandLine.Actions;
using ScoreHunter.CommandLine.Binding;
using ScoreHunter.CommandLine.Enums;
using ScoreHunter.CommandLine.Factories;
using ScoreHunter.Core.Enums;
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

            var fileArgument = new Argument<FileInfo>("file");

            var outputOption = new Option<FileInfo>("--output", "-o");

            var difficultyOption = new Option<Difficulty>("--difficulty", "-d")
            {
                DefaultValueFactory = _ => Difficulty.Expert
            };

            var heroPowersOption = new Option<IEnumerable<HeroPowerOption>>("--hero-powers", "--hp", "--hero-power")
            {
                DefaultValueFactory = _ => new[] { HeroPowerOption.ScoreChaser }
            };

            var maxMissOption = new Option<int>("--max-miss")
            {
                DefaultValueFactory = _ => 0
            };

            var pointsPerNoteOption = new Option<double>("--points-per-note", "--ppn")
            {
                DefaultValueFactory = _ => 50 * 1.45
            };

            var pointsPerSustainOption = new Option<double>("--points-per-sustain", "--pps")
            {
                DefaultValueFactory = _ => 1.45
            };

            var maxMultiplierOption = new Option<int>("--max-multiplier", "--mm")
            {
                DefaultValueFactory = _ => 7
            };

            var streakPerMultiplierOption = new Option<int>("--streak-per-multiplier", "--spm")
            {
                DefaultValueFactory = _ => 6
            };

            var sustainLengthOption = new Option<double>("--sustain-length", "--sl")
            {
                DefaultValueFactory = _ => 0.023
            };

            var sustainBurstLengthOption = new Option<double>("--sustain-burst-length", "--sbl")
            {
                DefaultValueFactory = _ => 0.2
            };

            var maxHeroPowerCountOption = new Option<int>("--max-hero-power-count", "--mhpc")
            {
                DefaultValueFactory = _ => -1
            };

            var ticksPerStaffOption = new Option<int>("--ticks-per-staff", "--tps")
            {
                DefaultValueFactory = _ => XmkConstants.TicksPerQuarterNote * 4 * 4
            };

            rootCommand.Arguments.Add(fileArgument);
            rootCommand.Options.Add(outputOption);
            rootCommand.Options.Add(difficultyOption);
            rootCommand.Options.Add(heroPowersOption);
            rootCommand.Options.Add(maxMissOption);
            rootCommand.Options.Add(pointsPerNoteOption);
            rootCommand.Options.Add(pointsPerSustainOption);
            rootCommand.Options.Add(maxMultiplierOption);
            rootCommand.Options.Add(streakPerMultiplierOption);
            rootCommand.Options.Add(sustainLengthOption);
            rootCommand.Options.Add(sustainBurstLengthOption);
            rootCommand.Options.Add(maxHeroPowerCountOption);
            rootCommand.Options.Add(ticksPerStaffOption);

            var localizationOptions = new LocalizationOptions { ResourcesPath = "Resources" };
            using (var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole()))
            {
                var stringLocalizerFactory = new ResourceManagerStringLocalizerFactory(new OptionsWrapper<LocalizationOptions>(localizationOptions), loggerFactory);
                var heroPowerFactory = new HeroPowerFactory(stringLocalizerFactory);

                var optimiserOptionsBinder = new OptimiserOptionsBinder(heroPowersOption, maxMissOption, heroPowerFactory);
                var scoringOptionsBinder = new ScoringOptionsBinder(pointsPerNoteOption, pointsPerSustainOption, maxMultiplierOption, streakPerMultiplierOption);
                var trackOptionsBinder = new TrackOptionsBinder(sustainLengthOption, sustainBurstLengthOption, maxHeroPowerCountOption);
                var tablatureOptionsBinder = new TablatureOptionsBinder(ticksPerStaffOption);

                var rootAction = new RootAction(fileArgument, outputOption, difficultyOption, optimiserOptionsBinder, scoringOptionsBinder, trackOptionsBinder, tablatureOptionsBinder);
                rootCommand.Action = rootAction;

                await rootCommand.Parse(args).InvokeAsync();
            }

            stopWatch.Stop();
            Console.WriteLine("Elapsed time: " + stopWatch.Elapsed);
        }
    }
}

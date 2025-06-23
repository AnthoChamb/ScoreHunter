using ScoreHunter.Options;
using System.CommandLine;

namespace ScoreHunter.CommandLine.Binding
{
    public class TrackOptionsBinder
    {
        private readonly Option<double> _sustainLengthOption;
        private readonly Option<double> _sustainBurstLengthOption;
        private readonly Option<int> _maxHeroPowerCountOption;

        public TrackOptionsBinder(Option<double> sustainLengthOption, Option<double> sustainBurstLengthOption, Option<int> maxHeroPowerCountOption)
        {
            _sustainLengthOption = sustainLengthOption;
            _sustainBurstLengthOption = sustainBurstLengthOption;
            _maxHeroPowerCountOption = maxHeroPowerCountOption;
        }

        public TrackOptions GetBoundValue(ParseResult parseResult)
        {
            var sustainLength = parseResult.GetRequiredValue(_sustainLengthOption);
            var sustainBurstLength = parseResult.GetRequiredValue(_sustainBurstLengthOption);
            var maxHeroPowerCount = parseResult.GetRequiredValue(_maxHeroPowerCountOption);

            return new TrackOptions(sustainLength, sustainBurstLength, maxHeroPowerCount);
        }
    }
}

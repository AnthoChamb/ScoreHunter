using ScoreHunter.Options;
using System.CommandLine;
using System.CommandLine.Binding;

namespace ScoreHunter.CommandLine.Binding
{
    public class TrackOptionsBinder : BinderBase<TrackOptions>
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

        protected override TrackOptions GetBoundValue(BindingContext bindingContext)
        {
            var sustainLength = bindingContext.ParseResult.GetValueForOption(_sustainLengthOption);
            var sustainBurstLength = bindingContext.ParseResult.GetValueForOption(_sustainBurstLengthOption);
            var maxHeroPowerCount = bindingContext.ParseResult.GetValueForOption(_maxHeroPowerCountOption);

            return new TrackOptions(sustainLength, sustainBurstLength, maxHeroPowerCount);
        }
    }
}

using ScoreHunter.Options;
using System.CommandLine;
using System.CommandLine.Binding;

namespace ScoreHunter.CommandLine.Binding
{
    public class TrackOptionsBinder : BinderBase<TrackOptions>
    {
        private readonly Option<double> _sustainLengthOption;
        private readonly Option<int> _maxHeroPowerCountOption;

        public TrackOptionsBinder(Option<double> sustainLengthOption, Option<int> maxHeroPowerCountOption)
        {
            _sustainLengthOption = sustainLengthOption;
            _maxHeroPowerCountOption = maxHeroPowerCountOption;
        }

        protected override TrackOptions GetBoundValue(BindingContext bindingContext)
        {
            var sustainLength = bindingContext.ParseResult.GetValueForOption(_sustainLengthOption);
            var maxHeroPowerCount = bindingContext.ParseResult.GetValueForOption(_maxHeroPowerCountOption);

            return new TrackOptions(sustainLength, maxHeroPowerCount);
        }
    }
}

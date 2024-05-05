using ScoreHunter.CommandLine.Enums;
using ScoreHunter.CommandLine.Factories;
using ScoreHunter.Core.Interfaces;
using ScoreHunter.Options;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Binding;
using System.Linq;

namespace ScoreHunter.CommandLine.Binding
{
    public class OptimiserOptionsBinder : BinderBase<OptimiserOptions>
    {
        private readonly Option<IEnumerable<HeroPowerOption>> _heroPowersOption;
        private readonly Option<int> _maxMissOption;
        private readonly HeroPowerFactory _heroPowerFactory;

        public OptimiserOptionsBinder(Option<IEnumerable<HeroPowerOption>> heroPowersOption, Option<int> maxMissOption, HeroPowerFactory heroPowerFactory)
        {
            _heroPowersOption = heroPowersOption;
            _maxMissOption = maxMissOption;
            _heroPowerFactory = heroPowerFactory;
        }

        protected override OptimiserOptions GetBoundValue(BindingContext bindingContext)
        {
            var heroPowersOptions = bindingContext.ParseResult.GetValueForOption(_heroPowersOption);
            var heroPowers = heroPowersOptions?.Select(_heroPowerFactory.Create).ToArray() ?? Enumerable.Empty<IHeroPower>();

            var maxMiss = bindingContext.ParseResult.GetValueForOption(_maxMissOption);

            return new OptimiserOptions(heroPowers, maxMiss);
        }
    }
}

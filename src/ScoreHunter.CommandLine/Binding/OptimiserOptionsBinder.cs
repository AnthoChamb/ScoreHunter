using ScoreHunter.CommandLine.Enums;
using ScoreHunter.CommandLine.Factories;
using ScoreHunter.Options;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;

namespace ScoreHunter.CommandLine.Binding
{
    public class OptimiserOptionsBinder
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

        public OptimiserOptions GetBoundValue(ParseResult parseResult)
        {
            var heroPowersOptions = parseResult.GetRequiredValue(_heroPowersOption);
            var heroPowers = heroPowersOptions.Select(_heroPowerFactory.Create).ToArray();

            var maxMiss = parseResult.GetRequiredValue(_maxMissOption);

            return new OptimiserOptions(heroPowers, maxMiss);
        }
    }
}

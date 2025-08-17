using ScoreHunter.Drawing.Options;
using System.CommandLine;

namespace ScoreHunter.CommandLine.Binding
{
    public class TablatureOptionsBinder
    {
        private readonly Option<int> _ticksPerStaffOption;

        public TablatureOptionsBinder(Option<int> ticksPerStaffOption)
        {
            _ticksPerStaffOption = ticksPerStaffOption;
        }

        public TablatureOptions GetBoundValue(ParseResult parseResult)
        {
            var ticksPerStaff = parseResult.GetRequiredValue(_ticksPerStaffOption);

            return new TablatureOptions(ticksPerStaff);
        }
    }
}

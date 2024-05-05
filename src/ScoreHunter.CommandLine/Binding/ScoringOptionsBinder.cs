using ScoreHunter.Options;
using System.CommandLine;
using System.CommandLine.Binding;

namespace ScoreHunter.CommandLine.Binding
{
    public class ScoringOptionsBinder : BinderBase<ScoringOptions>
    {
        private readonly Option<double> _pointsPerNoteOption;
        private readonly Option<double> _pointsPerSustainOption;
        private readonly Option<int> _maxMultiplierOption;
        private readonly Option<int> _streakPerMultiplierOption;

        public ScoringOptionsBinder(Option<double> pointsPerNoteOption, Option<double> pointsPerSustainOption, Option<int> maxMultiplierOption, Option<int> streakPerMultiplierOption)
        {
            _pointsPerNoteOption = pointsPerNoteOption;
            _pointsPerSustainOption = pointsPerSustainOption;
            _maxMultiplierOption = maxMultiplierOption;
            _streakPerMultiplierOption = streakPerMultiplierOption;
        }

        protected override ScoringOptions GetBoundValue(BindingContext bindingContext)
        {
            var pointsPerNote = bindingContext.ParseResult.GetValueForOption(_pointsPerNoteOption);
            var pointsPerSustain = bindingContext.ParseResult.GetValueForOption(_pointsPerSustainOption);
            var maxMultiplier = bindingContext.ParseResult.GetValueForOption(_maxMultiplierOption);
            var streakPerMultiplier = bindingContext.ParseResult.GetValueForOption(_streakPerMultiplierOption);

            return new ScoringOptions(pointsPerNote, pointsPerSustain, maxMultiplier, streakPerMultiplier);
        }
    }
}

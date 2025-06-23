using ScoreHunter.Options;
using System.CommandLine;

namespace ScoreHunter.CommandLine.Binding
{
    public class ScoringOptionsBinder
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

        public ScoringOptions GetBoundValue(ParseResult parseResult)
        {
            var pointsPerNote = parseResult.GetRequiredValue(_pointsPerNoteOption);
            var pointsPerSustain = parseResult.GetRequiredValue(_pointsPerSustainOption);
            var maxMultiplier = parseResult.GetRequiredValue(_maxMultiplierOption);
            var streakPerMultiplier = parseResult.GetRequiredValue(_streakPerMultiplierOption);

            return new ScoringOptions(pointsPerNote, pointsPerSustain, maxMultiplier, streakPerMultiplier);
        }
    }
}

using ScoreHunter.Core;
using ScoreHunter.Core.Builders;
using ScoreHunter.Core.Enums;
using ScoreHunter.Options;
using Xunit;

namespace ScoreHunter.Tests
{
    public class OptimiserTest
    {
        [Fact]
        public void Optimize_CountingStarsExtendedSustain_ReturnsPathWithScoreEqual249()
        {
            // Arrange
            var track = new TrackBuilder()
                .AddNote(Difficulty.Expert, new Note(0, 1.870121002197267, Frets.Black1, NoteFlags.Sustain))
                .AddNote(Difficulty.Expert, new Note(0.284414291381837, 1.870121002197267, Frets.Black3, NoteFlags.Sustain))
                .Build();

            var optimiser = new Optimiser(new OptimiserOptions(), new ScoringOptions(), new TrackOptions());

            // Act
            var path = optimiser.Optimize(track, Difficulty.Expert);

            // Assert
            Assert.Equal(249, path.Score);
        }
    }
}

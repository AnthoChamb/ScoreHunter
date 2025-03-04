using ScoreHunter.Core;
using ScoreHunter.Core.Events;
using ScoreHunter.HeroPowers;
using ScoreHunter.Testing;
using Xunit;

namespace ScoreHunter.Tests.HeroPowers
{
    public class DoubleMultiplierHeroPowerTest
    {
        [Fact]
        public void Duration_Returns7()
        {
            // Arrange
            var doubleMultiplier = new DoubleMultiplierHeroPower();

            // Act
            var duration = doubleMultiplier.Duration;

            // Assert
            Assert.Equal(7, duration);
        }

        [Fact]
        public void Multiplier_ReturnsDoubleMultiplier()
        {
            // Arrange
            var doubleMultiplier = new DoubleMultiplierHeroPower();
            var multiplier = 7;
            var expected = 14;

            // Act
            var actual = doubleMultiplier.Multiplier(multiplier);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void MaxMultiplier_ReturnsMaxMultiplier()
        {
            // Arrange
            var doubleMultiplier = new DoubleMultiplierHeroPower();
            var maxMultiplier = 7;

            // Act
            var actual = doubleMultiplier.MaxMultiplier(maxMultiplier);

            // Assert
            Assert.Equal(maxMultiplier, actual);
        }

        [Fact]
        public void CanActivate_Note_ReturnsTrue()
        {
            // Arrange
            var doubleMultiplier = new DoubleMultiplierHeroPower();
            var candidate = new FakeCandidate();
            var note = new NoteEvent(0, Frets.White1);

            // Act
            var canActivate = doubleMultiplier.CanActivate(candidate, note);

            // Assert
            Assert.True(canActivate);
        }

        [Fact]
        public void CanActivate_Sustain_ReturnsTrue()
        {
            // Arrange
            var doubleMultiplier = new DoubleMultiplierHeroPower();
            var candidate = new FakeCandidate();
            var sustain = new SustainEvent(0, 1);

            // Act
            var canActivate = doubleMultiplier.CanActivate(candidate, sustain);

            // Assert
            Assert.True(canActivate);
        }
    }
}

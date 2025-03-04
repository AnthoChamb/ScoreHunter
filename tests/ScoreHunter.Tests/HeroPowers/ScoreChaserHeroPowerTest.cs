using ScoreHunter.Core;
using ScoreHunter.Core.Events;
using ScoreHunter.HeroPowers;
using ScoreHunter.Testing;
using Xunit;

namespace ScoreHunter.Tests.HeroPowers
{
    public class ScoreChaserHeroPowerTest
    {
        [Fact]
        public void Duration_Returns14()
        {
            // Arrange
            var scoreChaser = new ScoreChaserHeroPower();

            // Act
            var duration = scoreChaser.Duration;

            // Assert
            Assert.Equal(14, duration);
        }

        [Fact]
        public void Multiplier_ReturnsMultiplier()
        {
            // Arrange
            var scoreChaser = new ScoreChaserHeroPower();
            var multiplier = 7;

            // Act
            var actual = scoreChaser.Multiplier(multiplier);

            // Assert
            Assert.Equal(multiplier, actual);
        }

        [Fact]
        public void MaxMultiplier_ReturnsDoubleMaxMultiplier()
        {
            // Arrange
            var scoreChaser = new ScoreChaserHeroPower();
            var maxMultiplier = 7;
            var doubleMaxMultiplier = 14;

            // Act
            var actual = scoreChaser.MaxMultiplier(maxMultiplier);

            // Assert
            Assert.Equal(doubleMaxMultiplier, actual);
        }

        [Fact]
        public void CanActivate_MultiplierGreaterThanMaxMultiplier_ReturnsTrue()
        {
            // Arrange
            var scoreChaser = new ScoreChaserHeroPower();
            var candidate = new FakeCandidate
            {
                Streak = 42,
                Multiplier = 8,
                MaxMultiplier = 7,
                StreakPerMultiplier = 6
            };
            var note = new NoteEvent(0, Frets.White1);

            // Act
            var canActivate = scoreChaser.CanActivate(candidate, note);

            // Assert
            Assert.True(canActivate);
        }

        [Fact]
        public void CanActivate_MultiplierEqualsMaxMultiplierAndStreakOneToIncrementMultiplier_ReturnsTrue()
        {
            // Arrange
            var scoreChaser = new ScoreChaserHeroPower();
            var candidate = new FakeCandidate
            {
                Streak = 41,
                Multiplier = 7,
                MaxMultiplier = 7,
                StreakPerMultiplier = 6
            };
            var note = new NoteEvent(0, Frets.White1);

            // Act
            var canActivate = scoreChaser.CanActivate(candidate, note);

            // Assert
            Assert.True(canActivate);
        }

        [Fact]
        public void CanActivate_MultiplierLowerThanMaxMultiplier_ReturnsFalse()
        {
            // Arrange
            var scoreChaser = new ScoreChaserHeroPower();
            var candidate = new FakeCandidate
            {
                Streak = 35,
                Multiplier = 6,
                MaxMultiplier = 7,
                StreakPerMultiplier = 6
            };
            var note = new NoteEvent(0, Frets.White1);

            // Act
            var canActivate = scoreChaser.CanActivate(candidate, note);

            // Assert
            Assert.False(canActivate);
        }

        [Fact]
        public void CanActivate_MultiplierEqualsMaxMultiplierAndStreakMoreThanOneToIncrementMultiplier_ReturnsFalse()
        {
            // Arrange
            var scoreChaser = new ScoreChaserHeroPower();
            var candidate = new FakeCandidate
            {
                Streak = 42,
                Multiplier = 7,
                MaxMultiplier = 7,
                StreakPerMultiplier = 6
            };
            var note = new NoteEvent(0, Frets.White1);

            // Act
            var canActivate = scoreChaser.CanActivate(candidate, note);

            // Assert
            Assert.False(canActivate);
        }

        [Fact]
        public void CanActivate_Sustain_ReturnsFalse()
        {
            // Arrange
            var scoreChaser = new ScoreChaserHeroPower();
            var candidate = new FakeCandidate();
            var sustain = new SustainEvent(0, 1);

            // Act
            var canActivate = scoreChaser.CanActivate(candidate, sustain);

            // Assert
            Assert.False(canActivate);
        }
    }
}

using ScoreHunter.Core;
using ScoreHunter.Core.Events;
using ScoreHunter.HeroPowers;
using ScoreHunter.Tests.Core;
using Xunit;

namespace ScoreHunter.Tests.HeroPowers
{
    public class ScoreChaserHeroPowerTest
    {
        [Fact]
        public void CanActivate_MultiplierGreaterThanMaxMultiplier_ReturnsTrue()
        {
            // Arrange
            var scoreChaser = new ScoreChaserHeroPower();
            var scoring = new FakeCandidate
            {
                Streak = 42,
                Multiplier = 8,
                MaxMultiplier = 7,
                StreakPerMultiplier = 6
            };
            var note = new NoteEvent(0, Frets.White1);

            // Act
            var canActivate = scoreChaser.CanActivate(scoring, note);

            // Assert
            Assert.True(canActivate);
        }


        [Fact]
        public void CanActivate_MultiplierEqualsMaxMultiplierAndStreakOneToIncrementMultiplier_ReturnsTrue()
        {
            // Arrange
            var scoreChaser = new ScoreChaserHeroPower();
            var scoring = new FakeCandidate
            {
                Streak = 41,
                Multiplier = 7,
                MaxMultiplier = 7,
                StreakPerMultiplier = 6
            };
            var note = new NoteEvent(0, Frets.White1);

            // Act
            var canActivate = scoreChaser.CanActivate(scoring, note);

            // Assert
            Assert.True(canActivate);
        }

        [Fact]
        public void CanActivate_MultiplierLowerThanMaxMultiplier_ReturnsFalse()
        {
            // Arrange
            var scoreChaser = new ScoreChaserHeroPower();
            var scoring = new FakeCandidate
            {
                Streak = 35,
                Multiplier = 6,
                MaxMultiplier = 7,
                StreakPerMultiplier = 6
            };
            var note = new NoteEvent(0, Frets.White1);

            // Act
            var canActivate = scoreChaser.CanActivate(scoring, note);

            // Assert
            Assert.False(canActivate);
        }

        [Fact]
        public void CanActivate_MultiplierEqualsMaxMultiplierAndStreakMoreThanOneToIncrementMultiplier_ReturnsFalse()
        {
            // Arrange
            var scoreChaser = new ScoreChaserHeroPower();
            var scoring = new FakeCandidate
            {
                Streak = 42,
                Multiplier = 7,
                MaxMultiplier = 7,
                StreakPerMultiplier = 6
            };
            var note = new NoteEvent(0, Frets.White1);

            // Act
            var canActivate = scoreChaser.CanActivate(scoring, note);

            // Assert
            Assert.False(canActivate);
        }

        [Fact]
        public void CanActivate_Sustain_ReturnsFalse()
        {
            // Arrange
            var scoreChaser = new ScoreChaserHeroPower();
            var scoring = new FakeCandidate();
            var sustain = new SustainEvent(0, Frets.White1);

            // Act
            var canActivate = scoreChaser.CanActivate(scoring, sustain);

            // Assert
            Assert.False(canActivate);
        }
    }
}

using ScoreHunter.Core;
using ScoreHunter.Core.Events;
using ScoreHunter.HeroPowers;
using Xunit;

namespace ScoreHunter.Tests
{
    public class ActivationTest
    {
        [Fact]
        public void End_ScoreChaserHeroPower_ReturnsEventStartPlusHeroPowerDuration()
        {
            // Arrange
            var heroPower = new ScoreChaserHeroPower();
            var note = new NoteEvent(0, Frets.White1);
            var activation = new Activation(heroPower, note, 0, false);

            // Act
            // Assert
            Assert.Equal(14, activation.End);
        }

        [Fact]
        public void End_DoubleMultiplierHeroPower_ReturnsEventStartPlusHeroPowerDuration()
        {
            // Arrange
            var heroPower = new DoubleMultiplierHeroPower();
            var note = new NoteEvent(0, Frets.White1);
            var activation = new Activation(heroPower, note, 0, false);

            // Act
            // Assert
            Assert.Equal(7, activation.End);
        }
    }
}

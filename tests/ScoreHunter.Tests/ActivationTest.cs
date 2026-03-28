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

        [Fact]
        public void EndActivation_EndEqualsEndParameter()
        {
            // Arrange
            var heroPower = new ScoreChaserHeroPower();
            var note = new NoteEvent(0, Frets.White1);
            var activation = new Activation(heroPower, note, 0, false);

            // Act
            activation = activation.EndActivation(1);

            // Assert
            Assert.Equal(1, activation.End);
        }

        [Fact]
        public void ExtendActivation_EndEqualsEndPlusDurationParameter()
        {
            // Arrange
            var heroPower = new ScoreChaserHeroPower();
            var note = new NoteEvent(0, Frets.White1);
            var activation = new Activation(heroPower, note, 0, false);

            // Act
            activation.ExtendActivation(1);

            // Assert
            Assert.Equal(15, activation.End);
        }
    }
}

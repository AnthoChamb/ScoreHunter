using ScoreHunter.Core;
using ScoreHunter.Core.Enums;
using Xunit;

namespace ScoreHunter.Tests
{
    public class ActiveSustainTest
    {
        [Fact]
        public void MoveNext_SingleNote_CountEquals1()
        {
            // Arrange
            var note = new Note(0, 1, Frets.White1, NoteFlags.Sustain);
            var activeSustain = new ActiveSustain(note, 0.023, 0.2);

            // Act
            // Assert
            Assert.True(activeSustain.MoveNext());
            Assert.Equal(1, activeSustain.Count);
            Assert.Equal(0.023, activeSustain.Position);
        }

        [Fact]
        public void MoveNext_BarreChord_CountEquals1()
        {
            // Arrange
            var note = new Note(0, 1, Frets.Barre1, NoteFlags.Sustain);
            var activeSustain = new ActiveSustain(note, 0.023, 0.2);

            // Act
            // Assert
            Assert.True(activeSustain.MoveNext());
            Assert.Equal(1, activeSustain.Count);
            Assert.Equal(0.023, activeSustain.Position);
        }

        [Fact]
        public void MoveNext_NoteLengthSmallerThanSustainLength_ReturnsFalse()
        {
            // Arrange
            var note = new Note(0, 0.01, Frets.White1, NoteFlags.Sustain);
            var activeSustain = new ActiveSustain(note, 0.023, 0.2);

            // Act
            // Assert
            Assert.False(activeSustain.MoveNext());
        }

        [Fact]
        public void WhileMoveNext_SingleNote_CountEquals1PerSustain()
        {
            // Arrange
            var note = new Note(0, 1, Frets.White1, NoteFlags.Sustain);
            var activeSustain = new ActiveSustain(note, 0.023, 0.2);
            var count = 0;

            // Act
            while (activeSustain.MoveNext())
            {
                count += activeSustain.Count;
            }

            // Assert
            Assert.Equal(43, count);
        }

        [Fact]
        public void WhileMoveNext_BarreChord_CountEquals2PerSustain()
        {
            // Arrange
            var note = new Note(0, 1, Frets.Barre1, NoteFlags.Sustain);
            var activeSustain = new ActiveSustain(note, 0.023, 0.2);
            var count = 0;

            // Act
            while (activeSustain.MoveNext())
            {
                count += activeSustain.Count;
            }

            // Assert
            Assert.Equal(86, count);
        }
    }
}

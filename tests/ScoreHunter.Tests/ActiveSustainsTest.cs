using ScoreHunter.Core;
using ScoreHunter.Core.Enums;
using Xunit;

namespace ScoreHunter.Tests
{
    public class ActiveSustainsTest
    {
        [Fact]
        public void MoveNext_SingleNote_CountEquals1()
        {
            // Arrange
            var activeSustains = new ActiveSustains(0.023, 0.2);
            activeSustains.AddNote(new Note(0, 1, Frets.White1, NoteFlags.Sustain));

            // Act
            // Assert
            Assert.True(activeSustains.MoveNext());
            Assert.Equal(1, activeSustains.Count);
            Assert.Equal(0.023, activeSustains.Position);
        }

        [Fact]
        public void MoveNext_BarreChord_CountEquals1()
        {
            // Arrange
            var activeSustains = new ActiveSustains(0.023, 0.2);
            activeSustains.AddNote(new Note(0, 1, Frets.Barre1, NoteFlags.Sustain));

            // Act
            // Assert
            Assert.True(activeSustains.MoveNext());
            Assert.Equal(1, activeSustains.Count);
            Assert.Equal(0.023, activeSustains.Position);
        }

        [Fact]
        public void MoveNext_DoubleChord_CountEquals2()
        {
            // Arrange
            var activeSustains = new ActiveSustains(0.023, 0.2);
            activeSustains.AddNote(new Note(0, 1, Frets.White1, NoteFlags.Sustain));
            activeSustains.AddNote(new Note(0, 1, Frets.White2, NoteFlags.Sustain));

            // Act
            // Assert
            Assert.True(activeSustains.MoveNext());
            Assert.Equal(2, activeSustains.Count);
            Assert.Equal(0.023, activeSustains.Position);
        }

        [Fact]
        public void MoveNext_TripleBarreChord_CountEquals2()
        {
            // Arrange
            var activeSustains = new ActiveSustains(0.023, 0.2);
            activeSustains.AddNote(new Note(0, 1, Frets.Barre1, NoteFlags.Sustain));
            activeSustains.AddNote(new Note(0, 1, Frets.White2, NoteFlags.Sustain));

            // Act
            // Assert
            Assert.True(activeSustains.MoveNext());
            Assert.Equal(2, activeSustains.Count);
            Assert.Equal(0.023, activeSustains.Position);
        }

        [Fact]
        public void MoveNext_TripleChord_CountEquals3()
        {
            // Arrange
            var activeSustains = new ActiveSustains(0.023, 0.2);
            activeSustains.AddNote(new Note(0, 1, Frets.White1, NoteFlags.Sustain));
            activeSustains.AddNote(new Note(0, 1, Frets.White2, NoteFlags.Sustain));
            activeSustains.AddNote(new Note(0, 1, Frets.White3, NoteFlags.Sustain));

            // Act
            // Assert
            Assert.True(activeSustains.MoveNext());
            Assert.Equal(3, activeSustains.Count);
            Assert.Equal(0.023, activeSustains.Position);
        }

        [Fact]
        public void MoveNext_Empty_ReturnsFalse()
        {
            // Arrange
            var activeSustains = new ActiveSustains(0.023, 0.2);

            // Act
            // Assert
            Assert.False(activeSustains.MoveNext());
        }

        [Fact]
        public void MoveNext_NoteLengthSmallerThanSustainLength_ReturnsFalse()
        {
            // Arrange
            var activeSustains = new ActiveSustains(0.023, 0.2);
            activeSustains.AddNote(new Note(0, 0.01, Frets.White1, NoteFlags.Sustain));

            // Act
            // Assert
            Assert.False(activeSustains.MoveNext());
        }

        [Fact]
        public void WhileMoveNext_SingleNote_CountEquals1PerSustain()
        {
            // Arrange
            var activeSustains = new ActiveSustains(0.023, 0.2);
            activeSustains.AddNote(new Note(0, 1, Frets.White1, NoteFlags.Sustain));
            var count = 0;

            // Act
            while (activeSustains.MoveNext())
            {
                count += activeSustains.Count;
            }

            // Assert
            Assert.Equal(43, count);
        }

        [Fact]
        public void WhileMoveNext_BarreChord_CountEquals2PerSustain()
        {
            // Arrange
            var activeSustains = new ActiveSustains(0.023, 0.2);
            activeSustains.AddNote(new Note(0, 1, Frets.Barre1, NoteFlags.Sustain));
            var count = 0;

            // Act
            while (activeSustains.MoveNext())
            {
                count += activeSustains.Count;
            }

            // Assert
            Assert.Equal(86, count);
        }

        [Fact]
        public void WhileMoveNext_DoubleChord_CountEquals2PerSustain()
        {
            // Arrange
            var activeSustains = new ActiveSustains(0.023, 0.2);
            activeSustains.AddNote(new Note(0, 1, Frets.White1, NoteFlags.Sustain));
            activeSustains.AddNote(new Note(0, 1, Frets.White2, NoteFlags.Sustain));
            var count = 0;

            // Act
            while (activeSustains.MoveNext())
            {
                count += activeSustains.Count;
            }

            // Assert
            Assert.Equal(86, count);
        }

        [Fact]
        public void WhileMoveNext_TripleBarreChord_CountEquals3PerSustain()
        {
            // Arrange
            var activeSustains = new ActiveSustains(0.023, 0.2);
            activeSustains.AddNote(new Note(0, 1, Frets.Barre1, NoteFlags.Sustain));
            activeSustains.AddNote(new Note(0, 1, Frets.White2, NoteFlags.Sustain));
            var count = 0;

            // Act
            while (activeSustains.MoveNext())
            {
                count += activeSustains.Count;
            }

            // Assert
            Assert.Equal(129, count);
        }

        [Fact]
        public void WhileMoveNext_TripleChord_CountEquals3PerSustain()
        {
            // Arrange
            var activeSustains = new ActiveSustains(0.023, 0.2);
            activeSustains.AddNote(new Note(0, 1, Frets.White1, NoteFlags.Sustain));
            activeSustains.AddNote(new Note(0, 1, Frets.White2, NoteFlags.Sustain));
            activeSustains.AddNote(new Note(0, 1, Frets.White3, NoteFlags.Sustain));
            var count = 0;

            // Act
            while (activeSustains.MoveNext())
            {
                count += activeSustains.Count;
            }

            // Assert
            Assert.Equal(129, count);
        }
    }
}

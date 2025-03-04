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
            var note = new Note(0, 1, Frets.White1, NoteFlags.Sustain);
            var activeSustains = new ActiveSustains(note, 0.023);

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
            var note = new Note(0, 1, Frets.Barre1, NoteFlags.Sustain);
            var activeSustains = new ActiveSustains(note, 0.023);

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
            var note = new Note(0, 1, Frets.White1, NoteFlags.Sustain);
            var activeSustains = new ActiveSustains(note, 0.023);
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
            var note = new Note(0, 1, Frets.Barre1, NoteFlags.Sustain);
            var activeSustains = new ActiveSustains(note, 0.023);
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
            var note = new Note(0, 1, Frets.White1, NoteFlags.Sustain);
            var activeSustains = new ActiveSustains(note, 0.023);
            activeSustains.AddNote(new Note(0, 1, Frets.White2, NoteFlags.Sustain));
            activeSustains.AddNote(new Note(0, 1, Frets.White3, NoteFlags.Sustain));

            // Act
            // Assert
            Assert.True(activeSustains.MoveNext());
            Assert.Equal(3, activeSustains.Count);
            Assert.Equal(0.023, activeSustains.Position);
        }

        [Fact]
        public void MoveNext_NoteLengthSmallerThanSustainLength_ReturnsFalse()
        {
            // Arrange
            var note = new Note(0, 0.01, Frets.White1, NoteFlags.Sustain);
            var activeSustains = new ActiveSustains(note, 0.023);

            // Act
            // Assert
            Assert.False(activeSustains.MoveNext());
            Assert.Equal(0, activeSustains.Count);
            Assert.Equal(0.023, activeSustains.Position);
        }

        [Fact]
        public void End_EqualsNoteEnd()
        {
            // Arrange
            var note = new Note(0, 1, Frets.White1, NoteFlags.Sustain);

            // Act
            var activeSustains = new ActiveSustains(note, 0.023);

            // Assert
            Assert.Equal(1, activeSustains.End);
        }

        [Fact]
        public void AddNote_NoteEndLessThanOrEqualEnd_EndEqualsEnd()
        {
            // Arrange
            var note = new Note(0, 2, Frets.Barre1, NoteFlags.Sustain);
            var activeSustains = new ActiveSustains(note, 0.023);

            // Act
            activeSustains.AddNote(new Note(0, 1, Frets.White2, NoteFlags.Sustain));

            // Assert
            Assert.Equal(2, activeSustains.End);
        }

        [Fact]
        public void AddNote_NoteEndGreaterThanEnd_EndEqualsNoteEnd()
        {
            // Arrange
            var note = new Note(0, 1, Frets.White1, NoteFlags.Sustain);
            var activeSustains = new ActiveSustains(note, 0.023);

            // Act
            activeSustains.AddNote(new Note(0, 2, Frets.White2, NoteFlags.Sustain));

            // Assert
            Assert.Equal(2, activeSustains.End);
        }
    }
}

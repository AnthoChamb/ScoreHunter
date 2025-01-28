using ScoreHunter.Core;
using ScoreHunter.Core.Enums;
using ScoreHunter.Core.Events;
using ScoreHunter.Testing;
using Xunit;

namespace ScoreHunter.Tests.Events
{
    public class NoteEventTest
    {
        [Fact]
        public void IsHeroPowerStart_HeroPowerFlagsNone_ReturnsFalse()
        {
            // Arrange
            var note = new NoteEvent(0, Frets.White1);

            // Act
            var isHeroPowerStart = note.IsHeroPowerStart;

            // Assert
            Assert.False(isHeroPowerStart);
        }

        [Fact]
        public void IsHeroPowerStart_HeroPowerFlagsHeroPowerStart_ReturnsTrue()
        {
            // Arrange
            var note = new NoteEvent(0, Frets.White1, HeroPowerFlags.HeroPowerStart);

            // Act
            var isHeroPowerStart = note.IsHeroPowerStart;

            // Assert
            Assert.True(isHeroPowerStart);
        }

        [Fact]
        public void IsHeroPowerEnd_HeroPowerFlagsNone_ReturnsFalse()
        {
            // Arrange
            var note = new NoteEvent(0, Frets.White1);

            // Act
            var isHeroPowerEnd = note.IsHeroPowerEnd;

            // Assert
            Assert.False(isHeroPowerEnd);
        }

        [Fact]
        public void IsHeroPowerEnd_HeroPowerFlagsHeroPowerEnd_ReturnsTrue()
        {
            // Arrange
            var note = new NoteEvent(0, Frets.White1, HeroPowerFlags.HeroPowerEnd);

            // Act
            var isHeroPowerEnd = note.IsHeroPowerEnd;

            // Assert
            Assert.True(isHeroPowerEnd);
        }

        [Fact]
        public void Accept_NoteVisited()
        {
            // Arrange
            var note = new NoteEvent(0, Frets.White1);
            var visitor = new FakeEventVisitor();

            // Act
            note.Accept(visitor);

            // Assert
            var visited = new[] { note };
            Assert.Equal(visited, visitor.Visited);
            Assert.Equal(visited, visitor.VisitedNotes);
        }
    }
}

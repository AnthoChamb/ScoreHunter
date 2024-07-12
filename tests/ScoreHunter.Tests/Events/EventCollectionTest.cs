using ScoreHunter.Core;
using ScoreHunter.Core.Events;
using System.Linq;
using Xunit;

namespace ScoreHunter.Tests.Events
{
    public class EventCollectionTest
    {
        [Fact]
        public void Events_NoEvent_ReturnsEmpty()
        {
            // Arrange
            var eventCollection = new EventCollection(null);

            // Act
            var events = eventCollection.Events;

            // Assert
            Assert.Equal(Enumerable.Empty<Event>(), events);
        }

        [Fact]
        public void Events_OneEvent_ReturnsOneEvent()
        {
            // Arrange
            var note = new NoteEvent(0, Frets.White1);
            var eventNode = new EventNode(note);
            var eventCollection = new EventCollection(eventNode);

            // Act
            var events = eventCollection.Events;

            // Assert
            Assert.Equal(new[] { note }, events);
        }

        [Fact]
        public void Events_ManyEvents_ReturnsManyEvents()
        {
            // Arrange
            var firstNote = new NoteEvent(0, Frets.White1);
            var firstEventNode = new EventNode(firstNote);
            var nextNote = new NoteEvent(1, Frets.White1);
            var nextEventNode = new EventNode(nextNote);
            firstEventNode.Next = nextEventNode;
            var eventCollection = new EventCollection(firstEventNode);

            // Act
            var events = eventCollection.Events;

            // Assert
            Assert.Equal(new[] { firstNote, nextNote }, events);
        }

        [Fact]
        public void GetFirstEventNode_NoEvent_ReturnsNull()
        {
            // Arrange
            var eventCollection = new EventCollection(null);

            // Act
            var first = eventCollection.GetFirstEventNode();

            // Assert
            Assert.Null(first);
        }

        [Fact]
        public void GetFirstEventNode_Event_ReturnsEventNode()
        {
            // Arrange
            var note = new NoteEvent(0, Frets.White1);
            var eventNode = new EventNode(note);
            var eventCollection = new EventCollection(eventNode);

            // Act
            var first = eventCollection.GetFirstEventNode();

            // Assert
            Assert.Equal(eventNode, first);
        }
    }
}

using FsgXmk;
using FsgXmk.Abstractions.Enums;
using ScoreHunter.Xmk.Builders;
using System.Linq;
using Xunit;

namespace ScoreHunter.Tests.Xmk.Builders
{
    public class XmkTrackBuilderTest
    {
        [Theory]
        [InlineData((Note)83)]
        [InlineData((Note)85)]
        public void AddXmkEvent_EventTypeEqualsHighway_NoteNotEqualsHighway_DoesNotAddHighwayPhrase(Note note)
        {
            // Arrange
            var builder = new XmkTrackBuilder();
            var xmkEvent = new XmkEvent(0, ChordFlags.None, EventType.Highway, note, 0, 1, 100);

            // Act
            builder.AddXmkEvent(xmkEvent);

            // Assert
            var track = builder.Build();
            Assert.False(track.HighwayPhrases.Any());
        }

        [Fact]
        public void AddXmkEvent_EventTypeEqualsHighway_NoteEqualsHighway_AddsHighwayPhrase()
        {
            // Arrange
            var builder = new XmkTrackBuilder();
            var xmkEvent = new XmkEvent(0, ChordFlags.None, EventType.Highway, Note.Highway, 0, 1, 100);

            // Act
            builder.AddXmkEvent(xmkEvent);

            // Assert
            var track = builder.Build();
            var highwayPhrase = track.HighwayPhrases.SingleOrDefault();
            Assert.NotNull(highwayPhrase);
            Assert.Equal(0, highwayPhrase.Start);
            Assert.Equal(1, highwayPhrase.End);
        }
    }
}

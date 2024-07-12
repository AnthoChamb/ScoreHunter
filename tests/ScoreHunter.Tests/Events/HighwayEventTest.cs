using ScoreHunter.Core.Events;
using ScoreHunter.Tests.Core;
using Xunit;

namespace ScoreHunter.Tests.Events
{
    public class HighwayEventTest
    {
        [Fact]
        public void Accept_HighwayVisited()
        {
            // Arrange
            var highway = new HighwayEvent(0, 1);
            var visitor = new FakeEventVisitor();

            // Act
            highway.Accept(visitor);

            // Assert
            var visited = new[] { highway };
            Assert.Equal(visited, visitor.Visited);
            Assert.Equal(visited, visitor.VisitedHighways);
        }
    }
}

﻿using ScoreHunter.Core;
using ScoreHunter.Core.Events;
using ScoreHunter.Tests.Core;
using Xunit;

namespace ScoreHunter.Tests.Events
{
    public class SustainEventTest
    {
        [Fact]
        public void Accept_SustainVisited()
        {
            // Arrange
            var sustain = new SustainEvent(0, Frets.White1);
            var visitor = new FakeEventVisitor();

            // Act
            sustain.Accept(visitor);

            // Assert
            var visited = new[] { sustain };
            Assert.Equal(visited, visitor.Visited);
            Assert.Equal(visited, visitor.VisitedSustains);
        }
    }
}

using ScoreHunter.Core;
using ScoreHunter.Core.Enums;
using ScoreHunter.Core.Events;
using ScoreHunter.Core.Interfaces;
using ScoreHunter.HeroPowers;
using ScoreHunter.Options;
using System.Linq;
using Xunit;

namespace ScoreHunter.Tests
{
    public class CandidateTest
    {
        [Fact]
        public void TrySetHeroPower_ActivationEndEqualsNoteStart()
        {
            // Arrange
            ICandidate candidate = new Candidate(new ScoringOptions());

            var doubleMultiplier = new DoubleMultiplierHeroPower();
            var scoreChaser = new ScoreChaserHeroPower();

            var doubleMultiplierNote = new NoteEvent(0, Frets.White1, HeroPowerFlags.HeroPowerStart | HeroPowerFlags.HeroPowerEnd);
            var activationNote = new NoteEvent(1, Frets.White1);
            var scoreChaserNote = new NoteEvent(2, Frets.White1, HeroPowerFlags.HeroPowerStart);

            candidate = candidate.Advance(doubleMultiplierNote);
            candidate.TrySetHeroPower(doubleMultiplierNote, doubleMultiplier, out candidate);
            candidate = candidate.HitNote(doubleMultiplierNote);

            candidate = candidate.Advance(activationNote);
            candidate.TryActivate(activationNote, out candidate);
            candidate = candidate.HitNote(activationNote);

            candidate = candidate.Advance(scoreChaserNote);

            // Act
            candidate.TrySetHeroPower(scoreChaserNote, scoreChaser, out candidate);

            // Assert
            var activation = candidate.Activations.FirstOrDefault();
            Assert.NotNull(activation);
            Assert.Equal(2, activation.End);
        }
    }
}

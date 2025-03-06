using ScoreHunter.Core.Enums;
using ScoreHunter.Core.Events;
using ScoreHunter.Core.Interfaces;
using ScoreHunter.Options;
using System.Linq;

namespace ScoreHunter.Factories
{
    public class TrackEventCollectionFactory
    {
        private readonly TrackOptions _options;

        public TrackEventCollectionFactory(TrackOptions options)
        {
            _options = options;
        }

        public EventCollection Create(ITrack track, Difficulty difficulty)
        {
            EventNode first = null;
            EventNode previous = null;

            if (track.Difficulties.TryGetValue(difficulty, out var difficultyTrack))
            {
                var currentChordNode = difficultyTrack.GetFirstChordNode();
                ActiveSustains activeSustains = new ActiveSustains(_options.SustainLength);
                var hasActiveSustains = false;
                var heroPowerCount = 0;

                using (var heroPowerPhrasesEnumerator = difficultyTrack.HeroPowerPhrases.GetEnumerator())
                {
                    var hasHeroPowerPhrase = heroPowerPhrasesEnumerator.MoveNext() && (heroPowerCount++ < _options.MaxHeroPowerCount || _options.MaxHeroPowerCount == -1);

                    while (currentChordNode != null)
                    {
                        var currentChord = currentChordNode.Value;

                        while (hasHeroPowerPhrase && currentChord.Start >= heroPowerPhrasesEnumerator.Current.End)
                        {
                            hasHeroPowerPhrase = heroPowerPhrasesEnumerator.MoveNext() && (heroPowerCount++ < _options.MaxHeroPowerCount || _options.MaxHeroPowerCount == -1);
                        }

                        var heroPowerFlags = HeroPowerFlags.None;

                        if (hasHeroPowerPhrase && IsEventInPhrase(currentChord.Start, heroPowerPhrasesEnumerator.Current))
                        {
                            if (currentChordNode.Previous == null || !IsEventInPhrase(currentChordNode.Previous.Value.Start, heroPowerPhrasesEnumerator.Current))
                            {
                                heroPowerFlags |= HeroPowerFlags.HeroPowerStart;
                            }

                            if (currentChordNode.Next == null || !IsEventInPhrase(currentChordNode.Next.Value.Start, heroPowerPhrasesEnumerator.Current))
                            {
                                heroPowerFlags |= HeroPowerFlags.HeroPowerEnd;
                            }
                        }

                        var noteEvent = new NoteEvent(currentChord.Start, currentChord.Frets, heroPowerFlags);
                        var noteEventNode = new EventNode(noteEvent);

                        if (first == null)
                        {
                            first = previous = noteEventNode;
                        }
                        else
                        {
                            previous.Next = noteEventNode;
                            previous = noteEventNode;
                        }

                        foreach (var sustainNote in currentChord.Notes.Where(note => note.IsSustain))
                        {
                            activeSustains.AddNote(sustainNote);
                        }

                        if (!hasActiveSustains)
                        {
                            hasActiveSustains = activeSustains.MoveNext();
                        }

                        while (hasActiveSustains &&
                            (currentChordNode.Next == null ||
                            activeSustains.Position < currentChordNode.Next.Value.Start))
                        {
                            var sustainEvent = new SustainEvent(activeSustains.Position, activeSustains.Count);
                            var sustainEventNode = new EventNode(sustainEvent);
                            previous.Next = sustainEventNode;
                            previous = sustainEventNode;
                            hasActiveSustains = activeSustains.MoveNext();
                        }

                        currentChordNode = currentChordNode.Next;
                    }
                }
            }

            var current = first;
            previous = null;

            foreach (var highwayPhrase in track.HighwayPhrases)
            {
                var highwayEvent = new HighwayEvent(highwayPhrase.Start, highwayPhrase.End);
                var highwwayEventNode = new EventNode(highwayEvent);

                while (current != null && current.Value.Start < highwayEvent.Start)
                {
                    previous = current;
                    current = current.Next;
                }

                if (previous == null)
                {
                    highwwayEventNode.Next = first;
                    first = highwwayEventNode;
                }
                else
                {
                    highwwayEventNode.Next = current;
                    previous.Next = highwwayEventNode;
                }

                previous = highwwayEventNode;
            }

            return new EventCollection(first);
        }

        private bool IsEventInPhrase(double start, IPhrase phrase) => start >= phrase.Start && start < phrase.End;
    }
}

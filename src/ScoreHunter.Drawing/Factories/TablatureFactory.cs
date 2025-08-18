using ScoreHunter.Core.Enums;
using ScoreHunter.Core.Interfaces;
using ScoreHunter.Drawing.Abstractions.Interfaces;
using ScoreHunter.Drawing.Options;
using System.Collections.Generic;
using System.Linq;

namespace ScoreHunter.Drawing.Factories
{
    public class TablatureFactory
    {
        private readonly TablatureOptions _options;

        public TablatureFactory(TablatureOptions options)
        {
            _options = options;
        }

        public ITablature Create(ITrack track, Difficulty difficulty, IPath path)
        {
            var staves = new List<IStaff>();

            if (track.Difficulties.TryGetValue(difficulty, out var difficultyTrack))
            {
                using (var timeSignaturesEnumerator = track.TimeSignatures.GetEnumerator())
                using (var temposEnumerator = track.Tempos.GetEnumerator())
                using (var notesEnumerator = difficultyTrack.Notes.GetEnumerator())
                using (var heroPowerPhrasesEnumerator = difficultyTrack.HeroPowerPhrases.GetEnumerator())
                using (var highwayPhrasesEnumerator = track.HighwayPhrases.GetEnumerator())
                {
                    var hasTimeSignature = timeSignaturesEnumerator.MoveNext();
                    var hasTempo = temposEnumerator.MoveNext();

                    if (hasTimeSignature && hasTempo)
                    {
                        var startTicks = 0;
                        var tempos = new List<ITempo>(1);
                        var sustains = new List<DrawnSustain>();
                        var heroPowerPhrases = new List<DrawnPhrase>();
                        var highwayPhrases = new List<DrawnPhrase>();

                        var currentTimeSignature = timeSignaturesEnumerator.Current;
                        hasTimeSignature = timeSignaturesEnumerator.MoveNext();

                        var currentTempo = temposEnumerator.Current;
                        tempos.Add(currentTempo);
                        hasTempo = temposEnumerator.MoveNext();

                        var hasNote = notesEnumerator.MoveNext();
                        var hasHeroPowerPhrase = heroPowerPhrasesEnumerator.MoveNext();
                        var hasHighwayPhrase = highwayPhrasesEnumerator.MoveNext();

                        void AddHeroPowerPhrases(int heroPowerPhrasesEndTicks)
                        {
                            IPhrase heroPowerPhrase;
                            int ticks;
                            while (hasHeroPowerPhrase && (ticks = currentTempo.SecondsToTicks((heroPowerPhrase = heroPowerPhrasesEnumerator.Current).Start, track.TicksPerQuarterNote)) < heroPowerPhrasesEndTicks)
                            {
                                var phrase = new DrawnPhrase(heroPowerPhrase, ticks);
                                heroPowerPhrases.Add(phrase);
                                SetHeroPowerPhraseEndTicks(phrase);
                                hasHeroPowerPhrase = heroPowerPhrasesEnumerator.MoveNext();
                            }
                        }

                        void SetHeroPowerPhraseEndTicks(DrawnPhrase heroPowerPhrase)
                        {
                            int ticks;
                            if (!hasTempo)
                            {
                                heroPowerPhrase.EndTicks = currentTempo.SecondsToTicks(heroPowerPhrase.End, track.TicksPerQuarterNote);
                            }
                            else if ((ticks = currentTempo.SecondsToTicks(heroPowerPhrase.End, track.TicksPerQuarterNote)) < temposEnumerator.Current.Ticks)
                            {
                                heroPowerPhrase.EndTicks = ticks;
                            }
                        }

                        void AddHighwayPhrases(int highwayPhrasesEndTicks)
                        {
                            IPhrase highwayPhrase;
                            int ticks;
                            while (hasHighwayPhrase && (ticks = currentTempo.SecondsToTicks((highwayPhrase = highwayPhrasesEnumerator.Current).Start, track.TicksPerQuarterNote)) < highwayPhrasesEndTicks)
                            {
                                var phrase = new DrawnPhrase(highwayPhrase, ticks);
                                highwayPhrases.Add(phrase);
                                SetHighwayPhraseEndTicks(phrase);
                                hasHighwayPhrase = highwayPhrasesEnumerator.MoveNext();
                            }
                        }

                        void SetHighwayPhraseEndTicks(DrawnPhrase highwayPhrase)
                        {
                            int ticks;
                            if (!hasTempo)
                            {
                                highwayPhrase.EndTicks = currentTempo.SecondsToTicks(highwayPhrase.End, track.TicksPerQuarterNote);
                            }
                            else if ((ticks = currentTempo.SecondsToTicks(highwayPhrase.End, track.TicksPerQuarterNote)) < temposEnumerator.Current.Ticks)
                            {
                                highwayPhrase.EndTicks = ticks;
                            }
                        }

                        while (hasNote || hasTempo || sustains.Count > 0)
                        {
                            var staffStartTicks = startTicks;
                            var measures = new List<IMeasure>(_options.TicksPerStaff / currentTimeSignature.TicksPerMeasure(track.TicksPerQuarterNote));

                            while ((hasNote || hasTempo || sustains.Any(sustain => sustain.EndTicks > startTicks)) &&
                                (startTicks - staffStartTicks) <= (_options.TicksPerStaff - currentTimeSignature.TicksPerMeasure(track.TicksPerQuarterNote)))
                            {
                                var start = currentTempo.TicksToSeconds(startTicks, track.TicksPerQuarterNote);
                                var endTicks = startTicks + currentTimeSignature.TicksPerMeasure(track.TicksPerQuarterNote);
                                var notes = new List<IDrawnNote>();

                                void AddNotes(int notesEndTicks)
                                {
                                    INote note;
                                    int ticks;
                                    while (hasNote && (ticks = currentTempo.SecondsToTicks((note = notesEnumerator.Current).Start, track.TicksPerQuarterNote)) < notesEndTicks)
                                    {
                                        notes.Add(new DrawnNote(note, ticks));
                                        if (note.IsSustain)
                                        {
                                            var sustain = new DrawnSustain(note, ticks);
                                            sustains.Add(sustain);
                                            SetSustainEndTicks(sustain);
                                        }
                                        hasNote = notesEnumerator.MoveNext();
                                    }
                                }

                                void SetSustainEndTicks(DrawnSustain sustain)
                                {
                                    int ticks;
                                    if (!hasTempo)
                                    {
                                        sustain.EndTicks = currentTempo.SecondsToTicks(sustain.End, track.TicksPerQuarterNote);
                                    }
                                    else if ((ticks = currentTempo.SecondsToTicks(sustain.End, track.TicksPerQuarterNote)) < temposEnumerator.Current.Ticks)
                                    {
                                        sustain.EndTicks = ticks;
                                    }
                                }

                                while (hasTempo && temposEnumerator.Current.Ticks < endTicks)
                                {
                                    var tempoEndTicks = temposEnumerator.Current.Ticks;
                                    AddNotes(tempoEndTicks);
                                    AddHeroPowerPhrases(tempoEndTicks);
                                    AddHighwayPhrases(tempoEndTicks);

                                    currentTempo = temposEnumerator.Current;
                                    tempos.Add(currentTempo);
                                    hasTempo = temposEnumerator.MoveNext();

                                    foreach (var sustain in sustains)
                                    {
                                        if (sustain.EndTicks == -1)
                                        {
                                            SetSustainEndTicks(sustain);
                                        }
                                    }

                                    foreach (var heroPowerPhrase in heroPowerPhrases)
                                    {
                                        if (heroPowerPhrase.EndTicks == -1)
                                        {
                                            SetHeroPowerPhraseEndTicks(heroPowerPhrase);
                                        }
                                    }

                                    foreach (var highwayPhrase in highwayPhrases)
                                    {
                                        if (highwayPhrase.EndTicks == -1)
                                        {
                                            SetHighwayPhraseEndTicks(highwayPhrase);
                                        }
                                    }
                                }

                                AddNotes(endTicks);

                                measures.Add(new Measure(startTicks,
                                                         endTicks,
                                                         currentTimeSignature,
                                                         tempos,
                                                         notes));


                                startTicks = endTicks;
                                tempos = new List<ITempo>(0);

                                if (hasTimeSignature && timeSignaturesEnumerator.Current.Ticks <= startTicks)
                                {
                                    currentTimeSignature = timeSignaturesEnumerator.Current;
                                    hasTimeSignature = timeSignaturesEnumerator.MoveNext();
                                }
                            }

                            AddHeroPowerPhrases(startTicks);
                            AddHighwayPhrases(startTicks);

                            var nextSustains = new List<DrawnSustain>(sustains.Count);
                            var nextHeroPowerPhrases = new List<DrawnPhrase>(heroPowerPhrases.Count);
                            var nextHighwayPhrases = new List<DrawnPhrase>(highwayPhrases.Count);

                            foreach (var sustain in sustains)
                            {
                                if (sustain.EndTicks == -1 || sustain.EndTicks > startTicks)
                                {
                                    nextSustains.Add(new DrawnSustain(sustain, startTicks));
                                    sustain.EndTicks = startTicks;
                                }
                            }

                            foreach (var heroPowerPhrase in heroPowerPhrases)
                            {
                                if (heroPowerPhrase.EndTicks == -1 || heroPowerPhrase.EndTicks > startTicks)
                                {
                                    nextHeroPowerPhrases.Add(new DrawnPhrase(heroPowerPhrase, startTicks));
                                    heroPowerPhrase.EndTicks = startTicks;
                                }
                            }

                            foreach (var highwayPhrase in highwayPhrases)
                            {
                                if (highwayPhrase.EndTicks == -1 || highwayPhrase.EndTicks > startTicks)
                                {
                                    nextHighwayPhrases.Add(new DrawnPhrase(highwayPhrase, startTicks));
                                    highwayPhrase.EndTicks = startTicks;
                                }
                            }

                            staves.Add(new Staff(staffStartTicks,
                                                 startTicks,
                                                 measures,
                                                 sustains,
                                                 heroPowerPhrases,
                                                 highwayPhrases,
                                                 // TODO: Add list,
                                                 Enumerable.Empty<IDrawnActivation>()));

                            sustains = nextSustains;
                            heroPowerPhrases = nextHeroPowerPhrases;
                            highwayPhrases = nextHighwayPhrases;
                        }
                    }
                }
            }
            return new Tablature(track.TicksPerQuarterNote, _options.TicksPerStaff, staves);
        }
    }
}

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
                {
                    var hasTimeSignature = timeSignaturesEnumerator.MoveNext();
                    var hasTempo = temposEnumerator.MoveNext();

                    if (hasTimeSignature && hasTempo)
                    {
                        var startTicks = 0;
                        var tempos = new List<ITempo>(1);
                        var sustains = new List<DrawnSustain>();

                        var currentTimeSignature = timeSignaturesEnumerator.Current;
                        hasTimeSignature = timeSignaturesEnumerator.MoveNext();

                        var currentTempo = temposEnumerator.Current;
                        tempos.Add(currentTempo);
                        hasTempo = temposEnumerator.MoveNext();

                        var hasNote = notesEnumerator.MoveNext();

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
                                    AddNotes(temposEnumerator.Current.Ticks);

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

                            var nextSustains = new List<DrawnSustain>(sustains.Count);

                            foreach (var sustain in sustains)
                            {
                                if (sustain.EndTicks == -1 || sustain.EndTicks > startTicks)
                                {
                                    nextSustains.Add(new DrawnSustain(sustain, startTicks));
                                    sustain.EndTicks = startTicks;
                                }
                            }

                            staves.Add(new Staff(staffStartTicks,
                                                 startTicks,
                                                 measures,
                                                 sustains,
                                                 // TODO: Add lists,
                                                 Enumerable.Empty<IDrawnPhrase>(),
                                                 Enumerable.Empty<IDrawnPhrase>(),
                                                 Enumerable.Empty<IDrawnActivation>()));

                            sustains = nextSustains;
                        }
                    }
                }
            }
            return new Tablature(track.TicksPerQuarterNote, _options.TicksPerStaff, staves);
        }
    }
}

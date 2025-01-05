using FsgXmk.Abstractions;
using FsgXmk.Abstractions.Enums;
using FsgXmk.Abstractions.Interfaces;
using ScoreHunter.Core;
using ScoreHunter.Core.Builders;
using ScoreHunter.Core.Enums;

using EventNote = FsgXmk.Abstractions.Enums.Note;

namespace ScoreHunter.Xmk.Builders
{
    public class XmkTrackBuilder
    {
        private readonly TrackBuilder _builder;

        public XmkTrackBuilder()
        {
            _builder = new TrackBuilder().WithTicksPerQuarterNote(XmkConstants.TicksPerQuarterNote);
        }

        public XmkTrackBuilder AddXmkTempo(IXmkTempo xmkTempo)
        {
            _builder.AddTempo(CreateTempo(xmkTempo));
            return this;
        }

        public XmkTrackBuilder AddXmkTimeSignature(IXmkTimeSignature xmkTimeSignature)
        {
            _builder.AddTimeSignature(CreateTimeSignature(xmkTimeSignature));
            return this;
        }

        public XmkTrackBuilder AddXmkEvent(IXmkEvent xmkEvent)
        {
            switch (xmkEvent.Type)
            {
                case EventType.Section:
                case EventType.HopoDetection:
                    break;
                case EventType.Highway:
                    _builder.AddHighwayPhrase(CreatePhrase(xmkEvent));
                    break;
                default:
                    AddXmkNoteEvent(xmkEvent);
                    break;
            }

            return this;
        }

        private void AddXmkNoteEvent(IXmkEvent xmkEvent)
        {
            switch (xmkEvent.Note)
            {
                case EventNote.CasualHp:
                    _builder.AddHeroPowerPhrase(Difficulty.Casual, CreatePhrase(xmkEvent));
                    break;
                case EventNote.EasyHp:
                    _builder.AddHeroPowerPhrase(Difficulty.Easy, CreatePhrase(xmkEvent));
                    break;
                case EventNote.MediumHp:
                    _builder.AddHeroPowerPhrase(Difficulty.Medium, CreatePhrase(xmkEvent));
                    break;
                case EventNote.HardHp:
                    _builder.AddHeroPowerPhrase(Difficulty.Hard, CreatePhrase(xmkEvent));
                    break;
                case EventNote.ExpertHp:
                    _builder.AddHeroPowerPhrase(Difficulty.Expert, CreatePhrase(xmkEvent));
                    break;
                default:
                    if (xmkEvent.Chord.HasFlag(ChordFlags.Barre))
                    {
                        switch (xmkEvent.Note)
                        {
                            case EventNote.EasyB1:
                            case EventNote.EasyW1:
                                _builder.AddNote(Difficulty.Easy, CreateNote(xmkEvent, Frets.Barre1));
                                break;
                            case EventNote.EasyB2:
                            case EventNote.EasyW2:
                                _builder.AddNote(Difficulty.Easy, CreateNote(xmkEvent, Frets.Barre2));
                                break;
                            case EventNote.EasyB3:
                            case EventNote.EasyW3:
                                _builder.AddNote(Difficulty.Easy, CreateNote(xmkEvent, Frets.Barre3));
                                break;
                            case EventNote.MediumB1:
                            case EventNote.MediumW1:
                                _builder.AddNote(Difficulty.Medium, CreateNote(xmkEvent, Frets.Barre1));
                                break;
                            case EventNote.MediumB2:
                            case EventNote.MediumW2:
                                _builder.AddNote(Difficulty.Medium, CreateNote(xmkEvent, Frets.Barre2));
                                break;
                            case EventNote.MediumB3:
                            case EventNote.MediumW3:
                                _builder.AddNote(Difficulty.Medium, CreateNote(xmkEvent, Frets.Barre3));
                                break;
                            case EventNote.HardB1:
                            case EventNote.HardW1:
                                _builder.AddNote(Difficulty.Hard, CreateNote(xmkEvent, Frets.Barre1));
                                break;
                            case EventNote.HardB2:
                            case EventNote.HardW2:
                                _builder.AddNote(Difficulty.Hard, CreateNote(xmkEvent, Frets.Barre2));
                                break;
                            case EventNote.HardB3:
                            case EventNote.HardW3:
                                _builder.AddNote(Difficulty.Hard, CreateNote(xmkEvent, Frets.Barre3));
                                break;
                            case EventNote.ExpertB1:
                            case EventNote.ExpertW1:
                                _builder.AddNote(Difficulty.Expert, CreateNote(xmkEvent, Frets.Barre1));
                                break;
                            case EventNote.ExpertB2:
                            case EventNote.ExpertW2:
                                _builder.AddNote(Difficulty.Expert, CreateNote(xmkEvent, Frets.Barre2));
                                break;
                            case EventNote.ExpertB3:
                            case EventNote.ExpertW3:
                                _builder.AddNote(Difficulty.Expert, CreateNote(xmkEvent, Frets.Barre3));
                                break;
                        }
                    }
                    else
                    {
                        switch (xmkEvent.Note)
                        {
                            case EventNote.CasualOpen:
                                _builder.AddNote(Difficulty.Casual, CreateNote(xmkEvent, Frets.Open));
                                break;
                            case EventNote.EasyB1:
                                _builder.AddNote(Difficulty.Easy, CreateNote(xmkEvent, Frets.Black1));
                                break;
                            case EventNote.EasyW1:
                                _builder.AddNote(Difficulty.Easy, CreateNote(xmkEvent, Frets.White1));
                                break;
                            case EventNote.EasyB2:
                                _builder.AddNote(Difficulty.Easy, CreateNote(xmkEvent, Frets.Black2));
                                break;
                            case EventNote.EasyW2:
                                _builder.AddNote(Difficulty.Easy, CreateNote(xmkEvent, Frets.White2));
                                break;
                            case EventNote.EasyB3:
                                _builder.AddNote(Difficulty.Easy, CreateNote(xmkEvent, Frets.Black3));
                                break;
                            case EventNote.EasyW3:
                                _builder.AddNote(Difficulty.Easy, CreateNote(xmkEvent, Frets.White3));
                                break;
                            case EventNote.EasyOpen:
                                _builder.AddNote(Difficulty.Easy, CreateNote(xmkEvent, Frets.Open));
                                break;
                            case EventNote.MediumB1:
                                _builder.AddNote(Difficulty.Medium, CreateNote(xmkEvent, Frets.Black1));
                                break;
                            case EventNote.MediumW1:
                                _builder.AddNote(Difficulty.Medium, CreateNote(xmkEvent, Frets.White1));
                                break;
                            case EventNote.MediumB2:
                                _builder.AddNote(Difficulty.Medium, CreateNote(xmkEvent, Frets.Black2));
                                break;
                            case EventNote.MediumW2:
                                _builder.AddNote(Difficulty.Medium, CreateNote(xmkEvent, Frets.White2));
                                break;
                            case EventNote.MediumB3:
                                _builder.AddNote(Difficulty.Medium, CreateNote(xmkEvent, Frets.Black3));
                                break;
                            case EventNote.MediumW3:
                                _builder.AddNote(Difficulty.Medium, CreateNote(xmkEvent, Frets.White3));
                                break;
                            case EventNote.MediumOpen:
                                _builder.AddNote(Difficulty.Medium, CreateNote(xmkEvent, Frets.Open));
                                break;
                            case EventNote.HardB1:
                                _builder.AddNote(Difficulty.Hard, CreateNote(xmkEvent, Frets.Black1));
                                break;
                            case EventNote.HardW1:
                                _builder.AddNote(Difficulty.Hard, CreateNote(xmkEvent, Frets.White1));
                                break;
                            case EventNote.HardB2:
                                _builder.AddNote(Difficulty.Hard, CreateNote(xmkEvent, Frets.Black2));
                                break;
                            case EventNote.HardW2:
                                _builder.AddNote(Difficulty.Hard, CreateNote(xmkEvent, Frets.White2));
                                break;
                            case EventNote.HardB3:
                                _builder.AddNote(Difficulty.Hard, CreateNote(xmkEvent, Frets.Black3));
                                break;
                            case EventNote.HardW3:
                                _builder.AddNote(Difficulty.Hard, CreateNote(xmkEvent, Frets.White3));
                                break;
                            case EventNote.HardOpen:
                                _builder.AddNote(Difficulty.Hard, CreateNote(xmkEvent, Frets.Open));
                                break;
                            case EventNote.ExpertB1:
                                _builder.AddNote(Difficulty.Expert, CreateNote(xmkEvent, Frets.Black1));
                                break;
                            case EventNote.ExpertW1:
                                _builder.AddNote(Difficulty.Expert, CreateNote(xmkEvent, Frets.White1));
                                break;
                            case EventNote.ExpertB2:
                                _builder.AddNote(Difficulty.Expert, CreateNote(xmkEvent, Frets.Black2));
                                break;
                            case EventNote.ExpertW2:
                                _builder.AddNote(Difficulty.Expert, CreateNote(xmkEvent, Frets.White2));
                                break;
                            case EventNote.ExpertB3:
                                _builder.AddNote(Difficulty.Expert, CreateNote(xmkEvent, Frets.Black3));
                                break;
                            case EventNote.ExpertW3:
                                _builder.AddNote(Difficulty.Expert, CreateNote(xmkEvent, Frets.White3));
                                break;
                            case EventNote.ExpertOpen:
                                _builder.AddNote(Difficulty.Expert, CreateNote(xmkEvent, Frets.Open));
                                break;
                        }
                    }
                    break;
            }
        }

        private Tempo CreateTempo(IXmkTempo xmkTempo) => new Tempo((int)xmkTempo.Ticks, (int)xmkTempo.Tempo);
        private TimeSignature CreateTimeSignature(IXmkTimeSignature xmkTimeSignature) => new TimeSignature((int)xmkTimeSignature.Ticks, (int)xmkTimeSignature.Numerator, (int)xmkTimeSignature.Denominator);
        private Phrase CreatePhrase(IXmkEvent xmkEvent) => new Phrase(xmkEvent.Start, xmkEvent.End);
        private XmkNote CreateNote(IXmkEvent xmkEvent, Frets frets) => new XmkNote(xmkEvent.Start, xmkEvent.End, frets, xmkEvent.Type);

        public Track Build() => _builder.Build();
    }
}

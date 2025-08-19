using ScoreHunter.Core.Enums;
using ScoreHunter.Core.Interfaces;
using ScoreHunter.Drawing.Abstractions.Interfaces;
using ScoreHunter.Drawing.Abstractions.Interfaces.IO;
using ScoreHunter.Drawing.Svg.Extensions;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace ScoreHunter.Drawing.Svg.IO
{
    public class SvgTablatureWriter : ITablatureWriter
    {
        private const double PixelsPerQuarterNote = 60;
        private const double StaffPaddingX = 30;
        private const double StaffPaddingY = 45;
        private const double StaffHeight = 30;
        private const double TextPaddingY = 4;
        private const double TextFontSize = 8;
        private const double NoteSize = 11;
        private const double SustainHeight = 7;

        private readonly XmlWriter _writer;

        private readonly bool _leaveOpen;
        private bool _disposed;

        public SvgTablatureWriter(XmlWriter writer) : this(writer, false)
        {
        }

        public SvgTablatureWriter(XmlWriter writer, bool leaveOpen)
        {
            _writer = writer;
            _leaveOpen = leaveOpen;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                if (!_leaveOpen)
                {
                    _writer.Dispose();
                }
                _disposed = true;
            }
        }

        public void Write(ITablature tablature)
        {
            double TicksToPixels(int ticks)
            {
                return this.TicksToPixels(ticks, tablature.TicksPerQuarterNote);
            }

            _writer.WriteStartElement(null, "svg", "http://www.w3.org/2000/svg");
            _writer.WriteAttributeString(null, "version", null, "1.1");
            _writer.WriteAttributeString(null, "width", null, (StaffPaddingX * 2 + TicksToPixels(tablature.TicksPerStaff)).ToString(CultureInfo.InvariantCulture));
            _writer.WriteAttributeString(null, "height", null, (StaffPaddingY * 2 + ((StaffHeight + StaffPaddingY) * tablature.Staves.Count())).ToString(CultureInfo.InvariantCulture));

            _writer.WriteStartElement("defs");

            _writer.WriteStartElement("circle");
            _writer.WriteAttributeString("id", "n");
            _writer.WriteAttributeDoubleInvariant("r", NoteSize / 2);
            _writer.WriteAttributeString("stroke", "black");
            _writer.WriteAttributeDoubleInvariant("stroke-width", 1);
            _writer.WriteEndElement();

            _writer.WriteStartElement("use");
            _writer.WriteAttributeString("id", "b");
            _writer.WriteAttributeString("href", "#n");
            _writer.WriteAttributeString("fill", "black");
            _writer.WriteEndElement();

            _writer.WriteStartElement("use");
            _writer.WriteAttributeString("id", "w");
            _writer.WriteAttributeString("href", "#n");
            _writer.WriteAttributeString("fill", "white");
            _writer.WriteEndElement();

            _writer.WriteStartElement("rect");
            _writer.WriteAttributeString("id", "o");
            _writer.WriteAttributeDoubleInvariant("width", NoteSize / 2);
            _writer.WriteAttributeDoubleInvariant("height", StaffHeight + NoteSize);
            _writer.WriteAttributeString("stroke", "black");
            _writer.WriteAttributeString("fill", "white");
            _writer.WriteAttributeDoubleInvariant("stroke-width", 1);
            _writer.WriteEndElement();

            _writer.WriteEndElement();

            ITimeSignature currentTimeSignature = null;
            var measureCount = 0;
            var staffY = StaffPaddingY;

            foreach (var staff in tablature.Staves)
            {
                var staffX2 = StaffPaddingX + TicksToPixels(staff.EndTicks - staff.StartTicks);
                var measureCountY = staffY - (NoteSize / 2 + TextPaddingY);
                var tempoY = measureCountY - (TextFontSize + TextPaddingY);

                _writer.WriteStartElement(null, "line", null);
                _writer.WriteAttributeString(null, "x1", null, StaffPaddingX.ToString(CultureInfo.InvariantCulture));
                _writer.WriteAttributeString(null, "x2", null, staffX2.ToString(CultureInfo.InvariantCulture));
                _writer.WriteAttributeString(null, "y1", null, staffY.ToString(CultureInfo.InvariantCulture));
                _writer.WriteAttributeString(null, "y2", null, staffY.ToString(CultureInfo.InvariantCulture));
                _writer.WriteAttributeString(null, "stroke", null, "black");
                _writer.WriteAttributeString(null, "stroke-width", null, "1");
                _writer.WriteEndElement();

                _writer.WriteStartElement(null, "line", null);
                _writer.WriteAttributeString(null, "x1", null, StaffPaddingX.ToString(CultureInfo.InvariantCulture));
                _writer.WriteAttributeString(null, "x2", null, staffX2.ToString(CultureInfo.InvariantCulture));
                _writer.WriteAttributeString(null, "y1", null, (staffY + StaffHeight / 2).ToString(CultureInfo.InvariantCulture));
                _writer.WriteAttributeString(null, "y2", null, (staffY + StaffHeight / 2).ToString(CultureInfo.InvariantCulture));
                _writer.WriteAttributeString(null, "stroke", null, "gray");
                _writer.WriteAttributeString(null, "stroke-width", null, "1");
                _writer.WriteEndElement();

                _writer.WriteStartElement(null, "line", null);
                _writer.WriteAttributeString(null, "x1", null, StaffPaddingX.ToString(CultureInfo.InvariantCulture));
                _writer.WriteAttributeString(null, "x2", null, staffX2.ToString(CultureInfo.InvariantCulture));
                _writer.WriteAttributeString(null, "y1", null, (staffY + StaffHeight).ToString(CultureInfo.InvariantCulture));
                _writer.WriteAttributeString(null, "y2", null, (staffY + StaffHeight).ToString(CultureInfo.InvariantCulture));
                _writer.WriteAttributeString(null, "stroke", null, "black");
                _writer.WriteAttributeString(null, "stroke-width", null, "1");
                _writer.WriteEndElement();

                _writer.WriteStartElement(null, "line", null);
                _writer.WriteAttributeString(null, "x1", null, staffX2.ToString(CultureInfo.InvariantCulture));
                _writer.WriteAttributeString(null, "x2", null, staffX2.ToString(CultureInfo.InvariantCulture));
                _writer.WriteAttributeString(null, "y1", null, staffY.ToString(CultureInfo.InvariantCulture));
                _writer.WriteAttributeString(null, "y2", null, (staffY + StaffHeight).ToString(CultureInfo.InvariantCulture));
                _writer.WriteAttributeString(null, "stroke", null, "black");
                _writer.WriteAttributeString(null, "stroke-width", null, "1");
                _writer.WriteEndElement();


                foreach (var measure in staff.Measures)
                {
                    var measureX = StaffPaddingX + TicksToPixels(measure.StartTicks - staff.StartTicks);

                    _writer.WriteStartElement(null, "line", null);
                    _writer.WriteAttributeString(null, "x1", null, measureX.ToString(CultureInfo.InvariantCulture));
                    _writer.WriteAttributeString(null, "x2", null, measureX.ToString(CultureInfo.InvariantCulture));
                    _writer.WriteAttributeString(null, "y1", null, staffY.ToString(CultureInfo.InvariantCulture));
                    _writer.WriteAttributeString(null, "y2", null, (staffY + StaffHeight).ToString(CultureInfo.InvariantCulture));
                    _writer.WriteAttributeString(null, "stroke", null, "black");
                    _writer.WriteAttributeString(null, "stroke-width", null, "1");
                    _writer.WriteEndElement();

                    if (measure.TimeSignature != currentTimeSignature)
                    {
                        currentTimeSignature = measure.TimeSignature;
                        var timeSignatureX = measureX + TicksToPixels(currentTimeSignature.Ticks - measure.StartTicks) + TextFontSize;

                        _writer.WriteStartElement(null, "text", null);
                        _writer.WriteAttributeString(null, "x", null, timeSignatureX.ToString(CultureInfo.InvariantCulture));
                        _writer.WriteAttributeString(null, "y", null, (staffY + StaffHeight / 2 - TextPaddingY / 2).ToString(CultureInfo.InvariantCulture));
                        _writer.WriteAttributeString(null, "font-family", null, "sans-serif");
                        _writer.WriteAttributeString(null, "font-size", null, (StaffHeight / 2).ToString(CultureInfo.InvariantCulture));
                        _writer.WriteAttributeString(null, "fill", null, "gray");

                        _writer.WriteString(currentTimeSignature.Numerator.ToString());

                        _writer.WriteEndElement();

                        _writer.WriteStartElement(null, "text", null);
                        _writer.WriteAttributeString(null, "x", null, timeSignatureX.ToString(CultureInfo.InvariantCulture));
                        _writer.WriteAttributeString(null, "y", null, (staffY + StaffHeight - TextPaddingY / 2).ToString(CultureInfo.InvariantCulture));
                        _writer.WriteAttributeString(null, "font-family", null, "sans-serif");
                        _writer.WriteAttributeString(null, "font-size", null, (StaffHeight / 2).ToString(CultureInfo.InvariantCulture));
                        _writer.WriteAttributeString(null, "fill", null, "gray");

                        _writer.WriteString(currentTimeSignature.Denominator.ToString());

                        _writer.WriteEndElement();
                    }

                    foreach (var beat in measure.Beats)
                    {
                        var beatX = measureX + TicksToPixels(beat.Ticks - measure.StartTicks);

                        _writer.WriteStartElement(null, "line", null);
                        _writer.WriteAttributeString(null, "x1", null, beatX.ToString(CultureInfo.InvariantCulture));
                        _writer.WriteAttributeString(null, "x2", null, beatX.ToString(CultureInfo.InvariantCulture));
                        _writer.WriteAttributeString(null, "y1", null, staffY.ToString(CultureInfo.InvariantCulture));
                        _writer.WriteAttributeString(null, "y2", null, (staffY + StaffHeight).ToString(CultureInfo.InvariantCulture));
                        _writer.WriteAttributeString(null, "stroke", null, "gray");
                        _writer.WriteAttributeString(null, "stroke-width", null, "1");
                        _writer.WriteEndElement();
                    }
                }

                foreach (var sustain in staff.Sustains)
                {
                    var sustainX = StaffPaddingX + TicksToPixels(sustain.StartTicks - staff.StartTicks);
                    var sustainWidth = TicksToPixels(sustain.EndTicks - sustain.StartTicks);

                    void WriteSustain(double y)
                    {
                        _writer.WriteStartElementRect(sustainX, y, sustainWidth, SustainHeight);
                        _writer.WriteAttributeString("fill", "gray");
                        _writer.WriteEndElement();
                    }

                    switch (sustain.Frets.Flags)
                    {
                        case FretFlags.Black1:
                        case FretFlags.White1:
                        case FretFlags.Black1 | FretFlags.White1:
                            WriteSustain(staffY - SustainHeight / 2);
                            break;
                        case FretFlags.Open:
                        case FretFlags.Black2:
                        case FretFlags.White2:
                        case FretFlags.Black2 | FretFlags.White2:
                            WriteSustain(staffY + StaffHeight / 2 - SustainHeight / 2);
                            break;
                        case FretFlags.Black3:
                        case FretFlags.White3:
                        case FretFlags.Black3 | FretFlags.White3:
                            WriteSustain(staffY + StaffHeight - SustainHeight / 2);
                            break;
                    }
                }

                foreach (var measure in staff.Measures)
                {
                    measureCount++;
                    var measureX = StaffPaddingX + TicksToPixels(measure.StartTicks - staff.StartTicks);

                    _writer.WriteStartElement(null, "text", null);
                    _writer.WriteAttributeString(null, "x", null, measureX.ToString(CultureInfo.InvariantCulture));
                    _writer.WriteAttributeString(null, "y", null, measureCountY.ToString(CultureInfo.InvariantCulture));
                    _writer.WriteAttributeString(null, "font-family", null, "sans-serif");
                    _writer.WriteAttributeString(null, "font-size", null, TextFontSize.ToString(CultureInfo.InvariantCulture));
                    _writer.WriteAttributeString(null, "fill", null, "red");

                    _writer.WriteString(measureCount.ToString());

                    _writer.WriteEndElement();

                    foreach (var tempo in measure.Tempos)
                    {
                        var tempoX = measureX + TicksToPixels(tempo.Ticks - measure.StartTicks);

                        _writer.WriteStartElement(null, "text", null);
                        _writer.WriteAttributeString(null, "x", null, tempoX.ToString(CultureInfo.InvariantCulture));
                        _writer.WriteAttributeString(null, "y", null, tempoY.ToString(CultureInfo.InvariantCulture));
                        _writer.WriteAttributeString(null, "font-family", null, "sans-serif");
                        _writer.WriteAttributeString(null, "font-size", null, TextFontSize.ToString(CultureInfo.InvariantCulture));
                        _writer.WriteAttributeString(null, "fill", null, "gray");

                        _writer.WriteString("\u2669=" + Math.Round(tempo.BeatsPerMinute));

                        _writer.WriteEndElement();
                    }

                    foreach (var note in measure.Notes)
                    {
                        var noteX = measureX + TicksToPixels(note.Ticks - measure.StartTicks);

                        switch (note.Frets.Flags)
                        {
                            case FretFlags.Open:
                                _writer.WriteStartElement("use");
                                _writer.WriteAttributeDoubleInvariant("x", noteX - NoteSize / 4);
                                _writer.WriteAttributeDoubleInvariant("y", staffY - NoteSize / 2);
                                _writer.WriteAttributeString("href", "#o");
                                _writer.WriteEndElement();
                                break;
                            case FretFlags.Black1:
                                _writer.WriteStartElement("use");
                                _writer.WriteAttributeDoubleInvariant("x", noteX);
                                _writer.WriteAttributeDoubleInvariant("y", staffY);
                                _writer.WriteAttributeString("href", "#b");
                                _writer.WriteEndElement();
                                break;
                            case FretFlags.Black2:
                                _writer.WriteStartElement("use");
                                _writer.WriteAttributeDoubleInvariant("x", noteX);
                                _writer.WriteAttributeDoubleInvariant("y", staffY + StaffHeight / 2);
                                _writer.WriteAttributeString("href", "#b");
                                _writer.WriteEndElement();
                                break;
                            case FretFlags.Black3:
                                _writer.WriteStartElement("use");
                                _writer.WriteAttributeDoubleInvariant("x", noteX);
                                _writer.WriteAttributeDoubleInvariant("y", staffY + StaffHeight);
                                _writer.WriteAttributeString("href", "#b");
                                _writer.WriteEndElement();
                                break;
                            case FretFlags.White1:
                                _writer.WriteStartElement("use");
                                _writer.WriteAttributeDoubleInvariant("x", noteX);
                                _writer.WriteAttributeDoubleInvariant("y", staffY);
                                _writer.WriteAttributeString("href", "#w");
                                _writer.WriteEndElement();
                                break;
                            case FretFlags.White2:
                                _writer.WriteStartElement("use");
                                _writer.WriteAttributeDoubleInvariant("x", noteX);
                                _writer.WriteAttributeDoubleInvariant("y", staffY + StaffHeight / 2);
                                _writer.WriteAttributeString("href", "#w");
                                _writer.WriteEndElement();
                                break;
                            case FretFlags.White3:
                                _writer.WriteStartElement("use");
                                _writer.WriteAttributeDoubleInvariant("x", noteX);
                                _writer.WriteAttributeDoubleInvariant("y", staffY + StaffHeight);
                                _writer.WriteAttributeString("href", "#w");
                                _writer.WriteEndElement();
                                break;
                            case FretFlags.Black1 | FretFlags.White1:
                                _writer.WriteStartElement(null, "rect", null);
                                _writer.WriteAttributeString(null, "x", null, (noteX - NoteSize / 2).ToString(CultureInfo.InvariantCulture));
                                _writer.WriteAttributeString(null, "y", null, (staffY - NoteSize / 2).ToString(CultureInfo.InvariantCulture));
                                _writer.WriteAttributeString(null, "width", null, NoteSize.ToString(CultureInfo.InvariantCulture));
                                _writer.WriteAttributeString(null, "height", null, NoteSize.ToString(CultureInfo.InvariantCulture));
                                _writer.WriteAttributeString(null, "stroke", null, "black");
                                _writer.WriteAttributeString(null, "fill", null, "black");
                                _writer.WriteAttributeString(null, "stroke-width", null, "1");
                                _writer.WriteEndElement();

                                _writer.WriteStartElement(null, "rect", null);
                                _writer.WriteAttributeString(null, "x", null, (noteX - NoteSize / 2).ToString(CultureInfo.InvariantCulture));
                                _writer.WriteAttributeString(null, "y", null, staffY.ToString(CultureInfo.InvariantCulture));
                                _writer.WriteAttributeString(null, "width", null, NoteSize.ToString(CultureInfo.InvariantCulture));
                                _writer.WriteAttributeString(null, "height", null, (NoteSize / 2).ToString(CultureInfo.InvariantCulture));
                                _writer.WriteAttributeString(null, "stroke", null, "black");
                                _writer.WriteAttributeString(null, "fill", null, "white");
                                _writer.WriteAttributeString(null, "stroke-width", null, "1");
                                _writer.WriteEndElement();
                                break;
                            case FretFlags.Black2 | FretFlags.White2:
                                _writer.WriteStartElement(null, "rect", null);
                                _writer.WriteAttributeString(null, "x", null, (noteX - NoteSize / 2).ToString(CultureInfo.InvariantCulture));
                                _writer.WriteAttributeString(null, "y", null, (staffY + StaffHeight / 2 - NoteSize / 2).ToString(CultureInfo.InvariantCulture));
                                _writer.WriteAttributeString(null, "width", null, NoteSize.ToString(CultureInfo.InvariantCulture));
                                _writer.WriteAttributeString(null, "height", null, NoteSize.ToString(CultureInfo.InvariantCulture));
                                _writer.WriteAttributeString(null, "stroke", null, "black");
                                _writer.WriteAttributeString(null, "fill", null, "black");
                                _writer.WriteAttributeString(null, "stroke-width", null, "1");
                                _writer.WriteEndElement();

                                _writer.WriteStartElement(null, "rect", null);
                                _writer.WriteAttributeString(null, "x", null, (noteX - NoteSize / 2).ToString(CultureInfo.InvariantCulture));
                                _writer.WriteAttributeString(null, "y", null, (staffY + StaffHeight / 2).ToString(CultureInfo.InvariantCulture));
                                _writer.WriteAttributeString(null, "width", null, NoteSize.ToString(CultureInfo.InvariantCulture));
                                _writer.WriteAttributeString(null, "height", null, (NoteSize / 2).ToString(CultureInfo.InvariantCulture));
                                _writer.WriteAttributeString(null, "stroke", null, "black");
                                _writer.WriteAttributeString(null, "fill", null, "white");
                                _writer.WriteAttributeString(null, "stroke-width", null, "1");
                                _writer.WriteEndElement();
                                break;
                            case FretFlags.Black3 | FretFlags.White3:
                                _writer.WriteStartElement(null, "rect", null);
                                _writer.WriteAttributeString(null, "x", null, (noteX - NoteSize / 2).ToString(CultureInfo.InvariantCulture));
                                _writer.WriteAttributeString(null, "y", null, (staffY + StaffHeight - NoteSize / 2).ToString(CultureInfo.InvariantCulture));
                                _writer.WriteAttributeString(null, "width", null, NoteSize.ToString(CultureInfo.InvariantCulture));
                                _writer.WriteAttributeString(null, "height", null, NoteSize.ToString(CultureInfo.InvariantCulture));
                                _writer.WriteAttributeString(null, "stroke", null, "black");
                                _writer.WriteAttributeString(null, "fill", null, "black");
                                _writer.WriteAttributeString(null, "stroke-width", null, "1");
                                _writer.WriteEndElement();

                                _writer.WriteStartElement(null, "rect", null);
                                _writer.WriteAttributeString(null, "x", null, (noteX - NoteSize / 2).ToString(CultureInfo.InvariantCulture));
                                _writer.WriteAttributeString(null, "y", null, (staffY + StaffHeight).ToString(CultureInfo.InvariantCulture));
                                _writer.WriteAttributeString(null, "width", null, NoteSize.ToString(CultureInfo.InvariantCulture));
                                _writer.WriteAttributeString(null, "height", null, (NoteSize / 2).ToString(CultureInfo.InvariantCulture));
                                _writer.WriteAttributeString(null, "stroke", null, "black");
                                _writer.WriteAttributeString(null, "fill", null, "white");
                                _writer.WriteAttributeString(null, "stroke-width", null, "1");
                                _writer.WriteEndElement();
                                break;
                        }
                    }
                }

                foreach (var heroPowerPhrase in staff.HeroPowerPhrases)
                {
                    var heroPowerPhraseX = StaffPaddingX + TicksToPixels(heroPowerPhrase.StartTicks - staff.StartTicks);
                    var heroPowerPhraseWidth = TicksToPixels(heroPowerPhrase.EndTicks - heroPowerPhrase.StartTicks);

                    _writer.WriteStartElement(null, "rect", null);
                    _writer.WriteAttributeString(null, "x", null, heroPowerPhraseX.ToString(CultureInfo.InvariantCulture));
                    _writer.WriteAttributeString(null, "y", null, staffY.ToString(CultureInfo.InvariantCulture));
                    _writer.WriteAttributeString(null, "width", null, heroPowerPhraseWidth.ToString(CultureInfo.InvariantCulture));
                    _writer.WriteAttributeString(null, "height", null, StaffHeight.ToString(CultureInfo.InvariantCulture));
                    _writer.WriteAttributeString(null, "fill", null, "green");
                    _writer.WriteAttributeString(null, "opacity", null, "0.3");
                    _writer.WriteEndElement();
                }

                foreach (var highwayPhrase in staff.HighwayPhrases)
                {
                    var highwayPhraseX = StaffPaddingX + TicksToPixels(highwayPhrase.StartTicks - staff.StartTicks);
                    var highwayPhraseWidth = TicksToPixels(highwayPhrase.EndTicks - highwayPhrase.StartTicks);

                    _writer.WriteStartElement(null, "rect", null);
                    _writer.WriteAttributeString(null, "x", null, highwayPhraseX.ToString(CultureInfo.InvariantCulture));
                    _writer.WriteAttributeString(null, "y", null, staffY.ToString(CultureInfo.InvariantCulture));
                    _writer.WriteAttributeString(null, "width", null, highwayPhraseWidth.ToString(CultureInfo.InvariantCulture));
                    _writer.WriteAttributeString(null, "height", null, StaffHeight.ToString(CultureInfo.InvariantCulture));
                    _writer.WriteAttributeString(null, "fill", null, "yellow");
                    _writer.WriteAttributeString(null, "opacity", null, "0.3");
                    _writer.WriteEndElement();
                }

                foreach (var activation in staff.Activations)
                {
                    var activationX = StaffPaddingX + TicksToPixels(activation.StartTicks - staff.StartTicks);
                    var activationWidth = TicksToPixels(activation.EndTicks - activation.StartTicks);

                    _writer.WriteStartElement(null, "rect", null);
                    _writer.WriteAttributeString(null, "x", null, activationX.ToString(CultureInfo.InvariantCulture));
                    _writer.WriteAttributeString(null, "y", null, staffY.ToString(CultureInfo.InvariantCulture));
                    _writer.WriteAttributeString(null, "width", null, activationWidth.ToString(CultureInfo.InvariantCulture));
                    _writer.WriteAttributeString(null, "height", null, StaffHeight.ToString(CultureInfo.InvariantCulture));
                    _writer.WriteAttributeString(null, "fill", null, "blue");
                    _writer.WriteAttributeString(null, "opacity", null, "0.3");
                    _writer.WriteEndElement();
                }

                staffY += StaffHeight + StaffPaddingY;
            }

            _writer.WriteEndElement();
        }

        public Task WriteAsync(ITablature tablature, CancellationToken cancellationToken = default)
        {
            Write(tablature);
            return Task.CompletedTask;
        }

        private double TicksToPixels(int ticks, int ticksPerQuarterNote)
        {
            return (double)ticks / ticksPerQuarterNote * PixelsPerQuarterNote;
        }
    }
}

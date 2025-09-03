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
        private bool _inPathCommand;

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

            WriteStyleElement();
            WriteDefsElement();

            ITimeSignature currentTimeSignature = null;
            var measureCount = 0;
            var staffY = StaffPaddingY;

            foreach (var staff in tablature.Staves)
            {
                var staffWidth = TicksToPixels(staff.EndTicks - staff.StartTicks);
                var measureCountY = staffY - (NoteSize / 2 + TextPaddingY);
                var tempoY = measureCountY - (TextFontSize + TextPaddingY);

                WriteStaff(staffY, staffWidth);

                foreach (var measure in staff.Measures)
                {
                    var measureX = StaffPaddingX + TicksToPixels(measure.StartTicks - staff.StartTicks);

                    _writer.WriteStartElementUse("#s", measureX, staffY);
                    _writer.WriteEndElement();

                    if (measure.TimeSignature != currentTimeSignature)
                    {
                        currentTimeSignature = measure.TimeSignature;
                        var timeSignatureX = measureX + TicksToPixels(currentTimeSignature.Ticks - measure.StartTicks) + TextFontSize;
                        WriteTimeSignature(timeSignatureX, staffY, currentTimeSignature);
                    }

                    foreach (var beat in measure.Beats)
                    {
                        var beatX = measureX + TicksToPixels(beat.Ticks - measure.StartTicks);

                        _writer.WriteStartElementUse("#l", beatX, staffY);
                        _writer.WriteEndElement();
                    }
                }

                foreach (var sustain in staff.Sustains)
                {
                    var sustainX = StaffPaddingX + TicksToPixels(sustain.StartTicks - staff.StartTicks);
                    var sustainWidth = TicksToPixels(sustain.EndTicks - sustain.StartTicks);

                    switch (sustain.Frets.Flags)
                    {
                        case FretFlags.Black1:
                        case FretFlags.White1:
                        case FretFlags.Black1 | FretFlags.White1:
                            WriteSustain(sustainX, staffY - SustainHeight / 2, sustainWidth);
                            break;
                        case FretFlags.Open:
                        case FretFlags.Black2:
                        case FretFlags.White2:
                        case FretFlags.Black2 | FretFlags.White2:
                            WriteSustain(sustainX, staffY + StaffHeight / 2 - SustainHeight / 2, sustainWidth);
                            break;
                        case FretFlags.Black3:
                        case FretFlags.White3:
                        case FretFlags.Black3 | FretFlags.White3:
                            WriteSustain(sustainX, staffY + StaffHeight - SustainHeight / 2, sustainWidth);
                            break;
                    }
                }

                foreach (var measure in staff.Measures)
                {
                    measureCount++;
                    var measureX = StaffPaddingX + TicksToPixels(measure.StartTicks - staff.StartTicks);
                    WriteMeasureCount(measureX, measureCountY, measureCount);

                    foreach (var tempo in measure.Tempos)
                    {
                        var tempoX = measureX + TicksToPixels(tempo.Ticks - measure.StartTicks);
                        WriteTempo(tempoX, tempoY, tempo);
                    }

                    foreach (var note in measure.Notes)
                    {
                        var noteX = measureX + TicksToPixels(note.Ticks - measure.StartTicks);

                        switch (note.Frets.Flags)
                        {
                            case FretFlags.Open:
                                _writer.WriteStartElementUse("#o", noteX - NoteSize / 4, staffY - NoteSize / 2);
                                _writer.WriteEndElement();
                                break;
                            case FretFlags.Black1:
                                _writer.WriteStartElementUse("#b", noteX, staffY);
                                _writer.WriteEndElement();
                                break;
                            case FretFlags.Black2:
                                _writer.WriteStartElementUse("#b", noteX, staffY + StaffHeight / 2);
                                _writer.WriteEndElement();
                                break;
                            case FretFlags.Black3:
                                _writer.WriteStartElementUse("#b", noteX, staffY + StaffHeight);
                                _writer.WriteEndElement();
                                break;
                            case FretFlags.White1:
                                _writer.WriteStartElementUse("#w", noteX, staffY);
                                _writer.WriteEndElement();
                                break;
                            case FretFlags.White2:
                                _writer.WriteStartElementUse("#w", noteX, staffY + StaffHeight / 2);
                                _writer.WriteEndElement();
                                break;
                            case FretFlags.White3:
                                _writer.WriteStartElementUse("#w", noteX, staffY + StaffHeight);
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
                    WritePhrase(heroPowerPhraseX, staffY, heroPowerPhraseWidth, "g");
                }

                foreach (var highwayPhrase in staff.HighwayPhrases)
                {
                    var highwayPhraseX = StaffPaddingX + TicksToPixels(highwayPhrase.StartTicks - staff.StartTicks);
                    var highwayPhraseWidth = TicksToPixels(highwayPhrase.EndTicks - highwayPhrase.StartTicks);
                    WritePhrase(highwayPhraseX, staffY, highwayPhraseWidth, "y");
                }

                foreach (var activation in staff.Activations)
                {
                    var activationX = StaffPaddingX + TicksToPixels(activation.StartTicks - staff.StartTicks);
                    var activationWidth = TicksToPixels(activation.EndTicks - activation.StartTicks);
                    WritePhrase(activationX, staffY, activationWidth, "b");
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

        private void WriteStyleElement()
        {
            _writer.WriteStartElement("style");
            _writer.WriteString("text { font-family: sans-serif; }");
            _writer.WriteString(".s, .w { fill: transparent; stroke-width: 1px; }");
            _writer.WriteString(".s { stroke: black; }");
            _writer.WriteString(".w { stroke: gray; }");
            _writer.WriteString(".m, .t { font-size: ");
            _writer.WriteValue(TextFontSize);
            _writer.WriteString("px; }");
            _writer.WriteString(".n { font-size: ");
            _writer.WriteValue(StaffHeight / 2);
            _writer.WriteString("px; }");
            _writer.WriteString(".m { fill: red; }");
            _writer.WriteString(".t, .n, .l { fill: gray; }");
            _writer.WriteString(".g, .y, .b { opacity: 0.3; }");
            _writer.WriteString(".g { fill: green; }");
            _writer.WriteString(".y { fill: yellow; }");
            _writer.WriteString(".b { fill: blue; }");
            _writer.WriteEndElement();
        }

        private void WriteDefsElement()
        {
            _writer.WriteStartElement("defs");

            _writer.WriteStartElement("path");
            _writer.WriteAttributeString("id", "h");
            WriteStartAttributePathCommand();
            WriteMoveTo(0, 0);
            WriteVerticalLineToRelative(StaffHeight);
            WriteEndAttributePathCommand();
            _writer.WriteAttributeDouble("stroke-width", 1);
            _writer.WriteEndElement();

            _writer.WriteStartElementUse("#h");
            _writer.WriteAttributeString("id", "s");
            _writer.WriteAttributeString("stroke", "black");
            _writer.WriteEndElement();

            _writer.WriteStartElementUse("#h");
            _writer.WriteAttributeString("id", "l");
            _writer.WriteAttributeString("stroke", "gray");
            _writer.WriteEndElement();

            _writer.WriteStartElement("circle");
            _writer.WriteAttributeString("id", "n");
            _writer.WriteAttributeDouble("r", NoteSize / 2);
            _writer.WriteAttributeString("stroke", "black");
            _writer.WriteAttributeDouble("stroke-width", 1);
            _writer.WriteEndElement();

            _writer.WriteStartElementUse("#n");
            _writer.WriteAttributeString("id", "b");
            _writer.WriteAttributeString("fill", "black");
            _writer.WriteEndElement();

            _writer.WriteStartElementUse("#n");
            _writer.WriteAttributeString("id", "w");
            _writer.WriteAttributeString("fill", "white");
            _writer.WriteEndElement();

            _writer.WriteStartElement("rect");
            _writer.WriteAttributeString("id", "o");
            _writer.WriteAttributeDouble("width", NoteSize / 2);
            _writer.WriteAttributeDouble("height", StaffHeight + NoteSize);
            _writer.WriteAttributeString("stroke", "black");
            _writer.WriteAttributeString("fill", "white");
            _writer.WriteAttributeDouble("stroke-width", 1);
            _writer.WriteEndElement();

            _writer.WriteEndElement();
        }

        private void WriteStaff(double y, double width)
        {
            _writer.WriteStartElement("path");
            WriteStartAttributePathCommand();
            WriteMoveTo(StaffPaddingX, y);
            WriteHorizontalLineToRelative(width);
            WriteVerticalLineToRelative(StaffHeight);
            WriteHorizontalLineToRelative(-width);
            WriteEndAttributePathCommand();
            _writer.WriteAttributeString("class", "s");
            _writer.WriteEndElement();

            _writer.WriteStartElement("path");
            WriteStartAttributePathCommand();
            WriteMoveTo(StaffPaddingX, y + StaffHeight / 2);
            WriteHorizontalLineToRelative(width);
            WriteEndAttributePathCommand();
            _writer.WriteAttributeString("class", "w");
            _writer.WriteEndElement();
        }

        private void WriteTimeSignature(double x, double y, ITimeSignature timeSignature)
        {
            _writer.WriteStartElement("text");
            _writer.WriteAttributeDouble("x", x);
            _writer.WriteAttributeDouble("y", y + StaffHeight / 2 - TextPaddingY / 2);
            _writer.WriteAttributeString("class", "n");

            _writer.WriteValue(timeSignature.Numerator);

            _writer.WriteEndElement();

            _writer.WriteStartElement("text");
            _writer.WriteAttributeDouble("x", x);
            _writer.WriteAttributeDouble("y", y + StaffHeight - TextPaddingY / 2);
            _writer.WriteAttributeString("class", "n");

            _writer.WriteValue(timeSignature.Denominator);

            _writer.WriteEndElement();
        }

        private void WriteSustain(double x, double y, double width)
        {
            _writer.WriteStartElement("path");
            WriteStartAttributePathCommand();
            WriteMoveTo(x, y);
            WriteHorizontalLineToRelative(width);
            WriteVerticalLineToRelative(SustainHeight);
            WriteHorizontalLineToRelative(-width);
            WriteEndAttributePathCommand();
            _writer.WriteAttributeString("class", "l");
            _writer.WriteEndElement();
        }

        private void WriteMeasureCount(double x, double y, int measureCount)
        {
            _writer.WriteStartElement("text");
            _writer.WriteAttributeDouble("x", x);
            _writer.WriteAttributeDouble("y", y);
            _writer.WriteAttributeString("class", "m");

            _writer.WriteValue(measureCount);

            _writer.WriteEndElement();
        }

        private void WriteTempo(double x, double y, ITempo tempo)
        {
            _writer.WriteStartElement("text");
            _writer.WriteAttributeDouble("x", x);
            _writer.WriteAttributeDouble("y", y);
            _writer.WriteAttributeString("class", "t");

            _writer.WriteString("\u2669=");
            _writer.WriteValue(Math.Round(tempo.BeatsPerMinute));

            _writer.WriteEndElement();
        }

        private void WritePhrase(double x, double y, double width, string @class)
        {
            _writer.WriteStartElement("path");
            WriteStartAttributePathCommand();
            WriteMoveTo(x, y);
            WriteHorizontalLineToRelative(width);
            WriteVerticalLineToRelative(StaffHeight);
            WriteHorizontalLineToRelative(-width);
            WriteClosePath();
            WriteEndAttributePathCommand();
            _writer.WriteAttributeString("class", @class);
            _writer.WriteEndElement();
        }

        private void WriteStartAttributePathCommand()
        {
            _writer.WriteStartAttribute("d");
        }

        private void WritePathCommand(string command)
        {
            if (_inPathCommand)
            {
                _writer.WriteString(" ");
            }
            _writer.WriteString(command);
            _inPathCommand = true;
        }

        private void WritePathCommandParameter(double value)
        {
            _writer.WriteString(" ");
            _writer.WriteValue(value);
        }

        private void WriteMoveTo(double x, double y)
        {
            WritePathCommand("M");
            WritePathCommandParameter(x);
            WritePathCommandParameter(y);
        }

        private void WriteHorizontalLineToRelative(double dx)
        {
            WritePathCommand("h");
            WritePathCommandParameter(dx);
        }

        private void WriteVerticalLineToRelative(double dy)
        {
            WritePathCommand("v");
            WritePathCommandParameter(dy);
        }

        private void WriteClosePath()
        {
            WritePathCommand("Z");
        }

        private void WriteEndAttributePathCommand()
        {
            _writer.WriteEndAttribute();
            _inPathCommand = false;
        }

        private double TicksToPixels(int ticks, int ticksPerQuarterNote)
        {
            return (double)ticks / ticksPerQuarterNote * PixelsPerQuarterNote;
        }
    }
}

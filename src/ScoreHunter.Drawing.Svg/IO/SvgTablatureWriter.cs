using ScoreHunter.Core.Enums;
using ScoreHunter.Drawing.Abstractions.Interfaces;
using ScoreHunter.Drawing.Abstractions.Interfaces.IO;
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
        private const double NoteSize = 11;

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

            var measureCount = 0;
            var staffY = StaffPaddingY;

            foreach (var staff in tablature.Staves)
            {
                var staffX2 = StaffPaddingX + TicksToPixels(staff.EndTicks - staff.StartTicks);

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
                    measureCount++;
                    var measureX = StaffPaddingX + TicksToPixels(measure.StartTicks - staff.StartTicks);

                    _writer.WriteStartElement(null, "text", null);
                    _writer.WriteAttributeString(null, "x", null, measureX.ToString(CultureInfo.InvariantCulture));
                    _writer.WriteAttributeString(null, "y", null, (staffY - 8).ToString(CultureInfo.InvariantCulture));
                    _writer.WriteAttributeString(null, "font-family", null, "sans-serif");
                    _writer.WriteAttributeString(null, "font-size", null, "8");
                    _writer.WriteAttributeString(null, "fill", null, "red");

                    _writer.WriteString(measureCount.ToString());

                    _writer.WriteEndElement();

                    _writer.WriteStartElement(null, "line", null);
                    _writer.WriteAttributeString(null, "x1", null, measureX.ToString(CultureInfo.InvariantCulture));
                    _writer.WriteAttributeString(null, "x2", null, measureX.ToString(CultureInfo.InvariantCulture));
                    _writer.WriteAttributeString(null, "y1", null, staffY.ToString(CultureInfo.InvariantCulture));
                    _writer.WriteAttributeString(null, "y2", null, (staffY + StaffHeight).ToString(CultureInfo.InvariantCulture));
                    _writer.WriteAttributeString(null, "stroke", null, "black");
                    _writer.WriteAttributeString(null, "stroke-width", null, "1");
                    _writer.WriteEndElement();

                    foreach (var note in measure.Notes)
                    {
                        var noteX = measureX + TicksToPixels(note.Ticks - measure.StartTicks);

                        switch (note.Frets.Flags)
                        {
                            case FretFlags.Open:
                                _writer.WriteStartElement(null, "rect", null);
                                _writer.WriteAttributeString(null, "x", null, (noteX - NoteSize / 4).ToString(CultureInfo.InvariantCulture));
                                _writer.WriteAttributeString(null, "y", null, (staffY - NoteSize / 2).ToString(CultureInfo.InvariantCulture));
                                _writer.WriteAttributeString(null, "width", null, (NoteSize / 2).ToString(CultureInfo.InvariantCulture));
                                _writer.WriteAttributeString(null, "height", null, (StaffHeight + NoteSize).ToString(CultureInfo.InvariantCulture));
                                _writer.WriteAttributeString(null, "stroke", null, "black");
                                _writer.WriteAttributeString(null, "fill", null, "white");
                                _writer.WriteAttributeString(null, "stroke-width", null, "1");
                                _writer.WriteEndElement();
                                break;
                            case FretFlags.Black1:
                                _writer.WriteStartElement(null, "circle", null);
                                _writer.WriteAttributeString(null, "cx", null, noteX.ToString(CultureInfo.InvariantCulture));
                                _writer.WriteAttributeString(null, "cy", null, staffY.ToString(CultureInfo.InvariantCulture));
                                _writer.WriteAttributeString(null, "r", null, (NoteSize / 2).ToString(CultureInfo.InvariantCulture));
                                _writer.WriteAttributeString(null, "stroke", null, "black");
                                _writer.WriteAttributeString(null, "fill", null, "black");
                                _writer.WriteAttributeString(null, "stroke-width", null, "1");
                                _writer.WriteEndElement();
                                break;
                            case FretFlags.Black2:
                                _writer.WriteStartElement(null, "circle", null);
                                _writer.WriteAttributeString(null, "cx", null, noteX.ToString(CultureInfo.InvariantCulture));
                                _writer.WriteAttributeString(null, "cy", null, (staffY + StaffHeight / 2).ToString(CultureInfo.InvariantCulture));
                                _writer.WriteAttributeString(null, "r", null, (NoteSize / 2).ToString(CultureInfo.InvariantCulture));
                                _writer.WriteAttributeString(null, "stroke", null, "black");
                                _writer.WriteAttributeString(null, "fill", null, "black");
                                _writer.WriteAttributeString(null, "stroke-width", null, "1");
                                _writer.WriteEndElement();
                                break;
                            case FretFlags.Black3:
                                _writer.WriteStartElement(null, "circle", null);
                                _writer.WriteAttributeString(null, "cx", null, noteX.ToString(CultureInfo.InvariantCulture));
                                _writer.WriteAttributeString(null, "cy", null, (staffY + StaffHeight).ToString(CultureInfo.InvariantCulture));
                                _writer.WriteAttributeString(null, "r", null, (NoteSize / 2).ToString(CultureInfo.InvariantCulture));
                                _writer.WriteAttributeString(null, "stroke", null, "black");
                                _writer.WriteAttributeString(null, "fill", null, "black");
                                _writer.WriteAttributeString(null, "stroke-width", null, "1");
                                _writer.WriteEndElement();
                                break;
                            case FretFlags.White1:
                                _writer.WriteStartElement(null, "circle", null);
                                _writer.WriteAttributeString(null, "cx", null, noteX.ToString(CultureInfo.InvariantCulture));
                                _writer.WriteAttributeString(null, "cy", null, staffY.ToString(CultureInfo.InvariantCulture));
                                _writer.WriteAttributeString(null, "r", null, (NoteSize / 2).ToString(CultureInfo.InvariantCulture));
                                _writer.WriteAttributeString(null, "stroke", null, "black");
                                _writer.WriteAttributeString(null, "fill", null, "white");
                                _writer.WriteAttributeString(null, "stroke-width", null, "1");
                                _writer.WriteEndElement();
                                break;
                            case FretFlags.White2:
                                _writer.WriteStartElement(null, "circle", null);
                                _writer.WriteAttributeString(null, "cx", null, noteX.ToString(CultureInfo.InvariantCulture));
                                _writer.WriteAttributeString(null, "cy", null, (staffY + StaffHeight / 2).ToString(CultureInfo.InvariantCulture));
                                _writer.WriteAttributeString(null, "r", null, (NoteSize / 2).ToString(CultureInfo.InvariantCulture));
                                _writer.WriteAttributeString(null, "stroke", null, "black");
                                _writer.WriteAttributeString(null, "fill", null, "white");
                                _writer.WriteAttributeString(null, "stroke-width", null, "1");
                                _writer.WriteEndElement();
                                break;
                            case FretFlags.White3:
                                _writer.WriteStartElement(null, "circle", null);
                                _writer.WriteAttributeString(null, "cx", null, noteX.ToString(CultureInfo.InvariantCulture));
                                _writer.WriteAttributeString(null, "cy", null, (staffY + StaffHeight).ToString(CultureInfo.InvariantCulture));
                                _writer.WriteAttributeString(null, "r", null, (NoteSize / 2).ToString(CultureInfo.InvariantCulture));
                                _writer.WriteAttributeString(null, "stroke", null, "black");
                                _writer.WriteAttributeString(null, "fill", null, "black");
                                _writer.WriteAttributeString(null, "stroke-width", null, "1");
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

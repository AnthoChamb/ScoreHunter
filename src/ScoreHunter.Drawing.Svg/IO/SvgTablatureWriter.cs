using ScoreHunter.Core.Enums;
using ScoreHunter.Core.Interfaces;
using ScoreHunter.Drawing.Abstractions.Interfaces;
using ScoreHunter.Drawing.Abstractions.Interfaces.IO;
using ScoreHunter.Drawing.Svg.Extensions;
using ScoreHunter.Drawing.Svg.Xml;
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
        private readonly SvgPathWriter _svgPathWriter;

        private readonly bool _leaveOpen;
        private bool _disposed;

        public SvgTablatureWriter(XmlWriter writer) : this(writer, false)
        {
        }

        public SvgTablatureWriter(XmlWriter writer, bool leaveOpen)
        {
            _writer = writer;
            _svgPathWriter = new XmlSvgPathWriter(writer);
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
                _svgPathWriter.Dispose();
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

        public async Task WriteAsync(ITablature tablature, CancellationToken cancellationToken = default)
        {
            double TicksToPixels(int ticks)
            {
                return this.TicksToPixels(ticks, tablature.TicksPerQuarterNote);
            }

            await _writer.WriteStartElementAsync(null, "svg", "http://www.w3.org/2000/svg").ConfigureAwait(false);
            await _writer.WriteAttributeStringAsync(null, "version", null, "1.1").ConfigureAwait(false);
            await _writer.WriteAttributeStringAsync(null, "width", null, (StaffPaddingX * 2 + TicksToPixels(tablature.TicksPerStaff)).ToString(CultureInfo.InvariantCulture)).ConfigureAwait(false);
            await _writer.WriteAttributeStringAsync(null, "height", null, (StaffPaddingY * 2 + ((StaffHeight + StaffPaddingY) * tablature.Staves.Count())).ToString(CultureInfo.InvariantCulture)).ConfigureAwait(false);

            await WriteStyleElementAsync().ConfigureAwait(false);
            await WriteDefsElementAsync().ConfigureAwait(false);

            ITimeSignature currentTimeSignature = null;
            var measureCount = 0;
            var staffY = StaffPaddingY;

            foreach (var staff in tablature.Staves)
            {
                var staffWidth = TicksToPixels(staff.EndTicks - staff.StartTicks);
                var measureCountY = staffY - (NoteSize / 2 + TextPaddingY);
                var tempoY = measureCountY - (TextFontSize + TextPaddingY);

                await WriteStaffAsync(staffY, staffWidth).ConfigureAwait(false);

                foreach (var measure in staff.Measures)
                {
                    var measureX = StaffPaddingX + TicksToPixels(measure.StartTicks - staff.StartTicks);

                    await _writer.WriteStartElementUseAsync("#s", measureX, staffY).ConfigureAwait(false);
                    await _writer.WriteEndElementAsync().ConfigureAwait(false);

                    if (measure.TimeSignature != currentTimeSignature)
                    {
                        currentTimeSignature = measure.TimeSignature;
                        var timeSignatureX = measureX + TicksToPixels(currentTimeSignature.Ticks - measure.StartTicks) + TextFontSize;
                        await WriteTimeSignatureAsync(timeSignatureX, staffY, currentTimeSignature).ConfigureAwait(false);
                    }

                    foreach (var beat in measure.Beats)
                    {
                        var beatX = measureX + TicksToPixels(beat.Ticks - measure.StartTicks);

                        await _writer.WriteStartElementUseAsync("#l", beatX, staffY).ConfigureAwait(false);
                        await _writer.WriteEndElementAsync().ConfigureAwait(false);
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
                            await WriteSustainAsync(sustainX, staffY - SustainHeight / 2, sustainWidth).ConfigureAwait(false);
                            break;
                        case FretFlags.Open:
                        case FretFlags.Black2:
                        case FretFlags.White2:
                        case FretFlags.Black2 | FretFlags.White2:
                            await WriteSustainAsync(sustainX, staffY + StaffHeight / 2 - SustainHeight / 2, sustainWidth).ConfigureAwait(false);
                            break;
                        case FretFlags.Black3:
                        case FretFlags.White3:
                        case FretFlags.Black3 | FretFlags.White3:
                            await WriteSustainAsync(sustainX, staffY + StaffHeight - SustainHeight / 2, sustainWidth).ConfigureAwait(false);
                            break;
                    }
                }

                foreach (var measure in staff.Measures)
                {
                    measureCount++;
                    var measureX = StaffPaddingX + TicksToPixels(measure.StartTicks - staff.StartTicks);
                    await WriteMeasureCountAsync(measureX, measureCountY, measureCount).ConfigureAwait(false);

                    foreach (var tempo in measure.Tempos)
                    {
                        var tempoX = measureX + TicksToPixels(tempo.Ticks - measure.StartTicks);
                        await WriteTempoAsync(tempoX, tempoY, tempo).ConfigureAwait(false);
                    }

                    foreach (var note in measure.Notes)
                    {
                        var noteX = measureX + TicksToPixels(note.Ticks - measure.StartTicks);

                        switch (note.Frets.Flags)
                        {
                            case FretFlags.Open:
                                await _writer.WriteStartElementUseAsync("#o", noteX - NoteSize / 4, staffY - NoteSize / 2).ConfigureAwait(false);
                                await _writer.WriteEndElementAsync().ConfigureAwait(false);
                                break;
                            case FretFlags.Black1:
                                await _writer.WriteStartElementUseAsync("#b", noteX, staffY).ConfigureAwait(false);
                                await _writer.WriteEndElementAsync().ConfigureAwait(false);
                                break;
                            case FretFlags.Black2:
                                await _writer.WriteStartElementUseAsync("#b", noteX, staffY + StaffHeight / 2).ConfigureAwait(false);
                                await _writer.WriteEndElementAsync().ConfigureAwait(false);
                                break;
                            case FretFlags.Black3:
                                await _writer.WriteStartElementUseAsync("#b", noteX, staffY + StaffHeight).ConfigureAwait(false);
                                await _writer.WriteEndElementAsync().ConfigureAwait(false);
                                break;
                            case FretFlags.White1:
                                await _writer.WriteStartElementUseAsync("#w", noteX, staffY).ConfigureAwait(false);
                                await _writer.WriteEndElementAsync().ConfigureAwait(false);
                                break;
                            case FretFlags.White2:
                                await _writer.WriteStartElementUseAsync("#w", noteX, staffY + StaffHeight / 2).ConfigureAwait(false);
                                await _writer.WriteEndElementAsync().ConfigureAwait(false);
                                break;
                            case FretFlags.White3:
                                await _writer.WriteStartElementUseAsync("#w", noteX, staffY + StaffHeight).ConfigureAwait(false);
                                await _writer.WriteEndElementAsync().ConfigureAwait(false);
                                break;
                            case FretFlags.Black1 | FretFlags.White1:
                                await _writer.WriteStartElementAsync(null, "rect", null).ConfigureAwait(false);
                                await _writer.WriteAttributeStringAsync(null, "x", null, (noteX - NoteSize / 2).ToString(CultureInfo.InvariantCulture)).ConfigureAwait(false);
                                await _writer.WriteAttributeStringAsync(null, "y", null, (staffY - NoteSize / 2).ToString(CultureInfo.InvariantCulture)).ConfigureAwait(false);
                                await _writer.WriteAttributeStringAsync(null, "width", null, NoteSize.ToString(CultureInfo.InvariantCulture)).ConfigureAwait(false);
                                await _writer.WriteAttributeStringAsync(null, "height", null, NoteSize.ToString(CultureInfo.InvariantCulture)).ConfigureAwait(false);
                                await _writer.WriteAttributeStringAsync(null, "stroke", null, "black").ConfigureAwait(false);
                                await _writer.WriteAttributeStringAsync(null, "fill", null, "black").ConfigureAwait(false);
                                await _writer.WriteAttributeStringAsync(null, "stroke-width", null, "1").ConfigureAwait(false);
                                await _writer.WriteEndElementAsync().ConfigureAwait(false);

                                await _writer.WriteStartElementAsync(null, "rect", null).ConfigureAwait(false);
                                await _writer.WriteAttributeStringAsync(null, "x", null, (noteX - NoteSize / 2).ToString(CultureInfo.InvariantCulture)).ConfigureAwait(false);
                                await _writer.WriteAttributeStringAsync(null, "y", null, staffY.ToString(CultureInfo.InvariantCulture)).ConfigureAwait(false);
                                await _writer.WriteAttributeStringAsync(null, "width", null, NoteSize.ToString(CultureInfo.InvariantCulture)).ConfigureAwait(false);
                                await _writer.WriteAttributeStringAsync(null, "height", null, (NoteSize / 2).ToString(CultureInfo.InvariantCulture)).ConfigureAwait(false);
                                await _writer.WriteAttributeStringAsync(null, "stroke", null, "black").ConfigureAwait(false);
                                await _writer.WriteAttributeStringAsync(null, "fill", null, "white").ConfigureAwait(false);
                                await _writer.WriteAttributeStringAsync(null, "stroke-width", null, "1").ConfigureAwait(false);
                                await _writer.WriteEndElementAsync().ConfigureAwait(false);
                                break;
                            case FretFlags.Black2 | FretFlags.White2:
                                await _writer.WriteStartElementAsync(null, "rect", null).ConfigureAwait(false);
                                await _writer.WriteAttributeStringAsync(null, "x", null, (noteX - NoteSize / 2).ToString(CultureInfo.InvariantCulture)).ConfigureAwait(false);
                                await _writer.WriteAttributeStringAsync(null, "y", null, (staffY + StaffHeight / 2 - NoteSize / 2).ToString(CultureInfo.InvariantCulture)).ConfigureAwait(false);
                                await _writer.WriteAttributeStringAsync(null, "width", null, NoteSize.ToString(CultureInfo.InvariantCulture)).ConfigureAwait(false);
                                await _writer.WriteAttributeStringAsync(null, "height", null, NoteSize.ToString(CultureInfo.InvariantCulture)).ConfigureAwait(false);
                                await _writer.WriteAttributeStringAsync(null, "stroke", null, "black").ConfigureAwait(false);
                                await _writer.WriteAttributeStringAsync(null, "fill", null, "black").ConfigureAwait(false);
                                await _writer.WriteAttributeStringAsync(null, "stroke-width", null, "1").ConfigureAwait(false);
                                await _writer.WriteEndElementAsync().ConfigureAwait(false);

                                await _writer.WriteStartElementAsync(null, "rect", null).ConfigureAwait(false);
                                await _writer.WriteAttributeStringAsync(null, "x", null, (noteX - NoteSize / 2).ToString(CultureInfo.InvariantCulture)).ConfigureAwait(false);
                                await _writer.WriteAttributeStringAsync(null, "y", null, (staffY + StaffHeight / 2).ToString(CultureInfo.InvariantCulture)).ConfigureAwait(false);
                                await _writer.WriteAttributeStringAsync(null, "width", null, NoteSize.ToString(CultureInfo.InvariantCulture)).ConfigureAwait(false);
                                await _writer.WriteAttributeStringAsync(null, "height", null, (NoteSize / 2).ToString(CultureInfo.InvariantCulture)).ConfigureAwait(false);
                                await _writer.WriteAttributeStringAsync(null, "stroke", null, "black").ConfigureAwait(false);
                                await _writer.WriteAttributeStringAsync(null, "fill", null, "white").ConfigureAwait(false);
                                await _writer.WriteAttributeStringAsync(null, "stroke-width", null, "1").ConfigureAwait(false);
                                await _writer.WriteEndElementAsync().ConfigureAwait(false);
                                break;
                            case FretFlags.Black3 | FretFlags.White3:
                                await _writer.WriteStartElementAsync(null, "rect", null).ConfigureAwait(false);
                                await _writer.WriteAttributeStringAsync(null, "x", null, (noteX - NoteSize / 2).ToString(CultureInfo.InvariantCulture)).ConfigureAwait(false);
                                await _writer.WriteAttributeStringAsync(null, "y", null, (staffY + StaffHeight - NoteSize / 2).ToString(CultureInfo.InvariantCulture)).ConfigureAwait(false);
                                await _writer.WriteAttributeStringAsync(null, "width", null, NoteSize.ToString(CultureInfo.InvariantCulture)).ConfigureAwait(false);
                                await _writer.WriteAttributeStringAsync(null, "height", null, NoteSize.ToString(CultureInfo.InvariantCulture)).ConfigureAwait(false);
                                await _writer.WriteAttributeStringAsync(null, "stroke", null, "black").ConfigureAwait(false);
                                await _writer.WriteAttributeStringAsync(null, "fill", null, "black").ConfigureAwait(false);
                                await _writer.WriteAttributeStringAsync(null, "stroke-width", null, "1").ConfigureAwait(false);
                                await _writer.WriteEndElementAsync().ConfigureAwait(false);

                                await _writer.WriteStartElementAsync(null, "rect", null).ConfigureAwait(false);
                                await _writer.WriteAttributeStringAsync(null, "x", null, (noteX - NoteSize / 2).ToString(CultureInfo.InvariantCulture)).ConfigureAwait(false);
                                await _writer.WriteAttributeStringAsync(null, "y", null, (staffY + StaffHeight).ToString(CultureInfo.InvariantCulture)).ConfigureAwait(false);
                                await _writer.WriteAttributeStringAsync(null, "width", null, NoteSize.ToString(CultureInfo.InvariantCulture)).ConfigureAwait(false);
                                await _writer.WriteAttributeStringAsync(null, "height", null, (NoteSize / 2).ToString(CultureInfo.InvariantCulture)).ConfigureAwait(false);
                                await _writer.WriteAttributeStringAsync(null, "stroke", null, "black").ConfigureAwait(false);
                                await _writer.WriteAttributeStringAsync(null, "fill", null, "white").ConfigureAwait(false);
                                await _writer.WriteAttributeStringAsync(null, "stroke-width", null, "1").ConfigureAwait(false);
                                await _writer.WriteEndElementAsync().ConfigureAwait(false);
                                break;
                        }
                    }
                }

                foreach (var heroPowerPhrase in staff.HeroPowerPhrases)
                {
                    var heroPowerPhraseX = StaffPaddingX + TicksToPixels(heroPowerPhrase.StartTicks - staff.StartTicks);
                    var heroPowerPhraseWidth = TicksToPixels(heroPowerPhrase.EndTicks - heroPowerPhrase.StartTicks);
                    await WritePhraseAsync(heroPowerPhraseX, staffY, heroPowerPhraseWidth, "g").ConfigureAwait(false);
                }

                foreach (var highwayPhrase in staff.HighwayPhrases)
                {
                    var highwayPhraseX = StaffPaddingX + TicksToPixels(highwayPhrase.StartTicks - staff.StartTicks);
                    var highwayPhraseWidth = TicksToPixels(highwayPhrase.EndTicks - highwayPhrase.StartTicks);
                    await WritePhraseAsync(highwayPhraseX, staffY, highwayPhraseWidth, "y").ConfigureAwait(false);
                }

                foreach (var activation in staff.Activations)
                {
                    var activationX = StaffPaddingX + TicksToPixels(activation.StartTicks - staff.StartTicks);
                    var activationWidth = TicksToPixels(activation.EndTicks - activation.StartTicks);
                    await WritePhraseAsync(activationX, staffY, activationWidth, "b").ConfigureAwait(false);
                }

                staffY += StaffHeight + StaffPaddingY;
            }

            await _writer.WriteEndElementAsync().ConfigureAwait(false);
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

        private async Task WriteStyleElementAsync()
        {
            await _writer.WriteStartElementAsync("style").ConfigureAwait(false);
            await _writer.WriteStringAsync("text { font-family: sans-serif; }").ConfigureAwait(false);
            await _writer.WriteStringAsync(".s, .w { fill: transparent; stroke-width: 1px; }").ConfigureAwait(false);
            await _writer.WriteStringAsync(".s { stroke: black; }").ConfigureAwait(false);
            await _writer.WriteStringAsync(".w { stroke: gray; }").ConfigureAwait(false);
            await _writer.WriteStringAsync(".m, .t { font-size: ").ConfigureAwait(false);
            await _writer.WriteValueAsync(TextFontSize).ConfigureAwait(false);
            await _writer.WriteStringAsync("px; }").ConfigureAwait(false);
            await _writer.WriteStringAsync(".n { font-size: ").ConfigureAwait(false);
            await _writer.WriteValueAsync(StaffHeight / 2).ConfigureAwait(false);
            await _writer.WriteStringAsync("px; }").ConfigureAwait(false);
            await _writer.WriteStringAsync(".m { fill: red; }").ConfigureAwait(false);
            await _writer.WriteStringAsync(".t, .n, .l { fill: gray; }").ConfigureAwait(false);
            await _writer.WriteStringAsync(".g, .y, .b { opacity: 0.3; }").ConfigureAwait(false);
            await _writer.WriteStringAsync(".g { fill: green; }").ConfigureAwait(false);
            await _writer.WriteStringAsync(".y { fill: yellow; }").ConfigureAwait(false);
            await _writer.WriteStringAsync(".b { fill: blue; }").ConfigureAwait(false);
            await _writer.WriteEndElementAsync().ConfigureAwait(false);
        }

        private void WriteDefsElement()
        {
            _writer.WriteStartElement("defs");

            _writer.WriteStartElement("path");
            _writer.WriteAttributeString("id", "h");
            _writer.WriteStartAttribute("d");

            _svgPathWriter.StartPath();
            _svgPathWriter.WriteMoveTo(0, 0);
            _svgPathWriter.WriteVerticalLineToRelative(StaffHeight);
            _svgPathWriter.EndPath();

            _writer.WriteEndAttribute();
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

        private async Task WriteDefsElementAsync()
        {
            await _writer.WriteStartElementAsync("defs").ConfigureAwait(false);

            await _writer.WriteStartElementAsync("path").ConfigureAwait(false);
            await _writer.WriteAttributeStringAsync("id", "h").ConfigureAwait(false);
            _writer.WriteStartAttribute("d");

            _svgPathWriter.StartPath();
            await _svgPathWriter.WriteMoveToAsync(0, 0).ConfigureAwait(false);
            await _svgPathWriter.WriteVerticalLineToRelativeAsync(StaffHeight).ConfigureAwait(false);
            _svgPathWriter.EndPath();

            _writer.WriteEndAttribute();
            await _writer.WriteAttributeDoubleAsync("stroke-width", 1).ConfigureAwait(false);
            await _writer.WriteEndElementAsync().ConfigureAwait(false);

            await _writer.WriteStartElementUseAsync("#h").ConfigureAwait(false);
            await _writer.WriteAttributeStringAsync("id", "s").ConfigureAwait(false);
            await _writer.WriteAttributeStringAsync("stroke", "black").ConfigureAwait(false);
            await _writer.WriteEndElementAsync().ConfigureAwait(false);

            await _writer.WriteStartElementUseAsync("#h").ConfigureAwait(false);
            await _writer.WriteAttributeStringAsync("id", "l").ConfigureAwait(false);
            await _writer.WriteAttributeStringAsync("stroke", "gray").ConfigureAwait(false);
            await _writer.WriteEndElementAsync().ConfigureAwait(false);

            await _writer.WriteStartElementAsync("circle").ConfigureAwait(false);
            await _writer.WriteAttributeStringAsync("id", "n").ConfigureAwait(false);
            await _writer.WriteAttributeDoubleAsync("r", NoteSize / 2).ConfigureAwait(false);
            await _writer.WriteAttributeStringAsync("stroke", "black").ConfigureAwait(false);
            await _writer.WriteAttributeDoubleAsync("stroke-width", 1).ConfigureAwait(false);
            await _writer.WriteEndElementAsync().ConfigureAwait(false);

            await _writer.WriteStartElementUseAsync("#n").ConfigureAwait(false);
            await _writer.WriteAttributeStringAsync("id", "b").ConfigureAwait(false);
            await _writer.WriteAttributeStringAsync("fill", "black").ConfigureAwait(false);
            await _writer.WriteEndElementAsync().ConfigureAwait(false);

            await _writer.WriteStartElementUseAsync("#n").ConfigureAwait(false);
            await _writer.WriteAttributeStringAsync("id", "w").ConfigureAwait(false);
            await _writer.WriteAttributeStringAsync("fill", "white").ConfigureAwait(false);
            await _writer.WriteEndElementAsync().ConfigureAwait(false);

            await _writer.WriteStartElementAsync("rect").ConfigureAwait(false);
            await _writer.WriteAttributeStringAsync("id", "o").ConfigureAwait(false);
            await _writer.WriteAttributeDoubleAsync("width", NoteSize / 2).ConfigureAwait(false);
            await _writer.WriteAttributeDoubleAsync("height", StaffHeight + NoteSize).ConfigureAwait(false);
            await _writer.WriteAttributeStringAsync("stroke", "black").ConfigureAwait(false);
            await _writer.WriteAttributeStringAsync("fill", "white").ConfigureAwait(false);
            await _writer.WriteAttributeDoubleAsync("stroke-width", 1).ConfigureAwait(false);
            await _writer.WriteEndElementAsync().ConfigureAwait(false);

            await _writer.WriteEndElementAsync().ConfigureAwait(false);
        }

        private void WriteStaff(double y, double width)
        {
            _writer.WriteStartElement("path");
            _writer.WriteStartAttribute("d");

            _svgPathWriter.StartPath();
            _svgPathWriter.WriteMoveTo(StaffPaddingX, y);
            _svgPathWriter.WriteHorizontalLineToRelative(width);
            _svgPathWriter.WriteVerticalLineToRelative(StaffHeight);
            _svgPathWriter.WriteHorizontalLineToRelative(-width);
            _svgPathWriter.EndPath();

            _writer.WriteEndAttribute();
            _writer.WriteAttributeString("class", "s");
            _writer.WriteEndElement();

            _writer.WriteStartElement("path");
            _writer.WriteStartAttribute("d");

            _svgPathWriter.StartPath();
            _svgPathWriter.WriteMoveTo(StaffPaddingX, y + StaffHeight / 2);
            _svgPathWriter.WriteHorizontalLineToRelative(width);
            _svgPathWriter.EndPath();

            _writer.WriteEndAttribute();
            _writer.WriteAttributeString("class", "w");
            _writer.WriteEndElement();
        }

        private async Task WriteStaffAsync(double y, double width)
        {
            await _writer.WriteStartElementAsync("path").ConfigureAwait(false);
            _writer.WriteStartAttribute("d");

            _svgPathWriter.StartPath();
            await _svgPathWriter.WriteMoveToAsync(StaffPaddingX, y).ConfigureAwait(false);
            await _svgPathWriter.WriteHorizontalLineToRelativeAsync(width).ConfigureAwait(false);
            await _svgPathWriter.WriteVerticalLineToRelativeAsync(StaffHeight).ConfigureAwait(false);
            await _svgPathWriter.WriteHorizontalLineToRelativeAsync(-width).ConfigureAwait(false);
            _svgPathWriter.EndPath();

            _writer.WriteEndAttribute();
            await _writer.WriteAttributeStringAsync("class", "s").ConfigureAwait(false);
            await _writer.WriteEndElementAsync().ConfigureAwait(false);

            await _writer.WriteStartElementAsync("path").ConfigureAwait(false);
            _writer.WriteStartAttribute("d");

            _svgPathWriter.StartPath();
            await _svgPathWriter.WriteMoveToAsync(StaffPaddingX, y + StaffHeight / 2).ConfigureAwait(false);
            await _svgPathWriter.WriteHorizontalLineToRelativeAsync(width).ConfigureAwait(false);
            _svgPathWriter.EndPath();

            _writer.WriteEndAttribute();
            await _writer.WriteAttributeStringAsync("class", "w").ConfigureAwait(false);
            await _writer.WriteEndElementAsync().ConfigureAwait(false);
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

        private async Task WriteTimeSignatureAsync(double x, double y, ITimeSignature timeSignature)
        {
            await _writer.WriteStartElementAsync("text").ConfigureAwait(false);
            await _writer.WriteAttributeDoubleAsync("x", x).ConfigureAwait(false);
            await _writer.WriteAttributeDoubleAsync("y", y + StaffHeight / 2 - TextPaddingY / 2).ConfigureAwait(false);
            await _writer.WriteAttributeStringAsync("class", "n").ConfigureAwait(false);

            await _writer.WriteValueAsync(timeSignature.Numerator).ConfigureAwait(false);

            await _writer.WriteEndElementAsync().ConfigureAwait(false);

            await _writer.WriteStartElementAsync("text").ConfigureAwait(false);
            await _writer.WriteAttributeDoubleAsync("x", x).ConfigureAwait(false);
            await _writer.WriteAttributeDoubleAsync("y", y + StaffHeight - TextPaddingY / 2).ConfigureAwait(false);
            await _writer.WriteAttributeStringAsync("class", "n").ConfigureAwait(false);

            await _writer.WriteValueAsync(timeSignature.Denominator).ConfigureAwait(false);

            await _writer.WriteEndElementAsync().ConfigureAwait(false);
        }

        private void WriteSustain(double x, double y, double width)
        {
            _writer.WriteStartElement("path");
            _writer.WriteStartAttribute("d");

            _svgPathWriter.StartPath();
            _svgPathWriter.WriteMoveTo(x, y);
            _svgPathWriter.WriteHorizontalLineToRelative(width);
            _svgPathWriter.WriteVerticalLineToRelative(SustainHeight);
            _svgPathWriter.WriteHorizontalLineToRelative(-width);
            _svgPathWriter.EndPath();

            _writer.WriteEndAttribute();
            _writer.WriteAttributeString("class", "l");
            _writer.WriteEndElement();
        }

        private async Task WriteSustainAsync(double x, double y, double width)
        {
            await _writer.WriteStartElementAsync("path").ConfigureAwait(false);
            _writer.WriteStartAttribute("d");

            _svgPathWriter.StartPath();
            await _svgPathWriter.WriteMoveToAsync(x, y).ConfigureAwait(false);
            await _svgPathWriter.WriteHorizontalLineToRelativeAsync(width).ConfigureAwait(false);
            await _svgPathWriter.WriteVerticalLineToRelativeAsync(SustainHeight).ConfigureAwait(false);
            await _svgPathWriter.WriteHorizontalLineToRelativeAsync(-width).ConfigureAwait(false);
            _svgPathWriter.EndPath();

            _writer.WriteEndAttribute();
            await _writer.WriteAttributeStringAsync("class", "l").ConfigureAwait(false);
            await _writer.WriteEndElementAsync().ConfigureAwait(false);
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

        private async Task WriteMeasureCountAsync(double x, double y, int measureCount)
        {
            await _writer.WriteStartElementAsync("text").ConfigureAwait(false);
            await _writer.WriteAttributeDoubleAsync("x", x).ConfigureAwait(false);
            await _writer.WriteAttributeDoubleAsync("y", y).ConfigureAwait(false);
            await _writer.WriteAttributeStringAsync("class", "m").ConfigureAwait(false);

            await _writer.WriteValueAsync(measureCount).ConfigureAwait(false);

            await _writer.WriteEndElementAsync().ConfigureAwait(false);
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

        private async Task WriteTempoAsync(double x, double y, ITempo tempo)
        {
            await _writer.WriteStartElementAsync("text").ConfigureAwait(false);
            await _writer.WriteAttributeDoubleAsync("x", x).ConfigureAwait(false);
            await _writer.WriteAttributeDoubleAsync("y", y).ConfigureAwait(false);
            await _writer.WriteAttributeStringAsync("class", "t").ConfigureAwait(false);

            await _writer.WriteStringAsync("\u2669=").ConfigureAwait(false);
            await _writer.WriteValueAsync(Math.Round(tempo.BeatsPerMinute)).ConfigureAwait(false);

            await _writer.WriteEndElementAsync().ConfigureAwait(false);
        }

        private void WritePhrase(double x, double y, double width, string @class)
        {
            _writer.WriteStartElement("path");
            _writer.WriteStartAttribute("d");

            _svgPathWriter.StartPath();
            _svgPathWriter.WriteMoveTo(x, y);
            _svgPathWriter.WriteHorizontalLineToRelative(width);
            _svgPathWriter.WriteVerticalLineToRelative(StaffHeight);
            _svgPathWriter.WriteHorizontalLineToRelative(-width);
            _svgPathWriter.WriteClosePath();
            _svgPathWriter.EndPath();

            _writer.WriteEndAttribute();
            _writer.WriteAttributeString("class", @class);
            _writer.WriteEndElement();
        }

        private async Task WritePhraseAsync(double x, double y, double width, string @class)
        {
            await _writer.WriteStartElementAsync("path").ConfigureAwait(false);
            _writer.WriteStartAttribute("d");

            _svgPathWriter.StartPath();
            await _svgPathWriter.WriteMoveToAsync(x, y).ConfigureAwait(false);
            await _svgPathWriter.WriteHorizontalLineToRelativeAsync(width).ConfigureAwait(false);
            await _svgPathWriter.WriteVerticalLineToRelativeAsync(StaffHeight).ConfigureAwait(false);
            await _svgPathWriter.WriteHorizontalLineToRelativeAsync(-width).ConfigureAwait(false);
            await _svgPathWriter.WriteClosePathAsync().ConfigureAwait(false);
            _svgPathWriter.EndPath();

            _writer.WriteEndAttribute();
            await _writer.WriteAttributeStringAsync("class", @class).ConfigureAwait(false);
            await _writer.WriteEndElementAsync().ConfigureAwait(false);
        }

        private double TicksToPixels(int ticks, int ticksPerQuarterNote)
        {
            return (double)ticks / ticksPerQuarterNote * PixelsPerQuarterNote;
        }
    }
}

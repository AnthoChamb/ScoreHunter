using ScoreHunter.Drawing.Svg.Enums;
using System.IO;
using System.Threading.Tasks;
using System.Xml;

namespace ScoreHunter.Drawing.Svg.IO
{
    public class SvgPathTextWriter : SvgPathWriter
    {
        private readonly TextWriter _writer;
        private readonly string _separator;

        public SvgPathTextWriter(TextWriter writer) : this(writer, " ")
        {
        }

        public SvgPathTextWriter(TextWriter writer, string separator)
        {
            _writer = writer;
            _separator = separator;
        }

        protected override void WriteCommand(char command, SvgPathWriteState writeState)
        {
            _writer.Write(command);
        }

        protected override async Task WriteCommandAsync(char command, SvgPathWriteState writeState)
        {
            await _writer.WriteAsync(command.ToString()).ConfigureAwait(false);
        }

        protected override void WriteParameter(double value, SvgPathWriteState writeState)
        {
            _writer.Write(XmlConvert.ToString(value));
        }

        protected override async Task WriteParameterAsync(double value, SvgPathWriteState writeState)
        {
            await _writer.WriteAsync(XmlConvert.ToString(value)).ConfigureAwait(false);
        }

        protected override void WriteSeparator(double value)
        {
            _writer.Write(_separator);
        }

        protected override async Task WriteSeparatorAsync(double value)
        {
            await _writer.WriteAsync(_separator).ConfigureAwait(false);
        }
    }
}

using ScoreHunter.Drawing.Svg.Enums;
using ScoreHunter.Drawing.Svg.Extensions;
using System.Threading.Tasks;
using System.Xml;

namespace ScoreHunter.Drawing.Svg.Xml
{
    public class XmlSvgPathWriter : SvgPathWriter
    {
        private readonly XmlWriter _writer;
        private readonly string _separator;

        public XmlSvgPathWriter(XmlWriter writer) : this(writer, " ")
        {
        }

        public XmlSvgPathWriter(XmlWriter writer, string separator)
        {
            _writer = writer;
            _separator = separator;
        }

        protected override void WriteCommand(char command, SvgPathWriteState writeState)
        {
            _writer.WriteString(command.ToString());
        }

        protected override async Task WriteCommandAsync(char command, SvgPathWriteState writeState)
        {
            await _writer.WriteStringAsync(command.ToString()).ConfigureAwait(false);
        }

        protected override void WriteParameter(double value, SvgPathWriteState writeState)
        {
            _writer.WriteValue(value);
        }

        protected override async Task WriteParameterAsync(double value, SvgPathWriteState writeState)
        {
            await _writer.WriteValueAsync(value).ConfigureAwait(false);
        }

        protected override void WriteSeparator(double value)
        {
            _writer.WriteString(_separator);
        }

        protected override async Task WriteSeparatorAsync(double value)
        {
            await _writer.WriteStringAsync(_separator).ConfigureAwait(false);
        }
    }
}

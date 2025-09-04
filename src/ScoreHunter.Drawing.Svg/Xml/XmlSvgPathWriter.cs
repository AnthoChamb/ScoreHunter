using ScoreHunter.Drawing.Svg.Enums;
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

        protected override void WriteParameter(double value, SvgPathWriteState writeState)
        {
            _writer.WriteValue(value);
        }

        protected override void WriteSeparator(double value)
        {
            _writer.WriteString(_separator);
        }
    }
}

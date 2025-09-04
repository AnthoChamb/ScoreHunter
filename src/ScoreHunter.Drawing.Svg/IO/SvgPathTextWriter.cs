using ScoreHunter.Drawing.Svg.Enums;
using System.IO;
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

        protected override void WriteParameter(double value, SvgPathWriteState writeState)
        {
            _writer.Write(XmlConvert.ToString(value));
        }

        protected override void WriteSeparator(double value)
        {
            _writer.Write(_separator);
        }
    }
}

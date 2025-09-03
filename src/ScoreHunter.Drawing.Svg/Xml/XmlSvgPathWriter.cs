using ScoreHunter.Drawing.Svg.Enums;
using System;
using System.Xml;

namespace ScoreHunter.Drawing.Svg.Xml
{
    public class XmlSvgPathWriter : SvgPathWriter
    {
        private readonly XmlWriter _writer;

        private SvgPathWriteState _writeState;
        private char _command;

        public XmlSvgPathWriter(XmlWriter writer)
        {
            _writer = writer;
        }

        public override SvgPathWriteState WriteState => _writeState;

        public override void StartPath()
        {
            _writeState = SvgPathWriteState.Path;
        }

        public override void EndPath()
        {
            _writeState = SvgPathWriteState.Closed;
        }

        protected override void WriteCommand(char command)
        {
            switch (_writeState)
            {
                case SvgPathWriteState.Path:
                    WriteCommandCore(command);
                    break;
                case SvgPathWriteState.Command:
                    _writer.WriteString(" ");
                    WriteCommandCore(command);
                    break;
                case SvgPathWriteState.Parameter:
                    if (_command != command)
                    {
                        WriteCommandCore(command);
                    }
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        protected override void WriteParameter(double value)
        {
            switch (_writeState)
            {
                case SvgPathWriteState.Command:
                    break;
                case SvgPathWriteState.Parameter:
                    _writer.WriteString(" ");
                    break;
                default:
                    throw new InvalidOperationException();
            }
            _writer.WriteValue(value);
            _writeState = SvgPathWriteState.Parameter;
        }

        private void WriteCommandCore(char command)
        {
            _writer.WriteString(command.ToString());
            _writeState = SvgPathWriteState.Command;
            _command = command;
        }
    }
}

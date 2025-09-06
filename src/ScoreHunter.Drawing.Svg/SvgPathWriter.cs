using CommunityToolkit.Diagnostics;
using ScoreHunter.Drawing.Svg.Enums;
using System;
using System.Threading.Tasks;

namespace ScoreHunter.Drawing.Svg
{
    public abstract class SvgPathWriter : IDisposable
    {
        private char _command;

        public SvgPathWriteState WriteState { get; private set; }

        protected abstract void WriteCommand(char command, SvgPathWriteState writeState);

        protected virtual Task WriteCommandAsync(char command, SvgPathWriteState writeState)
        {
            WriteCommand(command, writeState);
            return Task.CompletedTask;
        }

        protected abstract void WriteParameter(double value, SvgPathWriteState writeState);

        protected virtual Task WriteParameterAsync(double value, SvgPathWriteState writeState)
        {
            WriteParameter(value, writeState);
            return Task.CompletedTask;
        }

        protected abstract void WriteSeparator(double value);

        protected virtual Task WriteSeparatorAsync(double value)
        {
            WriteSeparator(value);
            return Task.CompletedTask;
        }

        public void StartPath()
        {
            WriteState = SvgPathWriteState.Path;
        }

        public void EndPath()
        {
            WriteState = SvgPathWriteState.Closed;
            _command = '\0';
        }

        private void WriteCommand(char command)
        {
            switch (WriteState)
            {
                case SvgPathWriteState.Path:
                case SvgPathWriteState.Command:
                    WriteCommandCore(command);
                    break;
                case SvgPathWriteState.Parameter:
                    if (_command != command)
                    {
                        WriteCommandCore(command);
                    }
                    break;
                default:
                    ThrowHelper.ThrowInvalidOperationException();
                    break;
            }
        }

        private async Task WriteCommandAsync(char command)
        {
            switch (WriteState)
            {
                case SvgPathWriteState.Path:
                case SvgPathWriteState.Command:
                    await WriteCommandCoreAsync(command).ConfigureAwait(false);
                    break;
                case SvgPathWriteState.Parameter:
                    if (_command != command)
                    {
                        await WriteCommandCoreAsync(command).ConfigureAwait(false);
                    }
                    break;
                default:
                    ThrowHelper.ThrowInvalidOperationException();
                    break;
            }
        }

        private void WriteCommandCore(char command)
        {
            WriteCommand(command, WriteState);
            WriteState = SvgPathWriteState.Command;
            _command = command;
        }

        private async Task WriteCommandCoreAsync(char command)
        {
            await WriteCommandAsync(command, WriteState).ConfigureAwait(false);
            WriteState = SvgPathWriteState.Command;
            _command = command;
        }

        private void WriteParameter(double value)
        {
            switch (WriteState)
            {
                case SvgPathWriteState.Command:
                    WriteParameterCore(value);
                    break;
                case SvgPathWriteState.Parameter:
                    WriteSeparator(value);
                    WriteParameterCore(value);
                    break;
                default:
                    ThrowHelper.ThrowInvalidOperationException();
                    break;
            }
        }

        private async Task WriteParameterAsync(double value)
        {
            switch (WriteState)
            {
                case SvgPathWriteState.Command:
                    await WriteParameterCoreAsync(value).ConfigureAwait(false);
                    break;
                case SvgPathWriteState.Parameter:
                    await WriteSeparatorAsync(value).ConfigureAwait(false);
                    await WriteParameterCoreAsync(value).ConfigureAwait(false);
                    break;
                default:
                    ThrowHelper.ThrowInvalidOperationException();
                    break;
            }
        }

        private void WriteParameterCore(double value)
        {
            WriteParameter(value, WriteState);
            WriteState = SvgPathWriteState.Parameter;
        }

        private async Task WriteParameterCoreAsync(double value)
        {
            await WriteParameterAsync(value, WriteState).ConfigureAwait(false);
            WriteState = SvgPathWriteState.Parameter;
        }

        private void WriteParameter(bool value)
        {
            if (value)
            {
                WriteParameter(1);
            }
            else
            {
                WriteParameter(0);
            }
        }

        private async Task WriteParameterAsync(bool value)
        {
            if (value)
            {
                await WriteParameterAsync(1).ConfigureAwait(false);
            }
            else
            {
                await WriteParameterAsync(0).ConfigureAwait(false);
            }
        }

        public void WriteMoveTo(double x, double y)
        {
            WriteCommand('M');
            WriteParameter(x);
            WriteParameter(y);
        }

        public async Task WriteMoveToAsync(double x, double y)
        {
            await WriteCommandAsync('M').ConfigureAwait(false);
            await WriteParameterAsync(x).ConfigureAwait(false);
            await WriteParameterAsync(y).ConfigureAwait(false);
        }

        public void WriteMoveToRelative(double dx, double dy)
        {
            WriteCommand('m');
            WriteParameter(dx);
            WriteParameter(dy);
        }

        public async Task WriteMoveToRelativeAsync(double dx, double dy)
        {
            await WriteCommandAsync('m').ConfigureAwait(false);
            await WriteParameterAsync(dx).ConfigureAwait(false);
            await WriteParameterAsync(dy).ConfigureAwait(false);
        }

        public void WriteLineTo(double x, double y)
        {
            WriteCommand('L');
            WriteParameter(x);
            WriteParameter(y);
        }

        public async Task WriteLineToAsync(double x, double y)
        {
            await WriteCommandAsync('L').ConfigureAwait(false);
            await WriteParameterAsync(x).ConfigureAwait(false);
            await WriteParameterAsync(y).ConfigureAwait(false);
        }

        public void WriteLineToRelative(double dx, double dy)
        {
            WriteCommand('l');
            WriteParameter(dx);
            WriteParameter(dy);
        }

        public async Task WriteLineToRelativeAsync(double dx, double dy)
        {
            await WriteCommandAsync('l').ConfigureAwait(false);
            await WriteParameterAsync(dx).ConfigureAwait(false);
            await WriteParameterAsync(dy).ConfigureAwait(false);
        }

        public void WriteHorizontalLineTo(double x)
        {
            WriteCommand('H');
            WriteParameter(x);
        }

        public async Task WriteHorizontalLineToAsync(double x)
        {
            await WriteCommandAsync('H').ConfigureAwait(false);
            await WriteParameterAsync(x).ConfigureAwait(false);
        }

        public void WriteHorizontalLineToRelative(double dx)
        {
            WriteCommand('h');
            WriteParameter(dx);
        }

        public async Task WriteHorizontalLineToRelativeAsync(double dx)
        {
            await WriteCommandAsync('h').ConfigureAwait(false);
            await WriteParameterAsync(dx).ConfigureAwait(false);
        }

        public void WriteVerticalLineTo(double y)
        {
            WriteCommand('V');
            WriteParameter(y);
        }

        public async Task WriteVerticalLineToAsync(double y)
        {
            await WriteCommandAsync('V').ConfigureAwait(false);
            await WriteParameterAsync(y).ConfigureAwait(false);
        }

        public void WriteVerticalLineToRelative(double dy)
        {
            WriteCommand('v');
            WriteParameter(dy);
        }

        public async Task WriteVerticalLineToRelativeAsync(double dy)
        {
            await WriteCommandAsync('v').ConfigureAwait(false);
            await WriteParameterAsync(dy).ConfigureAwait(false);
        }

        public void WriteCubicBezierCurve(double x1, double y1, double x2, double y2, double x, double y)
        {
            WriteCommand('C');
            WriteParameter(x1);
            WriteParameter(y1);
            WriteParameter(x2);
            WriteParameter(y2);
            WriteParameter(x);
            WriteParameter(y);
        }

        public async Task WriteCubicBezierCurveAsync(double x1, double y1, double x2, double y2, double x, double y)
        {
            await WriteCommandAsync('C').ConfigureAwait(false);
            await WriteParameterAsync(x1).ConfigureAwait(false);
            await WriteParameterAsync(y1).ConfigureAwait(false);
            await WriteParameterAsync(x2).ConfigureAwait(false);
            await WriteParameterAsync(y2).ConfigureAwait(false);
            await WriteParameterAsync(x).ConfigureAwait(false);
            await WriteParameterAsync(y).ConfigureAwait(false);
        }

        public void WriteCubicBezierCurveRelative(double dx1, double dy1, double dx2, double dy2, double dx, double dy)
        {
            WriteCommand('c');
            WriteParameter(dx1);
            WriteParameter(dy1);
            WriteParameter(dx2);
            WriteParameter(dy2);
            WriteParameter(dx);
            WriteParameter(dy);
        }

        public async Task WriteCubicBezierCurveRelativeAsync(double dx1, double dy1, double dx2, double dy2, double dx, double dy)
        {
            await WriteCommandAsync('c').ConfigureAwait(false);
            await WriteParameterAsync(dx1).ConfigureAwait(false);
            await WriteParameterAsync(dy1).ConfigureAwait(false);
            await WriteParameterAsync(dx2).ConfigureAwait(false);
            await WriteParameterAsync(dy2).ConfigureAwait(false);
            await WriteParameterAsync(dx).ConfigureAwait(false);
            await WriteParameterAsync(dy).ConfigureAwait(false);
        }

        public void WriteSmoothCubicBezierCurve(double x2, double y2, double x, double y)
        {
            WriteCommand('S');
            WriteParameter(x2);
            WriteParameter(y2);
            WriteParameter(x);
            WriteParameter(y);
        }

        public async Task WriteSmoothCubicBezierCurveAsync(double x2, double y2, double x, double y)
        {
            await WriteCommandAsync('S').ConfigureAwait(false);
            await WriteParameterAsync(x2).ConfigureAwait(false);
            await WriteParameterAsync(y2).ConfigureAwait(false);
            await WriteParameterAsync(x).ConfigureAwait(false);
            await WriteParameterAsync(y).ConfigureAwait(false);
        }

        public void WriteSmoothCubicBezierCurveRelative(double dx2, double dy2, double dx, double dy)
        {
            WriteCommand('s');
            WriteParameter(dx2);
            WriteParameter(dy2);
            WriteParameter(dx);
            WriteParameter(dy);
        }

        public async Task WriteSmoothCubicBezierCurveRelativeAsync(double dx2, double dy2, double dx, double dy)
        {
            await WriteCommandAsync('s').ConfigureAwait(false);
            await WriteParameterAsync(dx2).ConfigureAwait(false);
            await WriteParameterAsync(dy2).ConfigureAwait(false);
            await WriteParameterAsync(dx).ConfigureAwait(false);
            await WriteParameterAsync(dy).ConfigureAwait(false);
        }

        public void WriteQuadraticBezierCurve(double x1, double y1, double x, double y)
        {
            WriteCommand('Q');
            WriteParameter(x1);
            WriteParameter(y1);
            WriteParameter(x);
            WriteParameter(y);
        }

        public async Task WriteQuadraticBezierCurveAsync(double x1, double y1, double x, double y)
        {
            await WriteCommandAsync('Q').ConfigureAwait(false);
            await WriteParameterAsync(x1).ConfigureAwait(false);
            await WriteParameterAsync(y1).ConfigureAwait(false);
            await WriteParameterAsync(x).ConfigureAwait(false);
            await WriteParameterAsync(y).ConfigureAwait(false);
        }

        public void WriteQuadraticBezierCurveRelative(double dx1, double dy1, double dx, double dy)
        {
            WriteCommand('q');
            WriteParameter(dx1);
            WriteParameter(dy1);
            WriteParameter(dx);
            WriteParameter(dy);
        }

        public async Task WriteQuadraticBezierCurveRelativeAsync(double dx1, double dy1, double dx, double dy)
        {
            await WriteCommandAsync('q').ConfigureAwait(false);
            await WriteParameterAsync(dx1).ConfigureAwait(false);
            await WriteParameterAsync(dy1).ConfigureAwait(false);
            await WriteParameterAsync(dx).ConfigureAwait(false);
            await WriteParameterAsync(dy).ConfigureAwait(false);
        }

        public void WriteSmoothQuadraticBezierCurve(double x, double y)
        {
            WriteCommand('T');
            WriteParameter(x);
            WriteParameter(y);
        }

        public async Task WriteSmoothQuadraticBezierCurveAsync(double x, double y)
        {
            await WriteCommandAsync('T').ConfigureAwait(false);
            await WriteParameterAsync(x).ConfigureAwait(false);
            await WriteParameterAsync(y).ConfigureAwait(false);
        }

        public void WriteSmoothQuadraticBezierCurveRelative(double dx, double dy)
        {
            WriteCommand('t');
            WriteParameter(dx);
            WriteParameter(dy);
        }

        public async Task WriteSmoothQuadraticBezierCurveRelativeAsync(double dx, double dy)
        {
            await WriteCommandAsync('t').ConfigureAwait(false);
            await WriteParameterAsync(dx).ConfigureAwait(false);
            await WriteParameterAsync(dy).ConfigureAwait(false);
        }

        public void WriteArc(double rx, double ry, double angle, bool largeArc, bool sweep, double x, double y)
        {
            WriteCommand('A');
            WriteParameter(rx);
            WriteParameter(ry);
            WriteParameter(angle);
            WriteParameter(largeArc);
            WriteParameter(sweep);
            WriteParameter(x);
            WriteParameter(y);
        }

        public async Task WriteArcAsync(double rx, double ry, double angle, bool largeArc, bool sweep, double x, double y)
        {
            await WriteCommandAsync('A').ConfigureAwait(false);
            await WriteParameterAsync(rx).ConfigureAwait(false);
            await WriteParameterAsync(ry).ConfigureAwait(false);
            await WriteParameterAsync(angle).ConfigureAwait(false);
            await WriteParameterAsync(largeArc).ConfigureAwait(false);
            await WriteParameterAsync(sweep).ConfigureAwait(false);
            await WriteParameterAsync(x).ConfigureAwait(false);
            await WriteParameterAsync(y).ConfigureAwait(false);
        }

        public void WriteArcRelative(double rx, double ry, double angle, bool largeArc, bool sweep, double dx, double dy)
        {
            WriteCommand('a');
            WriteParameter(rx);
            WriteParameter(ry);
            WriteParameter(angle);
            WriteParameter(largeArc);
            WriteParameter(sweep);
            WriteParameter(dx);
            WriteParameter(dy);
        }

        public async Task WriteArcRelativeAsync(double rx, double ry, double angle, bool largeArc, bool sweep, double dx, double dy)
        {
            await WriteCommandAsync('a').ConfigureAwait(false);
            await WriteParameterAsync(rx).ConfigureAwait(false);
            await WriteParameterAsync(ry).ConfigureAwait(false);
            await WriteParameterAsync(angle).ConfigureAwait(false);
            await WriteParameterAsync(largeArc).ConfigureAwait(false);
            await WriteParameterAsync(sweep).ConfigureAwait(false);
            await WriteParameterAsync(dx).ConfigureAwait(false);
            await WriteParameterAsync(dy).ConfigureAwait(false);
        }

        public void WriteClosePath()
        {
            WriteCommand('Z');
        }

        public async Task WriteClosePathAsync()
        {
            await WriteCommandAsync('Z').ConfigureAwait(false);
        }

        public virtual void Dispose()
        {
        }
    }
}

using ScoreHunter.Drawing.Svg.Enums;
using System;

namespace ScoreHunter.Drawing.Svg
{
    public abstract class SvgPathWriter : IDisposable
    {
        public abstract SvgPathWriteState WriteState { get; }

        public abstract void StartPath();
        public abstract void EndPath();

        protected abstract void WriteCommand(char command);
        protected abstract void WriteParameter(double value);

        protected void WriteParameter(bool value)
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

        public void WriteMoveTo(double x, double y)
        {
            WriteCommand('M');
            WriteParameter(x);
            WriteParameter(y);
        }

        public void WriteMoveToRelative(double dx, double dy)
        {
            WriteCommand('m');
            WriteParameter(dx);
            WriteParameter(dy);
        }

        public void WriteLineTo(double x, double y)
        {
            WriteCommand('L');
            WriteParameter(x);
            WriteParameter(y);
        }

        public void WriteLineToRelative(double dx, double dy)
        {
            WriteCommand('l');
            WriteParameter(dx);
            WriteParameter(dy);
        }

        public void WriteHorizontalLineTo(double x)
        {
            WriteCommand('H');
            WriteParameter(x);
        }

        public void WriteHorizontalLineToRelative(double dx)
        {
            WriteCommand('h');
            WriteParameter(dx);
        }

        public void WriteVerticalLineTo(double y)
        {
            WriteCommand('V');
            WriteParameter(y);
        }

        public void WriteVerticalLineToRelative(double dy)
        {
            WriteCommand('v');
            WriteParameter(dy);
        }

        public void WriteCubicBezier(double x1, double y1, double x2, double y2, double x, double y)
        {
            WriteCommand('C');
            WriteParameter(x1);
            WriteParameter(y1);
            WriteParameter(x2);
            WriteParameter(y2);
            WriteParameter(x);
            WriteParameter(y);
        }

        public void WriteCubicBezierRelative(double dx1, double dy1, double dx2, double dy2, double dx, double dy)
        {
            WriteCommand('c');
            WriteParameter(dx1);
            WriteParameter(dy1);
            WriteParameter(dx2);
            WriteParameter(dy2);
            WriteParameter(dx);
            WriteParameter(dy);
        }

        public void WriteSmoothCubicBezier(double x2, double y2, double x, double y)
        {
            WriteCommand('S');
            WriteParameter(x2);
            WriteParameter(y2);
            WriteParameter(x);
            WriteParameter(y);
        }

        public void WriteSmoothCubicBezierRelative(double dx2, double dy2, double dx, double dy)
        {
            WriteCommand('s');
            WriteParameter(dx2);
            WriteParameter(dy2);
            WriteParameter(dx);
            WriteParameter(dy);
        }

        public void WriteQuadraticBezier(double x1, double y1, double x, double y)
        {
            WriteCommand('Q');
            WriteParameter(x1);
            WriteParameter(y1);
            WriteParameter(x);
            WriteParameter(y);
        }

        public void WriteQuadraticBezierRelative(double dx1, double dy1, double dx, double dy)
        {
            WriteCommand('q');
            WriteParameter(dx1);
            WriteParameter(dy1);
            WriteParameter(dx);
            WriteParameter(dy);
        }

        public void WriteSmoothQuadraticBezier(double x, double y)
        {
            WriteCommand('T');
            WriteParameter(x);
            WriteParameter(y);
        }

        public void WriteSmoothQuadraticBezierRelative(double dx, double dy)
        {
            WriteCommand('t');
            WriteParameter(dx);
            WriteParameter(dy);
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

        public void WriteClosePath()
        {
            WriteCommand('Z');
        }

        public virtual void Dispose()
        {
        }
    }
}

using System;
using System.Globalization;
using System.Xml;

namespace ScoreHunter.Drawing.Svg.Extensions
{
    public static class XmlWriterExtensions
    {
        public static void WriteAttributeDouble(this XmlWriter writer, string localName, double value, IFormatProvider provider)
        {
            writer.WriteAttributeString(localName, value.ToString(provider));
        }

        public static void WriteAttributeDoubleInvariant(this XmlWriter writer, string localName, double value)
        {
            writer.WriteAttributeDouble(localName, value, CultureInfo.InvariantCulture);
        }

        public static void WriteStartElementRect(this XmlWriter writer, double x, double y, double width, double height)
        {
            writer.WriteStartElement("rect");
            writer.WriteAttributeDoubleInvariant("x", x);
            writer.WriteAttributeDoubleInvariant("y", y);
            writer.WriteAttributeDoubleInvariant("width", width);
            writer.WriteAttributeDoubleInvariant("height", height);
        }
    }
}

using System.Xml;

namespace ScoreHunter.Drawing.Svg.Extensions
{
    public static class XmlWriterExtensions
    {
        public static void WriteAttributeDouble(this XmlWriter writer, string localName, double value)
        {
            writer.WriteStartAttribute(localName);
            writer.WriteValue(value);
            writer.WriteEndAttribute();
        }

        public static void WriteStartElementRect(this XmlWriter writer, double x, double y, double width, double height)
        {
            writer.WriteStartElement("rect");
            writer.WriteAttributeDouble("x", x);
            writer.WriteAttributeDouble("y", y);
            writer.WriteAttributeDouble("width", width);
            writer.WriteAttributeDouble("height", height);
        }
    }
}

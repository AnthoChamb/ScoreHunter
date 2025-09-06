using System.Threading.Tasks;
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

        public static async Task WriteAttributeDoubleAsync(this XmlWriter writer, string localName, double value)
        {
            await writer.WriteAttributeStringAsync(localName, XmlConvert.ToString(value)).ConfigureAwait(false);
        }

        public static void WriteStartElementRect(this XmlWriter writer, double x, double y, double width, double height)
        {
            writer.WriteStartElement("rect");
            writer.WriteAttributeDouble("x", x);
            writer.WriteAttributeDouble("y", y);
            writer.WriteAttributeDouble("width", width);
            writer.WriteAttributeDouble("height", height);
        }

        public static void WriteStartElementUse(this XmlWriter writer, string href)
        {
            writer.WriteStartElement("use");
            writer.WriteAttributeString("href", href);
        }

        public static async Task WriteStartElementUseAsync(this XmlWriter writer, string href)
        {
            await writer.WriteStartElementAsync("use").ConfigureAwait(false);
            await writer.WriteAttributeStringAsync("href", href).ConfigureAwait(false);
        }

        public static void WriteStartElementUse(this XmlWriter writer, string href, double x, double y)
        {
            writer.WriteStartElementUse(href);
            writer.WriteAttributeDouble("x", x);
            writer.WriteAttributeDouble("y", y);
        }

        public static async Task WriteStartElementUseAsync(this XmlWriter writer, string href, double x, double y)
        {
            await writer.WriteStartElementUseAsync(href).ConfigureAwait(false);
            await writer.WriteAttributeDoubleAsync("x", x).ConfigureAwait(false);
            await writer.WriteAttributeDoubleAsync("y", y).ConfigureAwait(false);
        }
    }
}

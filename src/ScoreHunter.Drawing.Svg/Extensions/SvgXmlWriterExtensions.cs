using System.Threading.Tasks;
using System.Xml;

namespace ScoreHunter.Drawing.Svg.Extensions
{
    public static class SvgXmlWriterExtensions
    {
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
            writer.WriteAttributeValue("x", x);
            writer.WriteAttributeValue("y", y);
        }

        public static async Task WriteStartElementUseAsync(this XmlWriter writer, string href, double x, double y)
        {
            await writer.WriteStartElementUseAsync(href).ConfigureAwait(false);
            await writer.WriteAttributeValueAsync("x", x).ConfigureAwait(false);
            await writer.WriteAttributeValueAsync("y", y).ConfigureAwait(false);
        }
    }
}

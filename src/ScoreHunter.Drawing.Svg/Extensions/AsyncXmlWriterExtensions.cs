using System.Threading.Tasks;
using System.Xml;

namespace ScoreHunter.Drawing.Svg.Extensions
{
    public static class AsyncXmlWriterExtensions
    {
        public static async Task WriteStartElementAsync(this XmlWriter writer, string localName)
        {
            await writer.WriteStartElementAsync(null, localName, null).ConfigureAwait(false);
        }

        public static async Task WriteAttributeStringAsync(this XmlWriter writer, string localName, string value)
        {
            await writer.WriteAttributeStringAsync(null, localName, null, value).ConfigureAwait(false);
        }

        public static async Task WriteValueAsync(this XmlWriter writer, double value)
        {
            await writer.WriteStringAsync(XmlConvert.ToString(value)).ConfigureAwait(false);
        }
    }
}

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
    }
}

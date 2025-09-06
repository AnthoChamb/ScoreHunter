using System.Threading.Tasks;
using System.Xml;

namespace ScoreHunter.Drawing.Svg.Extensions
{
    public static class AsyncXmlWriterExtensions
    {
        public static async Task WriteValueAsync(this XmlWriter writer, double value)
        {
            await writer.WriteStringAsync(XmlConvert.ToString(value)).ConfigureAwait(false);
        }
    }
}

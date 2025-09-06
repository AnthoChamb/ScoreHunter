using FsgXmk.Abstractions;
using ScoreHunter.Core.Enums;
using ScoreHunter.Core.Interfaces;
using ScoreHunter.Drawing.Factories;
using ScoreHunter.Drawing.Options;
using ScoreHunter.Drawing.Svg.IO;
using System.IO;
using System.Management.Automation;
using System.Xml;

namespace ScoreHunter.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "ScoreHunterSvg")]
    [OutputType(typeof(string))]
    public class GetScoreHunterSvgCommand : Cmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public ITrack Track { get; set; }

        [Parameter(Position = 1)]
        public IPath Path { get; set; }

        [Parameter]
        public Difficulty Difficulty { get; set; } = Difficulty.Expert;

        [Parameter]
        public int TicksPerStaff = XmkConstants.TicksPerQuarterNote * 4 * 4;

        protected override void ProcessRecord()
        {
            var tablatureOptions = new TablatureOptions(TicksPerStaff);
            var tablatureFactory = new TablatureFactory(tablatureOptions);

            var tablature = tablatureFactory.Create(Track, Difficulty, Path);

            using (var output = new StringWriter())
            using (var xmlWriter = XmlWriter.Create(output))
            using (var writer = new SvgTablatureWriter(xmlWriter, true))
            {
                writer.Write(tablature);
                WriteObject(output.ToString());
            }
        }
    }
}

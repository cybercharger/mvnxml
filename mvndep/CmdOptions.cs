using CommandLine;
using CommandLine.Text;

namespace mvndep
{
    public class CmdOptions
    {
        [Option('i', "input", Required = true, HelpText = "input file by running mvn dependency:tree -Dverbose -DoutputFile=%input%")]
        public string InputFile { get; set; }

        [Option('o', "output", Required = true, HelpText = "Xml out put of parsing mvn dependency")]
        public string OutputFile { get; set; }

        [Option('d', "directDepList", Required = false, DefaultValue = null, HelpText = "plain text file of direct dependency list")]
        public string DirectDepListFile { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, current => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
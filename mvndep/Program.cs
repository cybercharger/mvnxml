
using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using mvndepxml;

namespace mvndep
{
    class Program
    {
        static void Main(string[] args)
        {
            var cmdOptions = new CmdOptions();
            if (!CommandLine.Parser.Default.ParseArguments(args, cmdOptions))
            {
                return;
            }
            GenerateResult(cmdOptions);

        }

        static void GenerateResult(CmdOptions options)
        {
            var root = Node.Load(options.InputFile);
            GenerateXml(root, options.OutputFile);
            if (string.IsNullOrEmpty(options.DirectDepListFile)) return;
            var sb = new StringBuilder();
            foreach (var child in root.Children)
            {
                sb.Append(child.DepInfo + Environment.NewLine);
            }
            File.WriteAllText(options.DirectDepListFile, sb.ToString());
        }

        static void GenerateXml(Node root, string outputFile)
        {
            var doc = XmlGenerator.GenerateDocument(root);
            var settings = new XmlWriterSettings { Indent = true };
            using (var writer = XmlWriter.Create(outputFile, settings))
            {
                doc.Save(writer);
                writer.Flush();
            }
        }
    }
}

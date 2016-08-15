
using System;
using System.IO;
using System.Linq;
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
            if (!string.IsNullOrEmpty(options.DirectDepListFile))
            {
                GenerateDirectDepList(root, options.DirectDepListFile);
            }

            if (!string.IsNullOrEmpty(options.PomDepManagement))
            {
                GeneratePomDepManagementSection(root, options.PomDepManagement);
            }
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

        static void GenerateDirectDepList(Node root, string outputFile)
        {
            var sb = new StringBuilder();
            foreach (var child in root.Children)
            {
                sb.Append(child.DepInfo + Environment.NewLine);
            }
            File.WriteAllText(outputFile, sb.ToString());
        }

        static void GeneratePomDepManagementSection(Node root, string outputFile)
        {
            var doc = new XDocument(
                new XElement("dependencyManagement",
                    new XElement("dependencies", null)));

            var deps = doc.Root.XPathSelectElement("dependencies");

            var settings = new XmlWriterSettings { Indent = true };

            foreach (var depElm in root.Children.Select(child => new XElement("dependency",
                new XElement("groupId", child.DepInfo.GroupId),
                new XElement("artifactId", child.DepInfo.AritifactId),
                new XElement("version", child.DepInfo.Version))))
            {
                deps.Add(depElm);
            }

            using (var writer = XmlWriter.Create(outputFile, settings))
            {
                doc.Save(writer);
                writer.Flush();
            }
        }
    }
}

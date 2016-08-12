using System;
using System.Text;
using System.Xml;

namespace mvndepxml
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1) throw new ArgumentException("please pass in result file of running 'mvn dependency:tree -Dverbose -DoutputFile=%res_file%");
            var result = GenerateXml(args[0]);
            Console.WriteLine(result);
        }

        static string GenerateXml(string fileName)
        {
            var root = Node.Load(fileName);
            var doc = XmlGenerator.GenerateDocument(root);
            var builder = new StringBuilder();
            var settings = new XmlWriterSettings()
            {
                Indent = true,
                IndentChars = "\t",
                Encoding = Encoding.UTF8
            };
            using (var writer = XmlWriter.Create(builder, settings))
            {
                doc.Save(writer);
                writer.Flush();
            }
            return builder.ToString();
        }

    }
}

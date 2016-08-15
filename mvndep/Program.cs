using System;
using System.IO;
using System.Text;
using System.Xml;
using mvndepxml;

namespace mvndep
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
            var settings = new XmlWriterSettings { Indent = true };
            //StringBuilder is always encoded as utf-16, so using a MemoryStream to enforce utf-8 encoding
            using (var memStream = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(memStream, settings))
                {
                    doc.Save(writer);
                    writer.Flush();
                }
                return Encoding.UTF8.GetString(memStream.ToArray());
            }
        }

    }
}

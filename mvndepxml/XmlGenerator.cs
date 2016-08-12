using System;
using System.CodeDom;
using System.Linq;
using System.Xml.Linq;

namespace mvndepxml
{
    public class XmlGenerator
    {
        public const string RootElmName = "component";
        public const string DependencyElmName = "dependency";
        public static XDocument GenerateDocument(Node root)
        {
            return CreateDocument(root);
        }

        private static XDocument CreateDocument(Node root)
        {
            if (root == null) throw new ArgumentNullException("root");
            if (root.Parent != null) throw new ArgumentException("root.Parent != null");
            var doc = new XDocument();
            var rootElm = new XElement(RootElmName, null);
            doc.Add(rootElm);
            var info = new MavenDepInfo(root.Content);
            foreach (var propName in MavenDepInfo.PropNames)
            {
                var value = info.GetProperty(propName);
                if (value == null) continue;
                rootElm.Add(new XAttribute(propName, value));
            }
            GenerateXml(doc.Root, root);
            return doc;
        }

        public static void GenerateXml(XContainer xmlElm, Node currentNode)
        {
            if (currentNode.Children == null || !currentNode.Children.Any()) return;
            foreach (var node in currentNode.Children)
            {
                var xmlChild = new XElement(DependencyElmName, null);
                var info = new MavenDepInfo(node.Content);
                foreach (var propName in MavenDepInfo.PropNames)
                {
                    var value = info.GetProperty(propName);
                    if (value == null) continue;
                    xmlChild.Add(new XAttribute(propName, value));
                }
                xmlChild.Add(new XAttribute("isReference", info.IsReference));
                xmlElm.Add(xmlChild);
                GenerateXml(xmlChild, node);
            }
        }
    }
}
using System;
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
            doc.Add(new XElement(RootElmName, root.Content));
            GenerateXml(doc.Root, root);
            return doc;
        }

        private static void GenerateXml(XContainer xmlElm, Node currentNode)
        {
            if (currentNode.Children == null || !currentNode.Children.Any()) return;
            foreach (var node in currentNode.Children)
            {
                var xmlChild = new XElement(DependencyElmName, node.Content);
                xmlElm.Add(xmlChild);
                GenerateXml(xmlChild, node);
            }
        }
    }
}
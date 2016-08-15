using System;
using System.Linq;
using System.Xml.Linq;

namespace mvndepxml
{
    public class XmlGenerator
    {
        public const string RootElmName = "component";
        public const string DependencyElmName = "dependency";
        public const string IsReferenceAttrName = "isRef";
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
            SetAttributes(rootElm, root.DepInfo);
            GenerateXml(doc.Root, root);
            return doc;
        }

        public static void GenerateXml(XContainer xmlElm, Node currentNode)
        {
            if (currentNode.Children == null || !currentNode.Children.Any()) return;
            foreach (var node in currentNode.Children)
            {
                var xmlChild = new XElement(DependencyElmName, null);
                SetAttributes(xmlChild, node.DepInfo);
                xmlElm.Add(xmlChild);
                GenerateXml(xmlChild, node);
            }
        }

        private static void SetAttributes(XContainer element, MavenDepInfo info)
        {
            foreach (var propName in MavenDepInfo.PropNames)
            {
                var value = info.GetProperty(propName);
                if (value == null) continue;
                element.Add(new XAttribute(propName, value));
            }
            element.Add(new XAttribute(IsReferenceAttrName, info.IsReference));
        }
    }
}
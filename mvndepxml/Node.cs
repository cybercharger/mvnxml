using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace mvndepxml
{
    public class Node
    {
        public MavenDepInfo DepInfo { get; private set; }

        public int Depth { get; private set; }

        public Node Parent { get; private set; }

        public IList<Node> Children { get; private set; }

        private Node(string content, Node parent, int depth)
        {
            if (string.IsNullOrEmpty(content)) throw new ArgumentNullException("content");
            DepInfo = new MavenDepInfo(content);
            Parent = parent;
            Depth = depth;
            Children = new List<Node>();
        }

        public static Node Load(string file)
        {
            return Parse(File.ReadAllLines(file));
        }

        public static Node Parse(string[] mvnDepTreeText)
        {
            return GenerateNode(mvnDepTreeText);
        }

        private static Node GenerateNode(IEnumerable<string> text)
        {
            var parent = new Node("dependencies", null, -1);
            text.Aggregate(parent, (current, s) => Generate(s, current));
            if (parent.Children == null) return null;
            if (parent.Children.Count != 1)
            {
                throw new InvalidDataException(string.Format("more than one component found: {0}", string.Join(", ", parent.Children)));
            }
            parent.Children[0].Parent = null;
            return parent.Children[0];
        }

        private const string ChildIndicator = "+- ";
        private const string LastChildIndicator = "\\- ";
        private static readonly string[] Indents = { ChildIndicator, LastChildIndicator, "|  ", "   " };
        private const int IndicatorLen = 3;

        private static Node Generate(string content, Node previous)
        {
            var currentLine = content;
            var depth = GetDepth(ref currentLine);
            var newParent = previous;
            if (previous == null) throw new InvalidDataException("previous node is null");
            if (depth - previous.Depth > 2)
            {
                throw new InvalidDataException(string.Format("Depth of {0} is {1} where previous {2} depth is {3}",
                    content, depth, previous.DepInfo, previous.Depth));
            }
            if (depth == previous.Depth)
            {
                newParent = previous.Parent;
            }
            else if (depth == previous.Depth + 1)
            {
                newParent = previous;
            }
            else
            {
                for (var i = previous.Depth - depth + 1; i > 0; --i)
                {
                    newParent = newParent.Parent;
                }
            }
            var newNode = new Node(currentLine, newParent, depth);
            newParent.Children.Add(newNode);
            return newNode;
        }

        private static int GetDepth(ref string content)
        {
            var depth = 0;
            for (; Indents.Any(content.StartsWith); content = content.Substring(IndicatorLen), ++depth) ;
            return depth;
        }

        public override string ToString()
        {
            return DepInfo.ToString();
        }

        public void Traverse(Action<Node> action)
        {
            if (action == null) throw new ArgumentNullException("action");
            RecursivelyTraverse(action, this);
        }

        private static void RecursivelyTraverse(Action<Node> action, Node current)
        {
            if (current == null) throw new ArgumentNullException("current");
            action(current);
            if (current.Children == null || !current.Children.Any()) return;
            foreach (var child in current.Children)
            {
                RecursivelyTraverse(action, child);
            }
        }
    }
}
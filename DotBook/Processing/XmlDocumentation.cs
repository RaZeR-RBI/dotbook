using DotBook.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using static DotBook.Logger;
using System.Text.RegularExpressions;

namespace DotBook.Processing
{
    public class XmlDocumentation
    {
        public IEnumerable<XmlNode> Nodes { get; }

        private static List<(string start, string end)> escapeAnchors =
            new List<(string start, string end)>()
            {
                ("<c>", @"</c>"),
                ("<code>", @"</code>"),
                (" cref=\"", "\"")
            };

        private static List<(string src, string replacement)> escapedSymbols =
            new List<(string src, string replacement)>()
            {
                ("<", "&lt;"),
                (">", "&gt;"),
            };

        /// TODO: Add symbol escaping
        public XmlDocumentation(string source)
        {
            if (source == null) source = "";
            var header = "<?xml version='1.0' encoding='UTF-8' ?>";
            var rootOpen = "<root>";
            var rootClose = "</root>";

            var docSource = $"{header}{rootOpen}{Escape(source)}{rootClose}";

            var doc = new XmlDocument();
            doc.LoadXml(docSource);

            var root = doc.SelectSingleNode("root");
            if (!root.HasChildNodes)
            {
                Nodes = Enumerable.Empty<XmlNode>();
                return;
            }

            if (root.FirstChild.NodeType == XmlNodeType.Text)
            {
                var summary = doc.CreateElement("summary");
                summary.InnerText = source;
                root.RemoveAll();
                root.AppendChild(summary);
            }

            Nodes = root.ChildNodes.Cast<XmlNode>();
        }

        private string Escape(string source)
        {
            foreach (var anchors in escapeAnchors)
                source = Replace(source, anchors, escapedSymbols);
            return source;
        }

        private string Replace(string input, 
            (string from, string to) anchors,
            IEnumerable<(string from, string to)> replacements)
        {
            var startIndex = input.IndexOf(anchors.from);
            while (startIndex != -1)
            {
                startIndex += anchors.from.Length;
                var endIndex = input.IndexOf(anchors.to, startIndex + 1);
                if (endIndex == -1) break;
                var length = endIndex - startIndex;
                var fragment = input.Substring(startIndex, length);
                foreach(var replacement in replacements)
                    fragment = fragment.Replace(replacement.from, replacement.to);
                
                input = input.Substring(0, startIndex) + fragment + input.Substring(endIndex);
                startIndex = input.IndexOf(anchors.from, endIndex);
            }
            return input;
        }

        public Optional<XmlNode> GetSummary() =>
            Optional.Of(Nodes.FirstOrDefault(n => n.Name == "summary"));

        public Optional<XmlNode> GetRemarks() =>
            Optional.Of(Nodes.FirstOrDefault(n => n.Name == "remarks"));

        public IEnumerable<XmlNode> GetExamples() =>
            Nodes.Where(n => n.Name == "example");

        public IEnumerable<(string name, XmlNode root)> GetExceptions() =>
            Nodes.Where(n => n.Name == "exception")
                .Select(n => (n.AttributeValue("cref"), n));

        public IEnumerable<(string name, XmlNode root)> GetParameters() =>
            Nodes.Where(n => n.Name == "param")
                .Select(n => (n.AttributeValue("name"), n));

        public IEnumerable<(string name, XmlNode root)> GetTypeParameters() =>
            Nodes.Where(n => n.Name == "typeparam")
                .Select(n => (n.AttributeValue("name"), n));

        public IEnumerable<string> GetSeeAlso() =>
            Nodes.Where(n => n.Name == "seealso")
                .Select(n => n.AttributeValue("cref"))
                .Where(n => n != null);

        public Optional<XmlNode> GetValue() =>
            Optional.Of(Nodes.FirstOrDefault(n => n.Name == "value"));

        public Optional<XmlNode> GetReturns() =>
            Optional.Of(Nodes.FirstOrDefault(n => n.Name == "returns"));
    }
}

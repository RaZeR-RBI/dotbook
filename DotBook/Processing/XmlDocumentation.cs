using DotBook.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using static DotBook.Logger;

namespace DotBook.Processing
{
    public class XmlDocumentation
    {
        public IEnumerable<XmlNode> Nodes { get; }

        public XmlDocumentation(string source)
        {
            if (source == null) source = "";
            var header = "<?xml version='1.0' encoding='UTF-8' ?>";
            var rootOpen = "<root>";
            var rootClose = "</root>";

            var docSource = $"{header}{rootOpen}{source}{rootClose}";

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

        public Optional<XmlNode> GetSummary() =>
            Optional.Of(Nodes.FirstOrDefault(n => n.Name == "summary"));

        public IEnumerable<XmlNode> GetExamples() =>
            Nodes.Where(n => n.Name == "example");

        public IEnumerable<XmlNode> GetExceptions() =>
            Nodes.Where(n => n.Name == "exception");

        public IEnumerable<(string name, string desc)> GetParameters() =>
            Nodes.Where(n => n.Name == "param")
                .Select(n => {
                    var name = n.Attributes.OfType<XmlAttribute>()
                        .FirstOrDefault(a => a.Name == "name")?
                        .InnerText;
                    var desc = n.InnerText;
                    return (name, desc);
                });
    }
}

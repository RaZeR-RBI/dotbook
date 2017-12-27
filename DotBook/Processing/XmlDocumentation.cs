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

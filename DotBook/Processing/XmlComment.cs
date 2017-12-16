using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace DotBook.Processing
{
    public class XmlDocumentation
    {
        public IEnumerable<XmlNode> Nodes { get; }

        public XmlDocumentation(string source)
        {
            var header = "<?xml version='1.0' encoding='UTF-8' ?>";
            var rootOpen = "<root>";
            var rootClose = "</root>";

            var docSource = $"{header}{rootOpen}{source}{rootClose}";

            var doc = new XmlDocument();
            doc.LoadXml(docSource);

            var root = doc.SelectSingleNode("root");

            if (root.FirstChild.NodeType == XmlNodeType.Text)
            {
                var summary = doc.CreateElement("summary");
                summary.InnerText = source;
                root.RemoveAll();
                root.AppendChild(summary);
            }

            Nodes = root.ChildNodes.Cast<XmlNode>();
        }
    }
}

using DotBook.Processing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Xunit;

namespace DotBook.Tests.Processing
{
    public class XmlDocumentationTest
    {
        private XmlDocumentation Act(string docstring) =>
            new XmlDocumentation(docstring);

        [Fact]
        public void ShouldParseXmlNodes()
        {
            var expected = "This is a XML comment";
            var comment = $@"
                <summary>{expected}</summary>
            ";

            var nodes = Act(comment).Nodes;

            Assert.Single(nodes);
            Assert.Contains(nodes, n =>
                n.InnerText == expected && n.Name == "summary");
        }

        [Fact]
        public void ShouldFallbackToSummaryIfPlaintext()
        {
            var nonXmlComment = "Hello, world";

            var nodes = Act(nonXmlComment).Nodes;

            Assert.Single(nodes);
            Assert.Contains(nodes, n =>
                n.InnerText == nonXmlComment && n.Name == "summary");
        }
    }
}

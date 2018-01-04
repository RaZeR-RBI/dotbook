using DotBook.Processing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Web;
using Xunit;
using System.Linq;
using static System.Web.HttpUtility;

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

        [Fact]
        public void ShouldReturnSummaryIfPresent()
        {
            var withoutSummary = true;
            var withSummary = false;

            var commentWithoutSummary = Act("<returns>Something</returns>")
                .GetSummary();
            var commentWithSummary = Act("<summary>Something</summary>")
                .GetSummary();
            commentWithoutSummary.IfPresent(n => withoutSummary = false);
            commentWithSummary.IfPresent(n => withSummary = true);

            Assert.True(withoutSummary);
            Assert.True(withSummary);
        }

        [Fact]
        public void ShouldEscapeCodeSymbols()
        {
            var summary = "Something. See <see cref=\"MyType<T>\" />";
            var example = "<code>var obj = new MyType<int>()</code>";
            var see = "<seealso cref=\"MyType<T>\"/>";
            var hasSummary = false;

            var source = $"<summary>{summary}</summary>" +
                $"<example>{example}</example>" +
                see;
            
            var doc = Act(source);

            doc.GetSummary().IfPresent(s => {
                hasSummary = true;
                Assert.Equal(summary, HtmlDecode(s.InnerXml));
            });
            Assert.True(hasSummary);
            var examples = doc.GetExamples();
            var seealso = doc.GetSeeAlso();
            Assert.Single(examples);
            Assert.Equal(example, HtmlDecode(examples.First().InnerXml));
            Assert.Single(seealso);
            Assert.Equal("MyType<T>", seealso.First());
        }
    }
}

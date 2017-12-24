using DotBook.Model;
using DotBook.Processing;
using System;
using System.Collections.Generic;
using System.Text;
using DotBook.Model.Entities;
using static DotBook.Logger;
using System.Xml;
using Simplicity;
using System.Linq;
using DotBook.Model.Members;
using static DotBook.Processing.EntityTypeResolver;

namespace DotBook.Backend
{
    internal abstract class StringFormatterBase : IFormatter<string>
    {
        [Flags]
        protected enum TextStyle
        {
            Normal = 0b000,
            Bold = 0b001,
            Italic = 0b010
        }

        protected enum ListStyle
        {
            Bullet, Ordered
        }

        private StringBuilder builder = new StringBuilder();

        protected void Start() => builder = new StringBuilder();
        protected string Result() => builder.ToString();

        protected void Write(string text) => builder.Append(text);
        protected void WriteLine(string text = "") => builder.AppendLine(text);

        protected abstract StringFormatterBase Header(string title, int level = 1);
        protected abstract StringFormatterBase HorizontalRule();
        protected abstract StringFormatterBase ParagraphStart();
        protected abstract StringFormatterBase ParagraphEnd();
        protected abstract StringFormatterBase Text(string text,
            TextStyle style = TextStyle.Normal);
        protected abstract StringFormatterBase List(IEnumerable<string> items,
            ListStyle style = ListStyle.Bullet);
        protected abstract StringFormatterBase Link(string title, string url);
        protected abstract StringFormatterBase Image(string url, string title = "");
        protected abstract StringFormatterBase Table(List<string> header,
            List<List<string>> rows);
        protected abstract StringFormatterBase CodeInline(string code);
        protected abstract StringFormatterBase Code(string code);

        public string Process(Entity entity)
        {
            Start();
            var item = entity.Base;
            var name = item.Name;
            Header(name);

            // Entity type (method, class, etc.)
            ParagraphStart()
            .Text($"{Resolve(item)}", TextStyle.Bold)
            .ParagraphEnd();

            var @namespace = item.AncestorOfType<INameable, NamespaceInfo>();
            @namespace.IfPresent(ns =>
                ParagraphStart()
                .Text("Namespace:", TextStyle.Bold)
                .Text(" ")
                .Link($"{ns.FullName}", entity.GetLink(ns.FullName))
                .ParagraphEnd()
            );

            HorizontalRule();

            var doc = new XmlDocumentation(item.Documentation);
            doc.GetSummary()
                .IfPresent(summary =>
                    ParagraphStart()
                    .ContentsOf(summary)
                    .ParagraphEnd())
                .IfNone(() => {
                    Warning($"{entity.FullName} is missing summary");
                    .ParagraphStart()
                    .Text("No description provided", TextStyle.Italic)
                    .ParagraphEnd();
                });
            
            item.MaybeIs<IDocumentationNode, MethodInfoBase>()
                .IfPresent(m =>
                {
                    Header("Syntax", 2)
                        .Code(m.Syntax);
                    if (doc.GetParameters().Any())
                        Header("Parameters", 3);
                    doc.GetParameters().ForEach(p => {
                        if (p.name == null) 
                            Warning($"Missing param name: {name}");
                        else
                            ParagraphStart()
                            .CodeInline(p.name)
                            .ParagraphEnd()
                            .ParagraphStart()
                            .Text(p.desc)
                            .ParagraphEnd();
                    });
                });

            doc.GetExamples()
                .IfAny(() => Header("Examples", 2))
                .ForEach(example =>
                    ParagraphStart()
                    .ContentsOf(example)
                    .ParagraphEnd());

            return Result();
        }

        private StringFormatterBase ProcessNode(XmlNode node)
        {
            node.Match()
                .With(x => x.NodeType == XmlNodeType.Text, x => Text(x.InnerText))
                .With(x => x.Name == "c", x => CodeInline(x.InnerText))
                .With(x => x.Name == "code", x => Code(x.InnerText))
                .With(x => x.Name == "para", x => 
                    ParagraphStart()
                    .ContentsOf(x)
                    .ParagraphEnd())
                .Do();
            return this;
        }

        private StringFormatterBase ContentsOf(XmlNode node)
        {
            if (!node.HasChildNodes) return this;
            foreach (XmlNode child in node.ChildNodes) ProcessNode(child);
            return this;
        }
    }
}

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
        private Entity entity = null;

        protected void Start(Entity entity) =>
            (builder, this.entity) = (new StringBuilder(), entity);

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
            Start(entity);
            var item = entity.Base;
            var name = item.Name;
            var fullName = item.FullName;
            // Name
            Header(name);

            // Entity type (method, class, etc.)
            ParagraphStart()
            .Text($"{Resolve(item)}", TextStyle.Bold)
            .ParagraphEnd();

            // Namespace
            var @namespace = item.AncestorOfType<INameable, NamespaceInfo>();
            @namespace.IfPresent(ns =>
                ParagraphStart()
                .Text("Namespace:", TextStyle.Bold)
                .Text(" ")
                .MemberLink(ns.FullName)
                .ParagraphEnd()
            );

            HorizontalRule();

            // Summary
            var doc = new XmlDocumentation(item.Documentation);
            doc.GetSummary()
                .IfPresent(p => ParagraphFrom(p))
                .IfNone(() =>
                {
                    Warning($"{entity.FullName} is missing summary");
                    ParagraphStart()
                    .Text("No description provided", TextStyle.Italic)
                    .ParagraphEnd();
                });

            // Additional info if it's a method, constructor or operator
            item.MaybeIs<IDocumentationNode, MethodInfoBase>()
                .IfPresent(m =>
                {
                    Header("Syntax", 2)
                        .Code(m.Syntax);

                    doc.GetParameters()
                        .IfAny(() => Header("Parameters", 3))
                        .ForEach(p => PrintParameterInfo(fullName, "parameter", p));

                    doc.GetReturns()
                        .IfPresent(n =>
                            Header("Returns", 3)
                            .ContentsOf(n));
                });

            item.MaybeIs<IDocumentationNode, PropertyInfo>()
                .IfPresent(p => doc.GetValue().IfPresent(n =>
                        Header("Value")
                        .ParagraphFrom(n)
                    ));

            doc.GetTypeParameters()
                .IfAny(() => Header("Type parameters", 3))
                .ForEach(p => PrintParameterInfo(fullName, "type parameter", p));

            doc.GetExceptions()
                .IfAny(() => Header("Exceptions", 3))
                .ForEach(p => PrintParameterInfo(fullName, "exception", p));

            doc.GetRemarks().IfPresent(s =>
                Header("Remarks", 2)
                .ContentsOf(s));

            doc.GetExamples()
                .IfAny(() => Header("Examples", 2))
                .ForEach(p => ParagraphFrom(p));

            doc.GetSeeAlso()
                .IfAny(() => Header("See also", 2))
                .ForEach(s => ParagraphStart()
                         .MemberLink(s)
                         .ParagraphEnd());

            return Result();
        }

        private StringFormatterBase ProcessNode(XmlNode node)
        {
            node.Match()
                .With(x => x.NodeType == XmlNodeType.Text, x => Text(x.InnerText))
                .With(x => x.Name == "c", x => CodeInline(x.InnerText))
                .With(x => x.Name == "code", x => Code(x.InnerText))
                .With(x => x.Name == "para", ParagraphFrom)
                .With(x => x.Name == "paramref" || x.Name == "typeparamref",
                    x => CodeInline(x.AttributeValue("name"))
                )
                .With(x => x.Name == "see",
                    x => MemberLink(x.AttributeValue("cref"))
                )
                .Do();
            return this;
        }

        private StringFormatterBase ContentsOf(XmlNode node)
        {
            if (!node.HasChildNodes) return this;
            foreach (XmlNode child in node.ChildNodes) ProcessNode(child);
            return this;
        }

        private void PrintParameterInfo(string name, string title,
            (string name, XmlNode root) p)
        {
            if (p.name == null)
                Warning($"Missing {title.ToLower()} name: {name}");
            else
                ParagraphStart()
                .CodeInline(p.name)
                .ParagraphEnd()
                .ParagraphFrom(p.root);
        }

        private StringFormatterBase ParagraphFrom(XmlNode node) =>
            ParagraphStart()
            .ContentsOf(node)
            .ParagraphEnd();

        private StringFormatterBase MemberLink(string memberName)
        {
            if (memberName == null || memberName == "") return this;
            Link(memberName, entity.GetLink(memberName));
            return this;
        }
    }
}

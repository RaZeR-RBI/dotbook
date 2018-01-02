﻿using DotBook.Model;
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

        protected string Result() => builder.ToString().Trim();

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
        protected abstract StringFormatterBase LinkListItem(string title, string url);
        protected abstract StringFormatterBase LinkList(IDictionary<string, string> links);
        protected abstract StringFormatterBase Image(string url, string title = "");
        protected abstract StringFormatterBase Table(List<string> header,
            List<List<string>> rows);
        protected abstract StringFormatterBase CodeInline(string code);
        protected abstract StringFormatterBase Code(string code);

        protected abstract string Extension { get; }

        public string Process(Entity entity, IEnumerable<Modifier> visibilities)
        {
            Start(entity);

            if (entity.IsRoot())
            {
                Header("Namespaces");
                var links = new Dictionary<string, string>();
                entity.ChildrenNodes.ForEach(c =>
                {
                    var child = c as Entity;
                    links.Add(child.FullName, child.Link + Extension);
                });
                LinkList(links);
                return Result();
            }

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

            // Base types
            item.MaybeIs<IDocumentationNode, IDerivable>()
                .IfPresent(t =>
                    ParagraphStart()
                    .Text("Base types:", TextStyle.Bold)
                    .ParagraphEnd()
                    .MemberLinks(t.BaseTypes));


            // Parent member container (class, struct, etc.) if it's a member
            if (item.ParentNode is IMemberContainer)
                ParagraphStart()
                .Text("Member of:", TextStyle.Bold)
                .Text(" ")
                .Link((item.ParentNode as IMemberContainer).FullName,
                    (entity.ParentNode as Entity).Link)
                .ParagraphEnd();

            // Parent type container (namespace or other type) if it's a type
            if (item.ParentNode is ITypeContainer)
                ParagraphStart()
                .Text("Declared in:", TextStyle.Bold)
                .Text(" ")
                .Link((item.ParentNode as ITypeContainer).FullName,
                    (entity.ParentNode as Entity).Link)
                .ParagraphEnd();

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

            // If it's a method, constructor, operator or indexer
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
                            .ParagraphFrom(n));
                });

            // If it is a property, print info about it's value (if defined)
            item.MaybeIs<IDocumentationNode, PropertyInfo>()
                .IfPresent(p => doc.GetValue().IfPresent(n =>
                        Header("Value")
                        .ParagraphFrom(n)
                    ));

            // Type parameters
            doc.GetTypeParameters()
                .IfAny(() => Header("Type parameters", 3))
                .ForEach(p => PrintParameterInfo(fullName, "type parameter", p));

            // Print if it throws anything
            doc.GetExceptions()
                .IfAny(() => Header("Exceptions", 3))
                .ForEach(p => PrintParameterInfo(fullName, "exception", p));

            // Print type members if it's a type
            item.MaybeIs<IDocumentationNode, IMemberContainer>()
                .IfPresent(c => PrintChildrenInfo("Members", c.Members, visibilities));

            // Print nested types if they exist
            item.MaybeIs<IDocumentationNode, ITypeContainer>()
                .IfPresent(c => PrintChildrenInfo("Nested types", c.Types, visibilities));

            doc.GetRemarks().IfPresent(s =>
                Header("Remarks", 2)
                .ContentsOf(s));

            doc.GetExamples()
                .IfAny(() => Header("Examples", 2))
                .ForEach(p => ParagraphFrom(p));

            doc.GetSeeAlso()
                .IfAny(() => Header("See also", 2))
                .ForEach(m => MemberLinkList(m));

            HorizontalRule()
                .Link("Back to index", "index" + Extension);

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

        private void PrintChildrenInfo<T>(string header, IEnumerable<T> items,
            IEnumerable<Modifier> visibilities)
            where T : class, INameable
        {
            if (items == null) return;
            var visibleItems = items.Where(member =>
            {
                var result = true;
                member.MaybeIs<T, IModifiable>()
                    .IfPresent(m =>
                        result = m.Modifiers.Intersect(visibilities).Any());
                return result;
            });
            if (!visibleItems.Any()) return;

            Header(header, 2);
            var groups = visibleItems.GroupBy(m => Resolve(m));
            groups.ForEach(group =>
            {
                Header($"{group.Key.ToString()}", 3);
                group.ForEach(m => MemberLinkList(m.Name));
            });
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
            Link(memberName, entity.GetLink(memberName) + Extension);
            return this;
        }

        private StringFormatterBase MemberLinkList(string memberName)
        {
            if (memberName == null || memberName == "") return this;
            LinkListItem(memberName, entity.GetLink(memberName) + Extension);
            return this;
        }

        private StringFormatterBase MemberLinks(IEnumerable<string> names) =>
            LinkList(names.ToDictionary(name => name,
                name => entity.GetLink(name) + Extension));
    }
}

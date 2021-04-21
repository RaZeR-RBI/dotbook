﻿using DotBook.Model;
using DotBook.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using static System.Linq.Enumerable;

namespace DotBook.Backend
{
    internal sealed class MarkdownFormatter : StringFormatterBase
    {
        public override string Extension => ".md";

        protected override StringFormatterBase Header(string title, int level = 1)
        {
            WriteLine("\n" + " ".PadLeft(level + 1, '#') + title);
            return this;
        }

        protected override StringFormatterBase HorizontalRule()
        {
            WriteLine("\n------\n");
            return this;
        }

        protected override StringFormatterBase ParagraphStart()
        {
            WriteLine();
            return this;
        }

        protected override StringFormatterBase ParagraphEnd()
        {
            WriteLine();
            return this;
        }

        protected override StringFormatterBase Text(string text,
            TextStyle style = TextStyle.Normal)
        {
            var result = text;
            if (style.HasFlag(TextStyle.Bold)) result = $"**{result}**";
            if (style.HasFlag(TextStyle.Italic)) result = $"*{result}*";
            Write(result);
            return this;
        }

        protected override StringFormatterBase List(IEnumerable<string> items,
            ListStyle style = ListStyle.Bullet)
        {
            int i = 1;
            WriteLine();
            foreach (var item in items)
            {
                Write(style == ListStyle.Bullet ? "* " : $"{i}. ");
                WriteLine(item);
                i++;
            }
            WriteLine();
            return this;
        }

        protected override StringFormatterBase Link(string title, string url)
        {
            Write($"[{Escape(title)}]({url})");
            return this;
        }

        protected override StringFormatterBase Image(string url, string title = "")
        {
            Write($"![{Escape(title)}]({url})");
            return this;
        }

        protected override StringFormatterBase Table(List<string> header,
            List<List<string>> rows)
        {
            WriteLine($"|{string.Join("|", header)}|");
            WriteLine($"|{Repeat("-|", header.Count)}");
            foreach (var row in rows)
                WriteLine($"|{string.Join("|", row)}|");
            return this;
        }

        protected override StringFormatterBase CodeInline(string code)
        {
            Write($"`{code}`");
            return this;
        }

        protected override StringFormatterBase Code(string code)
        {
            Write($"\n```csharp\n{code}\n```\n");
            return this;
        }

        protected override StringFormatterBase LinkList(IDictionary<string, string> links)
        {
            WriteLine();
            foreach (var pair in links)
            {
                LinkListItem(pair.Key, pair.Value);
            }
            WriteLine();
            return this;
        }

        protected override StringFormatterBase LinkListItem(string title, string url)
        {
            Write("* ");
            Link(title, url);
            WriteLine();
            return this;
        }

        protected override StringFormatterBase LinkTree<T>(INode<T> root,
            Func<T, (string title, string url)> builder)
        {
            WriteLine();
            LinkTreeItem(root, builder);
            WriteLine();
            return this;
        }

        private void LinkTreeItem<T>(INode<T> node,
            Func<T, (string title, string url)> builder,
            int level = 1)
        {
            Write("".PadLeft((level - 1) * 4, ' '));
            var link = builder(node.NodeValue);
            LinkListItem(link.title, link.url + Extension);
            if (node.ChildrenNodes == null) return;
            foreach (var child in node.ChildrenNodes)
                LinkTreeItem(child, builder, level + 1);
        }

        private string Escape(string str) => HttpUtility.HtmlEncode(str);
    }
}

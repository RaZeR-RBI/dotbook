using DotBook.Model;
using DotBook.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static System.Linq.Enumerable;

namespace DotBook.Backend.Markdown
{
    internal abstract class MarkdownFormatterBase<TIn> : StringFormatterBase<TIn>
        where TIn : INameable, IDocumentable, INode<INameable>
    {
        protected override StringFormatterBase<TIn> Header(string title, int level = 1)
        {
            WriteLine(" ".PadLeft(level + 1, '#') + title);
            return this;
        }

        protected override StringFormatterBase<TIn> HorizontalRule()
        {
            WriteLine("------");
            return this;
        }

        protected override StringFormatterBase<TIn> ParagraphStart()
        {
            WriteLine();
            return this;
        }

        protected override StringFormatterBase<TIn> ParagraphEnd()
        {
            WriteLine();
            return this;
        }

        protected override StringFormatterBase<TIn> Text(string text,
            TextStyle style = TextStyle.Normal)
        {
            var result = text;
            if (style.HasFlag(TextStyle.Bold)) result = $"**{result}**";
            if (style.HasFlag(TextStyle.Italic)) result = $"*{result}*";
            Write(result);
            return this;
        }

        protected override StringFormatterBase<TIn> List(IEnumerable<string> items,
            ListStyle style = ListStyle.Bullet)
        {
            int i = 1;
            WriteLine();
            foreach(var item in items)
            {
                Write(style == ListStyle.Bullet ? "* " : $"{i}. ");
                WriteLine(item);
                i++;
            }
            WriteLine();
            return this;
        }

        protected override StringFormatterBase<TIn> Link(string title, string url)
        {
            Write($"[{title}]({url})");
            return this;
        }

        protected override StringFormatterBase<TIn> Image(string url, string title = "")
        {
            Write($"![{title}]({url})");
            return this;
        }

        protected override StringFormatterBase<TIn> Table(List<string> header,
            List<List<string>> rows)
        {
            WriteLine($"|{string.Join('|', header)}|");
            WriteLine($"|{Repeat("-|", header.Count)}");
            foreach(var row in rows)
                WriteLine($"|{string.Join('|', row)}|");
            return this;
        }

        protected override StringFormatterBase<TIn> CodeInline(string code)
        {
            Write($"`{code}`");
            return this;
        }

        protected override StringFormatterBase<TIn> Code(string code)
        {
            Write($"```csharp\n{code}\n```");
            return this;
        }
    }
}

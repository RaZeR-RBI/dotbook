using DotBook.Model;
using DotBook.Processing;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBook.Backend
{
    internal abstract class StringFormatterBase<TIn> : IFormatter<string, TIn>
        where TIn : INameable, IDocumentable, INode<INameable>
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

        protected void Write(string text) => builder.Append(text);
        protected void WriteLine(string text = "") => builder.AppendLine(text);

        protected abstract StringFormatterBase<TIn> Header(string title, int level = 1);
        protected abstract StringFormatterBase<TIn> HorizontalRule();
        protected abstract StringFormatterBase<TIn> ParagraphStart();
        protected abstract StringFormatterBase<TIn> ParagraphEnd();
        protected abstract StringFormatterBase<TIn> Text(string text, 
            TextStyle style = TextStyle.Normal);
        protected abstract StringFormatterBase<TIn> List(IEnumerable<string> items,
            ListStyle style = ListStyle.Bullet);
        protected abstract StringFormatterBase<TIn> Link(string title, string url);
        protected abstract StringFormatterBase<TIn> Image(string url, string title = "");
        protected abstract StringFormatterBase<TIn> Table(List<string> header,
            List<List<string>> rows);
        protected abstract StringFormatterBase<TIn> CodeInline(string code);
        protected abstract StringFormatterBase<TIn> Code(string code);

        public abstract string Process(TIn entity);
    }
}

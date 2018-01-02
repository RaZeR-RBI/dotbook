using System;

namespace DotBook.Backend
{
    public enum FileFormat
    {
        Markdown, Html
    }

    public static class FileWriters
    {
        public static FileWriter Markdown(string baseFolder) =>
            new FileWriter(new MarkdownFormatter(), ".md", baseFolder);

        public static FileWriter Html(string baseFolder) =>
            new FileWriter(new HtmlFormatter(), ".html", baseFolder);

        public static FileWriter BeginWritingAt(this FileFormat format, string baseFolder)
        {
            switch(format)
            {
                case FileFormat.Markdown: return Markdown(baseFolder);
                case FileFormat.Html: return Html(baseFolder);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
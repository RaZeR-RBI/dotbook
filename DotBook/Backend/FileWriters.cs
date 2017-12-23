using DotBook.Backend.Markdown;

namespace DotBook.Backend
{
    public static class FileWriters
    {
        public static FileWriter Markdown(string baseFolder) =>
            new FileWriter(new MarkdownFormatter(), ".md", baseFolder);
    }
}
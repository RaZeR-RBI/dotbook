using DotBook.Model.Members;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotBook.Utils
{
    public static class Common
    {
        public static string Format(TypeParameterListSyntax tpl) =>
            tpl != null ? $"<{string.Join(", ", tpl.Parameters)}>" : "";

        public static SortedSet<string> Parse(BaseListSyntax syntax) =>
            new SortedSet<string>(syntax?.Types.Select(t => t.ToString()) ??
                Enumerable.Empty<string>());

        public static List<ParameterInfo> Parse(ParameterListSyntax syntax) =>
            syntax?.Parameters.Select(p => new ParameterInfo(p)).ToList() ??
                new List<ParameterInfo>();

        public static List<ParameterInfo> Parse(BracketedParameterListSyntax syntax) =>
            syntax?.Parameters.Select(p => new ParameterInfo(p)).ToList() ??
                new List<ParameterInfo>();

        public static string JoinWithDelimiter(this IEnumerable<string> input,
            string delimiter) => string.Join(delimiter, input);

        public static string GetDocumentation(SyntaxTriviaList trivia) =>
            trivia.Select(i => i.GetStructure())
                .OfType<DocumentationCommentTriviaSyntax>()
                .FirstOrDefault()?.Content.ToString()
                .Split(new string[]{ Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Select(s => s.StartsWith("///") ? s.Remove(0, 3).Trim() : s)
                .JoinWithDelimiter(Environment.NewLine);
    }
}

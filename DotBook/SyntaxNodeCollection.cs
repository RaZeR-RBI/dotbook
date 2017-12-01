using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static DotBook.Logger;

namespace DotBook
{
    public class SyntaxNodeCollection
    {
        private static Func<string, bool> allFiles = s => true;

        public static IEnumerable<SyntaxNode> FromFolder(string folder,
            Func<string, bool> filenamePredicate = null) =>
            Directory.EnumerateFiles(folder, "*.cs", SearchOption.AllDirectories)
            .Where(filenamePredicate ?? allFiles)
            .Select(Parse);

        private static SyntaxNode Parse(string file)
        {
            Log($"Parsing file '{file}'");
            return CSharpSyntaxTree.ParseText(File.ReadAllText(file)).GetRoot();
        }
    }
}

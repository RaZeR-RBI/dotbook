using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static DotBook.Logger;

namespace DotBook
{
    public class CompilationUnits
    {
        private static Func<string, bool> allFiles = s => true;

        public static IEnumerable<CompilationUnitSyntax> FromFolder(string folder,
            Func<string, bool> filenamePredicate = null) =>
            Directory.EnumerateFiles(folder, "*.cs", SearchOption.AllDirectories)
            .Where(filenamePredicate ?? allFiles)
            .Select(ParseFile);

        public static IEnumerable<CompilationUnitSyntax> FromString(params string[] sources) =>
            sources.Select(ParseString);

        private static CompilationUnitSyntax ParseString(string input) =>
            CSharpSyntaxTree.ParseText(input).GetRoot() as CompilationUnitSyntax;

        private static CompilationUnitSyntax ParseFile(string file)
        {
            Log($"Parsing file '{file}'");
            return ParseString(File.ReadAllText(file));
        }
    }
}

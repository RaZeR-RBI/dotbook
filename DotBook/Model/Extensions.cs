using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotBook.Model
{
    public static class Extensions
    {

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source,
            IEqualityComparer<T> comparer = null) => new HashSet<T>(source, comparer);

        public static void AddOrReuse<T1, T2>(this T2 info, T1 decl, HashSet<T2> set)
            where T2 : IPartial<T1>
        {
            if (set == null) return;
            set.Add(info);
            var savedInfo = set.Where(c => c.Equals(info)).First();
            savedInfo.Populate(decl);
        }

        public static Modifier AsModifierEnum(this SyntaxToken token) =>
            Enum.Parse<Modifier>(token.Text.FirstCharToUpper());

        public static HashSet<Modifier> ParseModifiers(this SyntaxTokenList tokens)
        {
            var result = tokens.Select(m => m.AsModifierEnum())
                .ToHashSet();
            if (result.Count == 0) result.Add(Modifier.Private);
            return result;
        }

        public static void AddAsChild(this INameable parent, 
            MemberDeclarationSyntax s,
            HashSet<ClassInfo> classes,
            HashSet<StructInfo> structs,
            HashSet<InterfaceInfo> interfaces,
            HashSet<EnumInfo> enums)
        {
            // ugly but works
            switch (s)
            {
                case ClassDeclarationSyntax classDecl:
                    new ClassInfo(classDecl, parent).AddOrReuse(classDecl, classes);
                    break;
                case EnumDeclarationSyntax enumDecl:
                    if (enums != null) enums.Add(new EnumInfo(enumDecl, parent));
                    break;
                case StructDeclarationSyntax structDecl:
                    new StructInfo(structDecl, parent).AddOrReuse(structDecl, structs);
                    break;
                case InterfaceDeclarationSyntax intDecl:
                    new InterfaceInfo(intDecl, parent).AddOrReuse(intDecl, interfaces);
                    break;
            }
        }

        public static string FirstCharToUpper(this string input)
        {
            switch (input)
            {
                case null: throw new ArgumentNullException(nameof(input));
                case "": throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
                default: return input.First().ToString().ToUpper() + input.Substring(1);
            }
        }
    }
}

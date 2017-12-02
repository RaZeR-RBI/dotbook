using Microsoft.CodeAnalysis;
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

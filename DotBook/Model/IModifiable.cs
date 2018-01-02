using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotBook.Model
{
    public enum Modifier
    {
        Public,
        Private,
        Internal,
        Protected,
        Abstract,
        Async,
        Const,
        Delegate,
        Event,
        Extern,
        Explicit,
        Fixed,
        Implicit,
        New,
        Override,
        Partial,
        Readonly,
        Sealed,
        Static,
        Unsafe,
        Virtual,
        Volatile
    }

    public interface IModifiable
    {
        IReadOnlyCollection<Modifier> Modifiers { get; }
    }

    public static class Modifiers
    {
        private static SortedSet<Modifier> _visibility = new SortedSet<Modifier>()
        {
            Modifier.Private, Modifier.Protected, Modifier.Internal, Modifier.Public
        };

        public static IReadOnlyCollection<Modifier> Visibility => _visibility;


        public static Modifier AsModifierEnum(this SyntaxToken token) =>
            (Modifier)Enum.Parse(typeof(Modifier), token.Text.FirstCharToUpper());

        public static SortedSet<Modifier> ParseModifiers(this SyntaxTokenList tokens) =>
            tokens.Select(AsModifierEnum).ToSortedSet();

        public static SortedSet<Modifier> WithDefaultVisibility(this SortedSet<Modifier> modifiers,
            Modifier modifier)
        {
            if (!modifiers.Any(m => Visibility.Contains(m)))
                modifiers.Add(modifier);
            return modifiers;
        }
    }
}

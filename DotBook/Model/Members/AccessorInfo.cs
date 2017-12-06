using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DotBook.Model.Members
{
    public enum AccessorType
    {
        Getter, Setter
    }

    public class AccessorInfo : IModifiable
    {
        public AccessorType Type { get; }
        private SortedSet<Modifier> _modifiers = new SortedSet<Modifier>();
        public IReadOnlyCollection<Modifier> Modifiers => _modifiers;

        public bool IsSetter => Type == AccessorType.Setter;
        public bool IsGetter => Type == AccessorType.Getter;

        public AccessorInfo(AccessorDeclarationSyntax decl, IModifiable parent)
        {
            _modifiers = decl.Modifiers.ParseModifiers()
                .WithDefaultVisibility(parent.Modifiers.First());
            Type = decl.Keyword.ToString().StartsWith("get") ?
                AccessorType.Getter : AccessorType.Setter;
        }

        public AccessorInfo(IModifiable parent)
        {
            Type = AccessorType.Getter;
            _modifiers.Add(parent.Modifiers.First());
        }
    }
}

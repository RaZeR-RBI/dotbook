using DotBook.Model.Entities;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;
using static DotBook.Utils.Common;

namespace DotBook.Model.Members
{
    public class FieldInfo : IMember
    {
        public string Name { get; }
        public string FullName { get => $"{Parent.FullName}.{Name}"; }
        public IMemberContainer Parent { get; }

        private SortedSet<Modifier> _modifiers = new SortedSet<Modifier>();
        public IReadOnlyCollection<Modifier> Modifiers => _modifiers;

        public string Type { get; }
        public string Value { get; }

        public string Documentation { get; }

        public FieldInfo(FieldDeclarationSyntax decl, IMemberContainer parent)
        {
            var variable = decl.Declaration.Variables.First();
            if (decl.HasLeadingTrivia)
                Documentation = GetDocumentation(decl.GetLeadingTrivia());

            Name = variable.Identifier.Text;
            Type = decl.Declaration.Type.ToString();
            _modifiers = decl.Modifiers
                .ParseModifiers()
                .WithDefaultVisibility(Modifier.Private);

            Parent = parent;
            Value = variable.Initializer?.Value?.ToString();
        }

        public int CompareTo(object obj) => 
            FullName.CompareTo((obj as FieldInfo)?.FullName);
    }
}

using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBook.Model.Entities
{
    public class EnumInfo : INameable, IModifiable
    {
        private SortedSet<Modifier> _modifiers = new SortedSet<Modifier>();
        public IReadOnlyCollection<Modifier> Modifiers => _modifiers;

        public string Name { get; }
        public string FullName { get => $"{Parent.FullName}.{Name}"; }
        public INameable Parent { get; }

        public EnumInfo(EnumDeclarationSyntax decl, INameable parent) =>
            (Name, Parent) = (decl.Identifier.Text, parent);

        public override bool Equals(object obj) =>
            Equals(obj as EnumInfo);

        private bool Equals(EnumInfo other) =>
            (other != null) && (FullName == other.FullName);

        public override int GetHashCode() =>
            733961487 + EqualityComparer<string>.Default.GetHashCode(FullName);
    }
}

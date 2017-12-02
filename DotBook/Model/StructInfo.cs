using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBook.Model
{
    public class StructInfo : INameable, IModifiable, IPartial<StructDeclarationSyntax>
    {
        private HashSet<Modifier> _modifiers = new HashSet<Modifier>();
        public IReadOnlyCollection<Modifier> Modifiers => _modifiers;

        public string Name { get; }
        public string FullName { get => $"{Parent.FullName}.{Name}"; }
        public INameable Parent { get; }

        public StructInfo(StructDeclarationSyntax source, INameable parent) =>
            (Name, Parent) = (source.Identifier.Text, parent);

        public void Populate(StructDeclarationSyntax source)
        {
            // TODO
        }

        public override bool Equals(object obj) => Equals(obj as StructInfo);

        private bool Equals(StructInfo other) =>
            (other != null) && (FullName == other.FullName);

        public override string ToString() => FullName;

        public override int GetHashCode() =>
            733961487 + EqualityComparer<string>.Default.GetHashCode(FullName);
    }
}

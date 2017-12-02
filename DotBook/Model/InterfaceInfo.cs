using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBook.Model
{
    public class InterfaceInfo : INameable, IModifiable, IPartial<InterfaceDeclarationSyntax>
    {
        private HashSet<Modifier> _modifiers = new HashSet<Modifier>();
        public IReadOnlyCollection<Modifier> Modifiers => _modifiers;

        public string Name { get; }
        public string FullName { get => $"{Parent.FullName}.{Name}"; }
        public INameable Parent { get; }

        public InterfaceInfo(InterfaceDeclarationSyntax source, INameable parent) =>
            (Name, Parent) = (source.Identifier.Text, parent);

        public void Populate(InterfaceDeclarationSyntax source)
        {
            // TODO
        }

        public override bool Equals(object obj) => Equals(obj as InterfaceInfo);

        private bool Equals(InterfaceInfo other) =>
            (other != null) && (FullName == other.FullName);

        public override string ToString() => FullName;

        public override int GetHashCode() =>
            733961487 + EqualityComparer<string>.Default.GetHashCode(FullName);
    }
}

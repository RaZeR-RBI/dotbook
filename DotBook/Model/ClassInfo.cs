using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBook.Model
{
    public class ClassInfo : INameable, IModifiable, IPartial<ClassDeclarationSyntax>
    {
        private HashSet<Modifier> _modifiers = new HashSet<Modifier>();
        public IReadOnlyCollection<Modifier> Modifiers => _modifiers;

        public string Name { get; }
        public string FullName { get => $"{Parent.FullName}.{Name}"; }
        public INameable Parent { get; }

        public ClassInfo(ClassDeclarationSyntax source, INameable parent) =>
            (Name, Parent) = (source.Identifier.Text, parent);

        public void Populate(ClassDeclarationSyntax source)
        {
            // TODO
        }

        public override bool Equals(object obj) => Equals(obj as ClassInfo);

        private bool Equals(ClassInfo other) =>
            (other != null) && (Name == other.Name) && (Parent == other.Parent);

        public override int GetHashCode()
        {
            var hashCode = -512206829;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<INameable>.Default.GetHashCode(Parent);
            return hashCode;
        }

        public override string ToString() => FullName;
    }
}

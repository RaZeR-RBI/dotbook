using DotBook.Model.Entities;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DotBook.Model.Members
{
    public class PropertyInfo : INameable, IModifiable, IComparable
    {
        public string Name { get; }
        public string FullName { get => $"{Parent.FullName}.{Name}"; }
        public INameable Parent { get; }

        private SortedSet<Modifier> _modifiers = new SortedSet<Modifier>();
        public IReadOnlyCollection<Modifier> Modifiers => _modifiers;

        public string Type { get; }
        public AccessorInfo Getter { get; }
        public AccessorInfo Setter { get; }
        public bool HasGetter => Getter != null;
        public bool HasSetter => Setter != null;

        public PropertyInfo(PropertyDeclarationSyntax decl, INameable parent)
        {
            Name = decl.Identifier.Text;
            _modifiers = decl.Modifiers
                .ParseModifiers()
                .WithDefaultVisibility(Modifier.Private);

            Parent = parent;
            Type = decl.Type.ToString();

            var accessors = decl.AccessorList?.Accessors
                .Select(a => new AccessorInfo(a, this));

            if (accessors != null)
            {
                Setter = accessors.FirstOrDefault(a => a.IsSetter);
                Getter = accessors.FirstOrDefault(a => a.IsGetter);
            }
            else if (decl.ExpressionBody != null)
                Getter = new AccessorInfo(this);
        }

        public int CompareTo(object obj) =>
            FullName.CompareTo((obj as FieldInfo)?.FullName);
    }
}

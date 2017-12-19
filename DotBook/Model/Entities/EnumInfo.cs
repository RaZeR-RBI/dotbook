using DotBook.Processing;
using DotBook.Utils;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using static DotBook.Utils.Common;
using static DotBook.Model.Extensions;

namespace DotBook.Model.Entities
{
    public class EnumInfo : IMemberContainer
    {
        public string Name { get; }
        public string FullName { get => $"{Parent.FullName}.{Name}"; }

        private SortedSet<Modifier> _modifiers = new SortedSet<Modifier>();
        public IReadOnlyCollection<Modifier> Modifiers => _modifiers;

        public string UnderlyingType { get; }
        private List<EnumValue> _values = new List<EnumValue>();
        public IReadOnlyCollection<EnumValue> Values => _values;

        public string Documentation { get; }

        public IReadOnlyCollection<IMember> Members => null;
        public ITypeContainer Parent { get; }

        public INode<INameable> ParentNode => Parent;
        public IEnumerable<INode<INameable>> ChildrenNodes => null;


        public EnumInfo(EnumDeclarationSyntax decl, ITypeContainer parent)
        {
            (Name, Parent) = (decl.Identifier.Text, parent);
            if (decl.HasLeadingTrivia)
                Documentation = GetDocumentation(decl.GetLeadingTrivia());

            _modifiers = decl.Modifiers
                .ParseModifiers()
                .WithDefaultVisibility(
                    Parent is NamespaceInfo ?
                    Modifier.Internal : Modifier.Private);

            var baseList = decl.BaseList;
            var typeDecl = baseList == null ? "int" : baseList.Types.First().ToString();
            UnderlyingType = typeDecl;

            foreach (EnumMemberDeclarationSyntax member in decl.Members)
                _values.Add(new EnumValue(member, this));
        }

        public override bool Equals(object obj) =>
            Equals(obj as EnumInfo);

        private bool Equals(EnumInfo other) =>
            (other != null) && (FullName == other.FullName);

        public override int GetHashCode() =>
            733961487 + EqualityComparer<string>.Default.GetHashCode(FullName);

        public int CompareTo(object obj) =>
            FullName.CompareTo((obj as EnumInfo)?.FullName);

        public class EnumValue
        {
            public string Key { get; }
            public string Value { get; }

            private static IReadOnlyCollection<Modifier> _visibility =
                new List<Modifier>() { Modifier.Public };
            public IReadOnlyCollection<Modifier> Modifiers => _visibility;

            public string Documentation { get; }

            public EnumValue(EnumMemberDeclarationSyntax decl, EnumInfo parent)
            {
                Key = decl.Identifier.Text;
                Value = decl.EqualsValue?.Value?.ToString() ?? "";
                if (decl.HasLeadingTrivia)
                    Documentation = GetDocumentation(decl.GetLeadingTrivia());
            }

            public int CompareTo(object obj) =>
                Key.CompareTo((obj as EnumValue)?.Key);
        }
    }
}

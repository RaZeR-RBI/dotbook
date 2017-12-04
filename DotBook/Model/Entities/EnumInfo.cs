using DotBook.Utils;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;

namespace DotBook.Model.Entities
{
    public class EnumInfo : INameable, IModifiable, IComparable
    {
        public string Name { get; }
        public string FullName { get => $"{Parent.FullName}.{Name}"; }
        public INameable Parent { get; }

        private SortedSet<Modifier> _modifiers = new SortedSet<Modifier>();
        public IReadOnlyCollection<Modifier> Modifiers => _modifiers;

        public string UnderlyingType { get; }
        private List<Member> _members = new List<Member>();
        public IReadOnlyCollection<Member> Members => _members;


        public EnumInfo(EnumDeclarationSyntax decl, INameable parent)
        {
            (Name, Parent) = (decl.Identifier.Text, parent);
            _modifiers = decl.Modifiers
                .ParseModifiers()
                .WithDefaultVisibility(
                    Parent is NamespaceInfo ?
                    Modifier.Internal : Modifier.Private);

            var baseList = decl.BaseList;
            var typeDecl = baseList == null ? "int" : baseList.Types.First().ToString();
            UnderlyingType = typeDecl;

            foreach (EnumMemberDeclarationSyntax member in decl.Members)
                _members.Add(new Member(member));
        }

        public override bool Equals(object obj) =>
            Equals(obj as EnumInfo);

        private bool Equals(EnumInfo other) =>
            (other != null) && (FullName == other.FullName);

        public override int GetHashCode() =>
            733961487 + EqualityComparer<string>.Default.GetHashCode(FullName);

        public int CompareTo(object obj) =>
            FullName.CompareTo((obj as EnumInfo)?.FullName);

        public class Member
        {
            public string Key { get; }
            public string Value { get; }

            public Member(EnumMemberDeclarationSyntax decl)
            {
                Key = decl.Identifier.Text;
                Value = decl.EqualsValue?.Value?.ToString() ?? "";
            }
        }
    }
}

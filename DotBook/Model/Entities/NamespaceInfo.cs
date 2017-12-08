using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using static DotBook.Utils.Common;
using static DotBook.Logger;

namespace DotBook.Model.Entities
{
    public class NamespaceInfo : INameable, IPartial<NamespaceDeclarationSyntax>,
        IComparable, IDocumentable
    {
        public string Name { get; }
        public string FullName => Name;

        private SortedSet<ClassInfo> _classes = new SortedSet<ClassInfo>();
        public IReadOnlyCollection<ClassInfo> Classes => _classes;

        private SortedSet<StructInfo> _structs = new SortedSet<StructInfo>();
        public IReadOnlyCollection<StructInfo> Structs => _structs;

        private SortedSet<EnumInfo> _enums = new SortedSet<EnumInfo>();
        public IReadOnlyCollection<EnumInfo> Enums => _enums;

        private SortedSet<InterfaceInfo> _interfaces = new SortedSet<InterfaceInfo>();
        public IReadOnlyCollection<InterfaceInfo> Interfaces => _interfaces;

        public string Documentation { get; private set; }

        public NamespaceInfo(NamespaceDeclarationSyntax declaration) =>
            Name = declaration.Name.ToString();

        public void Populate(NamespaceDeclarationSyntax source)
        {
            if (source.HasLeadingTrivia)
            {
                var doc = GetDocumentation(source.GetLeadingTrivia());
                if (doc != null)
                {
                    if (Documentation != null)
                        Warning("Found several documentation comments for " + FullName);
                    Documentation = doc;
                }
            }

            foreach (var member in source.Members)
                this.AddAsChild(member, _classes, _structs, _interfaces, _enums);
        }

        public override bool Equals(object obj) => Equals(obj as NamespaceInfo);

        private bool Equals(NamespaceInfo other)
        {
            if (other == null) return false;
            return Name == other.Name;
        }

        public override int GetHashCode() => 
            539060726 + EqualityComparer<string>.Default.GetHashCode(Name);

        public override string ToString() => FullName;

        public int CompareTo(object obj) => 
            FullName.CompareTo((obj as NamespaceInfo)?.FullName);
    }
}

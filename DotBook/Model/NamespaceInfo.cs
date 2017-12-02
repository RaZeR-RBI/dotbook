using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DotBook.Model
{
    public class NamespaceInfo : INameable, IPartial<NamespaceDeclarationSyntax>
    {
        public string Name { get; }
        public string FullName => Name;

        private HashSet<ClassInfo> _classes = new HashSet<ClassInfo>();
        public IReadOnlyCollection<ClassInfo> Classes => _classes;

        private HashSet<StructInfo> _structs = new HashSet<StructInfo>();
        public IReadOnlyCollection<StructInfo> Structs => _structs;

        private HashSet<EnumInfo> _enums = new HashSet<EnumInfo>();
        public IReadOnlyCollection<EnumInfo> Enums => _enums;

        private HashSet<InterfaceInfo> _interfaces = new HashSet<InterfaceInfo>();
        public IReadOnlyCollection<InterfaceInfo> Interfaces => _interfaces;

        public NamespaceInfo(NamespaceDeclarationSyntax declaration) =>
            Name = declaration.Name.ToString();

        public void Populate(NamespaceDeclarationSyntax root)
        {
            foreach (var member in root.Members)
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
    }
}

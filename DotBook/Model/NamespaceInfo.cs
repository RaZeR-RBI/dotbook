using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;

namespace DotBook.Model
{
    public class NamespaceInfo : INameable, IPartial<NamespaceDeclarationSyntax>
    {
        public string Name { get; }

        private List<ClassInfo> _classes = new List<ClassInfo>();
        public IReadOnlyCollection<ClassInfo> Classes => _classes;

        private List<StructInfo> _structs = new List<StructInfo>();
        public IReadOnlyCollection<StructInfo> Structs => _structs;

        private List<EnumInfo> _enums = new List<EnumInfo>();
        public IReadOnlyCollection<EnumInfo> Enums => _enums;

        public NamespaceInfo(NamespaceDeclarationSyntax declaration) =>
            Name = declaration.Name.ToString();

        public void Populate(NamespaceDeclarationSyntax root)
        {
            foreach (var member in root.Members)
            {
                switch (member)
                {
                    case ClassDeclarationSyntax @class:

                        break;
                    case EnumDeclarationSyntax @enum:

                        break;
                    case StructDeclarationSyntax @struct:

                        break;
                }
            }
            throw new NotImplementedException();
        }

        public override bool Equals(object obj) => Equals(obj as NamespaceInfo);

        private bool Equals(NamespaceInfo other)
        {
            if (other == null) return false;
            return Name == other.Name;
        }

        public override int GetHashCode() => 
            539060726 + EqualityComparer<string>.Default.GetHashCode(Name);
    }
}

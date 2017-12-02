﻿using Microsoft.CodeAnalysis.CSharp.Syntax;
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
            {
                switch (member)
                {
                    case ClassDeclarationSyntax classDecl:
                        new ClassInfo(classDecl, this).AddOrReuse(classDecl, _classes);
                        break;
                    case EnumDeclarationSyntax enumDecl:
                        _enums.Add(new EnumInfo(enumDecl, this));
                        break;
                    case StructDeclarationSyntax structDecl:
                        new StructInfo(structDecl, this).AddOrReuse(structDecl, _structs);
                        break;
                    case InterfaceDeclarationSyntax intDecl:
                        new InterfaceInfo(intDecl, this).AddOrReuse(intDecl, _interfaces);
                        break;
                }
            }
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

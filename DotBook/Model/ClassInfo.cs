﻿using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
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

        private HashSet<ClassInfo> _classes = new HashSet<ClassInfo>();
        public IReadOnlyCollection<ClassInfo> Classes => _classes;

        private HashSet<StructInfo> _structs = new HashSet<StructInfo>();
        public IReadOnlyCollection<StructInfo> Structs => _structs;

        private HashSet<EnumInfo> _enums = new HashSet<EnumInfo>();
        public IReadOnlyCollection<EnumInfo> Enums => _enums;

        private HashSet<InterfaceInfo> _interfaces = new HashSet<InterfaceInfo>();
        public IReadOnlyCollection<InterfaceInfo> Interfaces => _interfaces;

        public ClassInfo(ClassDeclarationSyntax source, INameable parent) =>
            (Name, Parent) = (source.Identifier.Text, parent);

        public void Populate(ClassDeclarationSyntax source)
        {
            _modifiers = source.Modifiers.ParseModifiers();
            foreach (var member in source.Members)
                this.AddAsChild(member, _classes, _structs, _interfaces, _enums);
        }

        public override bool Equals(object obj) => Equals(obj as ClassInfo);

        private bool Equals(ClassInfo other) =>
            (other != null) && (FullName == other.FullName);

        public override string ToString() => FullName;

        public override int GetHashCode() =>
            733961487 + EqualityComparer<string>.Default.GetHashCode(FullName);
    }
}

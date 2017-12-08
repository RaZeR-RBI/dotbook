using DotBook.Model.Members;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using static DotBook.Utils.Common;
using static DotBook.Logger;

namespace DotBook.Model.Entities
{
    public class StructInfo : INameable, IModifiable, IPartial<StructDeclarationSyntax>,
        IComparable, IDerivable, IDocumentable
    {
        public string Name { get; }
        public string FullName { get => $"{Parent.FullName}.{Name}"; }
        public INameable Parent { get; }

        private SortedSet<Modifier> _modifiers = new SortedSet<Modifier>();
        public IReadOnlyCollection<Modifier> Modifiers => _modifiers;

        private SortedSet<ClassInfo> _classes = new SortedSet<ClassInfo>();
        public IReadOnlyCollection<ClassInfo> Classes => _classes;

        private SortedSet<StructInfo> _structs = new SortedSet<StructInfo>();
        public IReadOnlyCollection<StructInfo> Structs => _structs;

        private SortedSet<EnumInfo> _enums = new SortedSet<EnumInfo>();
        public IReadOnlyCollection<EnumInfo> Enums => _enums;

        private SortedSet<InterfaceInfo> _interfaces = new SortedSet<InterfaceInfo>();
        public IReadOnlyCollection<InterfaceInfo> Interfaces => _interfaces;

        private SortedSet<FieldInfo> _fields = new SortedSet<FieldInfo>();
        public IReadOnlyCollection<FieldInfo> Fields => _fields;

        private SortedSet<PropertyInfo> _properties = new SortedSet<PropertyInfo>();
        public IReadOnlyCollection<PropertyInfo> Properties => _properties;

        private SortedSet<IndexerInfo> _indexers = new SortedSet<IndexerInfo>();
        public IReadOnlyCollection<IndexerInfo> Indexers => _indexers;

        private SortedSet<MethodInfo> _methods = new SortedSet<MethodInfo>();
        public IReadOnlyCollection<MethodInfo> Methods => _methods;

        private SortedSet<ConstructorInfo> _constructors = new SortedSet<ConstructorInfo>();
        public IReadOnlyCollection<ConstructorInfo> Constructors => _constructors;

        private SortedSet<string> _baseTypes = new SortedSet<string>();
        public IReadOnlyCollection<string> BaseTypes => _baseTypes;

        public string Documentation { get; private set; }

        public StructInfo(StructDeclarationSyntax source, INameable parent)
        {
            Parent = parent;
            Name = source.Identifier.Text + Format(source.TypeParameterList);
            _baseTypes = Parse(source.BaseList);
        }

        public void Populate(StructDeclarationSyntax source)
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

            _modifiers = source.Modifiers
                .ParseModifiers()
                .WithDefaultVisibility(
                    Parent is NamespaceInfo ?
                    Modifier.Internal : Modifier.Private);

            foreach (var member in source.Members)
                this.AddAsChild(member,
                    _classes, _structs, _interfaces, _enums,
                    _fields, _properties, _indexers, _methods, _constructors);
        }

        public override bool Equals(object obj) => Equals(obj as StructInfo);

        private bool Equals(StructInfo other) =>
            (other != null) && (FullName == other.FullName);

        public override string ToString() => FullName;

        public override int GetHashCode() =>
            733961487 + EqualityComparer<string>.Default.GetHashCode(FullName);

        public int CompareTo(object obj) =>
            FullName.CompareTo((obj as StructInfo)?.FullName);
    }
}

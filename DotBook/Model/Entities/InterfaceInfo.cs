using DotBook.Model.Members;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;
using static DotBook.Utils.Common;
using static DotBook.Logger;
using static DotBook.Model.Extensions;
using System.Linq;

namespace DotBook.Model.Entities
{
    public class InterfaceInfo : IMemberContainer, IPartial<InterfaceDeclarationSyntax>,
        IDerivable
    {
        private SortedSet<Modifier> _modifiers = new SortedSet<Modifier>();
        public IReadOnlyCollection<Modifier> Modifiers => _modifiers;

        public string Name { get; }
        public string FullName { get => $"{Parent.FullName}.{Name}"; }
        public ITypeContainer Parent { get; }

        private SortedSet<PropertyInfo> _properties = new SortedSet<PropertyInfo>();
        public IReadOnlyCollection<PropertyInfo> Properties => _properties;

        private SortedSet<IndexerInfo> _indexers = new SortedSet<IndexerInfo>();
        public IReadOnlyCollection<IndexerInfo> Indexers => _indexers;

        private SortedSet<MethodInfoBase> _methods = new SortedSet<MethodInfoBase>();
        public IReadOnlyCollection<MethodInfo> Methods =>
            _methods.OfType<MethodInfo>().ToList();

        private SortedSet<string> _baseTypes = new SortedSet<string>();
        public IReadOnlyCollection<string> BaseTypes => _baseTypes;

        public string Documentation { get; private set; }

        public IReadOnlyCollection<IMember> Members =>
            CastJoin<IMember>(_properties, _indexers, _methods);

        public InterfaceInfo(InterfaceDeclarationSyntax source, ITypeContainer parent)
        {
            Parent = parent;
            Name = source.Identifier.Text + Format(source.TypeParameterList);
            _baseTypes = Parse(source.BaseList);
        }

        public void Populate(InterfaceDeclarationSyntax source)
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
            foreach(var member in source.Members)
            {
                this.AddMembers(member, 
                    properties: _properties,
                    indexers: _indexers,
                    methods: _methods);
            }  
        }

        public override bool Equals(object obj) => Equals(obj as InterfaceInfo);

        private bool Equals(InterfaceInfo other) =>
            (other != null) && (FullName == other.FullName);

        public override string ToString() => FullName;

        public override int GetHashCode() =>
            733961487 + EqualityComparer<string>.Default.GetHashCode(FullName);

        public int CompareTo(object obj) =>
            FullName.CompareTo((obj as InterfaceInfo)?.FullName);
    }
}

﻿using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;
using static DotBook.Utils.Common;

namespace DotBook.Model.Entities
{
    public class InterfaceInfo : INameable, IModifiable, IPartial<InterfaceDeclarationSyntax>,
        IDerivable, IComparable
    {
        private SortedSet<Modifier> _modifiers = new SortedSet<Modifier>();
        public IReadOnlyCollection<Modifier> Modifiers => _modifiers;

        public string Name { get; }
        public string FullName { get => $"{Parent.FullName}.{Name}"; }
        public INameable Parent { get; }

        private SortedSet<string> _baseTypes = new SortedSet<string>();
        public IReadOnlyCollection<string> BaseTypes => _baseTypes;

        public InterfaceInfo(InterfaceDeclarationSyntax source, INameable parent)
        {
            Parent = parent;
            Name = source.Identifier.Text + Format(source.TypeParameterList);
            _baseTypes = Parse(source.BaseList);
        }

        public void Populate(InterfaceDeclarationSyntax source)
        {
            _modifiers = source.Modifiers
                .ParseModifiers()
                .WithDefaultVisibility(
                    Parent is NamespaceInfo ?
                    Modifier.Internal : Modifier.Private);
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

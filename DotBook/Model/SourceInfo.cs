using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using DotBook.Model.Entities;

namespace DotBook.Model
{
    public class SourceInfo
    {
        private SortedSet<NamespaceInfo> _namespaces = new SortedSet<NamespaceInfo>();
        public IReadOnlyCollection<NamespaceInfo> Namespaces => _namespaces;

        public SourceInfo(List<CompilationUnitSyntax> roots) =>
            roots.ForEach(Process);

        private void Process(CompilationUnitSyntax root)
        {
            if (!root.Members.Any()) return;
            var node = root.Members.First() as NamespaceDeclarationSyntax;
            new NamespaceInfo(node).AddOrReuse(node, _namespaces);
        }

        public override string ToString() =>
            string.Join("\n", _namespaces.Select(n => n.ToString()));
    }
}

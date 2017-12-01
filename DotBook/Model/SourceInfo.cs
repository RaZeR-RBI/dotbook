using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DotBook.Model
{
    public class SourceInfo
    {
        private HashSet<NamespaceInfo> _namespaces = new HashSet<NamespaceInfo>();
        public IReadOnlyCollection<NamespaceInfo> Namespaces => _namespaces;

        public SourceInfo(List<CompilationUnitSyntax> roots) =>
            roots.ForEach(Process);

        private void Process(CompilationUnitSyntax root)
        {
            var namespaceNode = root.Members.First() as NamespaceDeclarationSyntax;
            var namespaceInfo = GetNamespace(namespaceNode);
        }

        private NamespaceInfo GetNamespace(NamespaceDeclarationSyntax node)
        {
            _namespaces.Add(new NamespaceInfo(node));
            return _namespaces.First(n => n.Equals(node));
        }

        public override string ToString() =>
            string.Join("\n", _namespaces.Select(n => n.ToString()));
    }
}

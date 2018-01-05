using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using DotBook.Model.Entities;
using DotBook.Processing;

namespace DotBook.Model
{
    public class SourceInfo : IDocumentationNode
    {
        private SortedSet<NamespaceInfo> _namespaces = new SortedSet<NamespaceInfo>();
        public IReadOnlyCollection<NamespaceInfo> Namespaces => _namespaces;

        public INode<INameable> ParentNode => null;
        public IEnumerable<INode<INameable>> ChildrenNodes =>
            _namespaces.OfType<INode<INameable>>();

        public string Name => "";
        public string FullName => "";

        public INameable NodeValue => this;

        public string Documentation { get; set; }

        public string Preface => Documentation;

        public SourceInfo(List<CompilationUnitSyntax> roots) =>
            roots.ForEach(Process);

        private void Process(CompilationUnitSyntax root)
        {
            if (!root.Members.Any()) return;
            var node = root.Members.First() as NamespaceDeclarationSyntax;
            if (node != null)
                new NamespaceInfo(node, this).AddOrReuse(node, _namespaces);
            else
            {
                if (!_namespaces.Any(n => n.Name == "global"))
                    _namespaces.Add(new NamespaceInfo(this));
                var global = _namespaces.FirstOrDefault(n => n.Name == "global");
                global.Populate(root.Members);
            }
        }

        public override string ToString() =>
            string.Join("\n", _namespaces.Select(n => n.ToString()));
    }
}

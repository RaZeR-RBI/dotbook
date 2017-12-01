using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace DotBook
{
    public class SourceInfo
    {
        private HashSet<string> _namespaces = new HashSet<string>();
        public IReadOnlyCollection<string> Namespaces => _namespaces;

        public SourceInfo(List<SyntaxNode> roots) =>
            roots.ForEach(Process);

        private void Process(SyntaxNode node)
        {
            
        }
    }
}

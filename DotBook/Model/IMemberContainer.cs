using DotBook.Processing;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBook.Model
{
    public interface IMemberContainer : IDocumentationNode, IModifiable, IComparable
    {
        IReadOnlyCollection<IMember> Members { get; }
        ITypeContainer Parent { get; }
    }
}

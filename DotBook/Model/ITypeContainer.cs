using DotBook.Processing;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBook.Model
{
    public interface ITypeContainer : IDocumentationNode, IComparable
    {
        IReadOnlyCollection<IMemberContainer> Types { get; }
    }
}

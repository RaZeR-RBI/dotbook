using DotBook.Processing;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBook.Model
{
    public interface ITypeContainer : INameable, IDocumentable, IComparable, INode<INameable>
    {
        IReadOnlyCollection<IMemberContainer> Types { get; }
    }
}

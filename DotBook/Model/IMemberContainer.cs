using DotBook.Processing;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBook.Model
{
    public interface IMemberContainer : INameable, IModifiable, IDocumentable,
        IComparable, INode<INameable>
    {
        IReadOnlyCollection<IMember> Members { get; }
        ITypeContainer Parent { get; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace DotBook.Model
{
    public interface IMemberContainer : INameable, IModifiable, IDocumentable,
        IComparable
    {
        IReadOnlyCollection<IMember> Members { get; }
        ITypeContainer Parent { get; }
    }
}

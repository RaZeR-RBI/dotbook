using System;
using System.Collections.Generic;
using System.Text;

namespace DotBook.Model
{
    public interface ITypeContainer : INameable, IDocumentable, IComparable
    {
        IReadOnlyCollection<IMemberContainer> Types { get; }
    }
}

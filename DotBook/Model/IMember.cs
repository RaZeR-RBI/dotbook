using System;
using System.Collections.Generic;
using System.Text;

namespace DotBook.Model
{
    public interface IMember : INameable, IModifiable, IDocumentable, IComparable
    {
        IMemberContainer Parent { get; }
    }
}

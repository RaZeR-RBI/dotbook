using DotBook.Processing;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBook.Model
{
    public interface IMember : IDocumentationNode, IModifiable, IComparable
    {
        IMemberContainer Parent { get; }
    }
}

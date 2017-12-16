using System;
using System.Collections.Generic;
using System.Text;

namespace DotBook.Processing
{
    public interface INode<T>
    {
        INode<T> ParentNode { get; }
        IEnumerable<INode<T>> ChildrenNodes { get; }
    }
}

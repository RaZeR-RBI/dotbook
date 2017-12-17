using DotBook.Processing;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBook.Backend
{
    public interface INodeFormatter<TOut, TDoc, TNode>
    {
        TOut Process(TDoc doc, INode<TNode> node);
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace DotBook.Backend
{
    public interface IBackend<TOut, TNode>
    {
        TOut Process(TNode node);
    }
}

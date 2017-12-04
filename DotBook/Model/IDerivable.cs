using System;
using System.Collections.Generic;
using System.Text;

namespace DotBook.Model
{
    public interface IDerivable
    {
        IReadOnlyCollection<string> BaseTypes { get; }
    }
}

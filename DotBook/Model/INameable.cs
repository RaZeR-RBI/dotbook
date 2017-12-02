using System;
using System.Collections.Generic;
using System.Text;

namespace DotBook.Model
{
    public interface INameable
    {
        string Name { get; }
        string FullName { get; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace DotBook.Model
{
    public interface IPartial<T>
    {
        void Populate(T source);
    }
}

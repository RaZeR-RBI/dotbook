using DotBook.Model;
using DotBook.Processing;
using System.Collections.Generic;

namespace DotBook.Backend
{
    public interface IWriter
    {
         void Write(Entity entity, IEnumerable<Modifier> visibilities);
    }
}
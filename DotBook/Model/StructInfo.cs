using System;
using System.Collections.Generic;
using System.Text;

namespace DotBook.Model
{
    public class StructInfo : IModifiable
    {
        public IReadOnlyCollection<Modifier> Modifiers => throw new NotImplementedException();
    }
}

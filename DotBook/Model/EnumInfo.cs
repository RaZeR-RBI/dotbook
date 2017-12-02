using System;
using System.Collections.Generic;
using System.Text;

namespace DotBook.Model
{
    public class EnumInfo : INameable, IModifiable
    {
        public IReadOnlyCollection<Modifier> Modifiers => throw new NotImplementedException();

        public string Name => throw new NotImplementedException();

        public string FullName => throw new NotImplementedException();
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace DotBook.Model
{
    public enum Modifier
    {
        Public,
        Private,
        Internal,
        Protected,
        Abstract,
        Async,
        Const,
        Event,
        Extern,
        New,
        Override,
        Partial,
        Readonly,
        Sealed,
        Static,
        Unsafe,
        Virtual,
        Volatile
    }

    public interface IModifiable
    {
        IReadOnlyCollection<Modifier> Modifiers { get; }
    }
}

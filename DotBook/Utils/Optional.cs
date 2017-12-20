using System;
using System.Collections.Generic;
using System.Text;

namespace DotBook.Utils
{
    public class Optional<T> where T : class
    {
        private T value;

        public Optional(T value) => this.value = value;

        public void IfPresent(Action<T> action)
        {
            if (value != null) action(value);
        }
    }

    public static class Optional
    {
        public static Optional<T> Of<T>(T value)
            where T : class
            => new Optional<T>(value);

        public static Optional<T> None<T>()
            where T : class
            => new Optional<T>(null);
    }
}

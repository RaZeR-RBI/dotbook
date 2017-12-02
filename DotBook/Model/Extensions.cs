using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotBook.Model
{
    public static class Extensions
    {
        public static void AddOrReuse<T1, T2>(this T2 info, T1 decl, HashSet<T2> set)
            where T2 : IPartial<T1>
        {
            set.Add(info);
            var savedInfo = set.Where(c => c.Equals(info)).First();
            savedInfo.Populate(decl);
        }
    }
}

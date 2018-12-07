using DotBook.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotBook.Processing
{
    public static class EntityTree
    {
        // taken from corefx
        static char[] s_invalidNameChars = new char[]
        {
            '\"', '<', '>', '|', '\0',
            (char)1, (char)2, (char)3, (char)4, (char)5, (char)6, (char)7, (char)8, (char)9, (char)10,
            (char)11, (char)12, (char)13, (char)14, (char)15, (char)16, (char)17, (char)18, (char)19, (char)20,
            (char)21, (char)22, (char)23, (char)24, (char)25, (char)26, (char)27, (char)28, (char)29, (char)30,
            (char)31, ':', '*', '?', '\\', '/'
        };

        private static JsonSerializerSettings settings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        public static Entity WithVisibility(this SourceInfo root,
            bool useHashCodeAsLink = false,
            params Modifier[] modifiers) =>
            new Entity(root, n =>
            {
                var modifiable = n as IModifiable;
                if (modifiable == null) return true;
                return modifiable.Modifiers.Intersect(modifiers).Any();
            },
            s =>
            {
                if (useHashCodeAsLink)
                    return $"{(long)s.GetHashCode() + int.MaxValue}";
                else
                    return new string(s
                        .Select(c => s_invalidNameChars.Contains(c) ? '_' : c)
                        .ToArray());
            });

        public static string AsJson(this Entity entity) =>
            JsonConvert.SerializeObject(entity, Formatting.Indented, settings);
    }
}

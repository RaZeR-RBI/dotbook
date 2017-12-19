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
                    return s.Replace('<', '{')
                        .Replace('>', '}')
                        .Replace(" ", "");
            });

        public static string AsJson(this Entity entity) =>
            JsonConvert.SerializeObject(entity, Formatting.Indented, settings);
    }
}

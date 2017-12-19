using DotBook.Model;
using DotBook.Model.Entities;
using DotBook.Model.Members;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBook.Processing
{
    public static class EntityTypeResolver
    {
        private static Dictionary<Type, EntityType> _map =
            new Dictionary<Type, EntityType>()
            {
                [typeof(SourceInfo)] = EntityType.Root,
                [typeof(NamespaceInfo)] = EntityType.Namespace,
                [typeof(ClassInfo)] = EntityType.Class,
                [typeof(EnumInfo)] = EntityType.Enum,
                [typeof(StructInfo)] = EntityType.Struct,
                [typeof(InterfaceInfo)] = EntityType.Interface,

                [typeof(FieldInfo)] = EntityType.Field,
                [typeof(PropertyInfo)] = EntityType.Property,
                [typeof(MethodInfo)] = EntityType.Method,
                [typeof(ConstructorInfo)] = EntityType.Constructor,
                [typeof(OperatorInfo)] = EntityType.Operator,
                [typeof(IndexerInfo)] = EntityType.Indexer,
                [typeof(EnumInfo.EnumValue)] = EntityType.EnumValue
            };

        public static EntityType Resolve(INameable source) =>
            _map[source.GetType()];
    }
}

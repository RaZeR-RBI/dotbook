﻿using DotBook.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using static DotBook.Processing.EntityTypeResolver;
using DotBook.Utils;

namespace DotBook.Processing
{
    public enum EntityType
    {
        Root,
        Namespace,
        Class,
        Enum,
        Struct,
        Interface,
        Field,
        Property,
        Method,
        Constructor,
        Operator,
        Indexer,
        EnumValue
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class Entity : INameable, INode<Entity>
    {
        [JsonProperty("name")]
        public string Name { get; }

        [JsonProperty("fullName")]
        public string FullName { get; }

        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public EntityType Type { get; }

        [JsonProperty("link")]
        public string Link { get; }

        public IDocumentationNode Base { get; }

        public INode<Entity> ParentNode { get; }
        public IEnumerable<INode<Entity>> ChildrenNodes { get; }
        private Func<object, bool> bypass = s => true;

        public Entity(INode<INameable> node, Func<object, bool> predicate,
            Func<string, string> linkFromName)
        {
            var source = node as INameable;
            (Name, FullName, Type, Base) =
            (source.Name, source.FullName, Resolve(source), source as IDocumentationNode);

            Link = linkFromName(FullName);
            if (node != null)
                ChildrenNodes = node.ChildrenNodes?
                    .Where(predicate ?? bypass)
                    .Select(n => new Entity(n as INode<INameable>, this,
                        predicate, linkFromName));
        }

        private Entity(INode<INameable> node, Entity parent,
            Func<object, bool> predicate,
            Func<string, string> linkFromName) :
            this(node, predicate, linkFromName) =>
            ParentNode = parent;

        public Entity GetByFullName(string fullName) =>
            this.GetRoot()
                .Descendants(n => (n as Entity)?.FullName == fullName)
                .FirstOrDefault() as Entity;

        public string GetLink(string fullName) =>
            GetByFullName(fullName)?.Link ?? "";
    }
}

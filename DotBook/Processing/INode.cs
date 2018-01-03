using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBook.Processing
{
    [JsonObject(MemberSerialization.OptIn)]
    public interface INode<T>
    {
        INode<T> ParentNode { get; }
        T NodeValue { get; }

        [JsonProperty("children")]
        IEnumerable<INode<T>> ChildrenNodes { get; }
    }
}

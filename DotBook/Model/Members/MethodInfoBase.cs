using DotBook.Processing;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBook.Model.Members
{
    public abstract class MethodInfoBase : IMember, INode<INameable>
    {
        public string Name { get; protected set; }
        public string FullName { get => $"{Parent.FullName}.{Name}"; }
        public IMemberContainer Parent { get; protected set; }

        protected SortedSet<Modifier> _modifiers = new SortedSet<Modifier>();
        public IReadOnlyCollection<Modifier> Modifiers => _modifiers;

        protected List<ParameterInfo> _parameters;
        public IReadOnlyCollection<ParameterInfo> Parameters => _parameters;

        public string Signature { get; protected set; }
        public string ReturnType { get; protected set; }

        public string Documentation { get; protected set; }

        public INode<INameable> ParentNode => Parent;

        public IEnumerable<INode<INameable>> ChildrenNodes => null;

        public int CompareTo(object obj) =>
            FullName.CompareTo((obj as MethodInfo)?.FullName);
    }
}

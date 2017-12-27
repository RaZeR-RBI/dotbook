using DotBook.Processing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

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

        private string _signature;
        public string Signature { 
            get => _signature.SingleLine(); 
            protected set => _signature = value;
        }

        public string Syntax =>
            string.Join(" ", _modifiers.Select(e => e.ToString().ToLower())) +
            $" {_signature}";

        public string ReturnType { get; protected set; }

        public string Documentation { get; protected set; }

        public INode<INameable> ParentNode => Parent;

        public IEnumerable<INode<INameable>> ChildrenNodes => null;

        protected string ParamSyntax() =>
            _parameters.Count == 0 ? "" :
            $"\n\t{string.Join(",\n\t", _parameters)}\n";

        public int CompareTo(object obj) =>
            FullName.CompareTo((obj as MethodInfo)?.FullName);
    }
}

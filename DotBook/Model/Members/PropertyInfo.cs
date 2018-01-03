using DotBook.Model.Entities;
using DotBook.Processing;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using static DotBook.Utils.Common;

namespace DotBook.Model.Members
{
    public class PropertyInfo : MemberInfoBase
    {
        public AccessorInfo Getter { get; }
        public AccessorInfo Setter { get; }
        public bool HasGetter => Getter != null;
        public bool HasSetter => Setter != null;

        public PropertyInfo(PropertyDeclarationSyntax decl, IMemberContainer parent)
        {
            Name = decl.Identifier.Text;
            if (decl.HasLeadingTrivia)
                Documentation = GetDocumentation(decl.GetLeadingTrivia());
            Parent = parent;
            _modifiers = decl.Modifiers
                .ParseModifiers()
                .WithDefaultVisibility(Parent is InterfaceInfo ?
                    Modifier.Public : Modifier.Private);

            ReturnType = decl.Type.ToString();

            var accessors = decl.AccessorList?.Accessors
                .Select(a => new AccessorInfo(a, this));

            if (accessors != null)
            {
                Setter = accessors.FirstOrDefault(a => a.IsSetter);
                Getter = accessors.FirstOrDefault(a => a.IsGetter);
            }
            else if (decl.ExpressionBody != null)
                Getter = new AccessorInfo(this);

            _parameters = new List<ParameterInfo>();
            Signature = $"{ReturnType} {Name} " + "{";
            if (HasGetter) Signature += Getter.ToString();
            if (HasSetter) Signature += Setter.ToString();
            Signature += " }";
        }
    }
}

using DotBook.Model.Entities;
using DotBook.Processing;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using static DotBook.Utils.Common;

namespace DotBook.Model.Members
{
    public class IndexerInfo : MemberInfoBase
    {
        public AccessorInfo Getter { get; }
        public AccessorInfo Setter { get; }
        public bool HasGetter => Getter != null;
        public bool HasSetter => Setter != null;

        public IndexerInfo(IndexerDeclarationSyntax decl, IMemberContainer parent)
        {
            var paramList = decl.ParameterList.Parameters
                .Select(p => p.Type.ToString());

            if (decl.HasLeadingTrivia)
                Documentation = GetDocumentation(decl.GetLeadingTrivia());

            Name = decl.Type.ToString() + $"[{string.Join(", ", paramList)}]";
            _parameters = Parse(decl.ParameterList);
            Parent = parent;
            _modifiers = decl.Modifiers
                .ParseModifiers()
                .WithDefaultVisibility(Parent is InterfaceInfo ?
                    Modifier.Public : Modifier.Private);

            ReturnType = decl.Type.ToString();
            Signature = $"{ReturnType} this[{ParamSyntax()}]";

            var accessors = decl.AccessorList?.Accessors
                .Select(a => new AccessorInfo(a, this));

            if (accessors != null)
            {
                Setter = accessors.FirstOrDefault(a => a.IsSetter);
                Getter = accessors.FirstOrDefault(a => a.IsGetter);
            }
            else if (decl.ExpressionBody != null)
                Getter = new AccessorInfo(this);
        }
    }
}

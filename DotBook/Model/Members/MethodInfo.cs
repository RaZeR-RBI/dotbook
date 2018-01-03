using DotBook.Utils;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static DotBook.Utils.Common;
using DotBook.Model.Entities;

namespace DotBook.Model.Members
{
    public class MethodInfo : MemberInfoBase
    {
        public MethodInfo(MethodDeclarationSyntax decl, IMemberContainer parent)
        {
            var paramTypes = decl.ParameterList.Parameters
                .Select(p => p.Type.ToString());
            var typeParams = Format(decl.TypeParameterList);

            Name = $"{decl.Identifier.Text}{typeParams}" +
                $"({string.Join(", ", paramTypes)})";
            _parameters = Parse(decl.ParameterList);
            Parent = parent;
            _modifiers = decl.Modifiers
                .ParseModifiers()
                .WithDefaultVisibility(Parent is InterfaceInfo ?
                    Modifier.Public : Modifier.Private);
            
            Signature = $"{decl.ReturnType} {decl.Identifier.Text}" +
                $"{typeParams}({ParamSyntax()})";

            if (decl.HasLeadingTrivia)
                Documentation = GetDocumentation(decl.GetLeadingTrivia());

            ReturnType = decl.ReturnType.ToString();
        }
    }
}

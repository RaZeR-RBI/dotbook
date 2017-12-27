using DotBook.Utils;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static DotBook.Utils.Common;

namespace DotBook.Model.Members
{
    public class MethodInfo : MethodInfoBase
    {
        public MethodInfo(MethodDeclarationSyntax decl, IMemberContainer parent)
        {
            var paramTypes = decl.ParameterList.Parameters
                .Select(p => p.Type.ToString());
            var typeParams = Format(decl.TypeParameterList);

            Name = $"{decl.Identifier.Text}{typeParams}" +
                $"({string.Join(", ", paramTypes)})";
            _parameters = Parse(decl.ParameterList);
            _modifiers = decl.Modifiers
                .ParseModifiers()
                .WithDefaultVisibility(Modifier.Private);
            
            Signature = $"{decl.ReturnType} {decl.Identifier.Text}" +
                $"{typeParams}({ParamSyntax()})";

            if (decl.HasLeadingTrivia)
                Documentation = GetDocumentation(decl.GetLeadingTrivia());

            ReturnType = decl.ReturnType.ToString();
            Parent = parent;
        }
    }
}

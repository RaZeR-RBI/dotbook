using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static DotBook.Utils.Common;

namespace DotBook.Model.Members
{
    public class ConstructorInfo : MethodInfoBase
    {
        public ConstructorInfo(ConstructorDeclarationSyntax decl, 
            IMemberContainer parent)
        {
            var paramTypes = decl.ParameterList.Parameters
                .Select(p => p.Type.ToString());

            if (decl.HasLeadingTrivia)
                Documentation = GetDocumentation(decl.GetLeadingTrivia());

            Name = $"{decl.Identifier.Text}({string.Join(", ", paramTypes)})";
            _parameters = Parse(decl.ParameterList);
            _modifiers = decl.Modifiers
                .ParseModifiers()
                .WithDefaultVisibility(Modifier.Private);

            var paramSyntax = string.Join(",\n\t", _parameters);
            Signature = $"{decl.Identifier.Text}(\n\t{paramSyntax}\n)";


            ReturnType = "void";
            Parent = parent;
        }
    }
}

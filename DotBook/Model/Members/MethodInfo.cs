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
        public MethodInfo(MethodDeclarationSyntax decl, INameable parent)
        {
            var paramTypes = decl.ParameterList.Parameters
                .Select(p => p.Type.ToString());
            var typeParams = Format(decl.TypeParameterList);

            Name = $"{decl.Identifier.Text}{typeParams}" +
                $"({string.Join(", ", paramTypes)})";

            Signature = $"{decl.ReturnType} {decl.Identifier.Text}" +
                $"{typeParams}{decl.ParameterList}";

            _modifiers = decl.Modifiers
                .ParseModifiers()
                .WithDefaultVisibility(Modifier.Private);

            _parameters = Parse(decl.ParameterList);
            ReturnType = decl.ReturnType.ToString();
            Parent = parent;
        }
    }
}

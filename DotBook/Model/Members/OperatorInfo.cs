﻿using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static DotBook.Utils.Common;

namespace DotBook.Model.Members
{
    public class OperatorInfo : MethodInfoBase
    {
        public OperatorInfo(OperatorDeclarationSyntax decl, IMemberContainer parent)
        {
            var paramTypes = decl.ParameterList.Parameters
                .Select(p => p.Type.ToString());

            if (decl.HasLeadingTrivia)
                Documentation = GetDocumentation(decl.GetLeadingTrivia());

            ReturnType = decl.ReturnType.ToString();
            Name = $"{decl.OperatorToken}({string.Join(", ", paramTypes)})";
            Signature = $"{ReturnType} operator {decl.OperatorToken}" +
                $"{decl.ParameterList}";

            _modifiers = decl.Modifiers
                .ParseModifiers()
                .WithDefaultVisibility(Modifier.Private);

            _parameters = Parse(decl.ParameterList);
            Parent = parent;
        }
    }
}
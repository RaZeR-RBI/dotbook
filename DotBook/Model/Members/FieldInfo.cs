using DotBook.Model.Entities;
using DotBook.Processing;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;
using static DotBook.Utils.Common;

namespace DotBook.Model.Members
{
    public class FieldInfo : MemberInfoBase
    {
        public string Value { get; }

        public FieldInfo(FieldDeclarationSyntax decl, IMemberContainer parent)
        {
            var variable = decl.Declaration.Variables.First();
            if (decl.HasLeadingTrivia)
                Documentation = GetDocumentation(decl.GetLeadingTrivia());

            Name = variable.Identifier.Text;
            ReturnType = decl.Declaration.Type.ToString();
            _modifiers = decl.Modifiers
                .ParseModifiers()
                .WithDefaultVisibility(Modifier.Private);

            Parent = parent;
            Value = variable.Initializer?.Value?.ToString();
            Signature = $"{ReturnType} {Name}";
            if (Value != null) Signature += " = " + Value;
            Signature += ";";
        }
    }
}

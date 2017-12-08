using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBook.Model.Members
{
    public class ParameterInfo
    {
        public string Name { get; }
        public string Type { get; }

        public ParameterInfo(ParameterSyntax decl)
        {
            Name = decl.Identifier.Text;
            Type = decl.Type.ToString();
        }
    }
}

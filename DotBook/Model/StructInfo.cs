﻿using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBook.Model
{
    public class StructInfo : INameable, IModifiable, IPartial<StructDeclarationSyntax>
    {
        public IReadOnlyCollection<Modifier> Modifiers => throw new NotImplementedException();

        public string Name => throw new NotImplementedException();

        public string FullName => throw new NotImplementedException();

        public void Populate(StructDeclarationSyntax source)
        {
            throw new NotImplementedException();
        }
    }
}

using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBook.Model
{
    public class InterfaceInfo : INameable, IModifiable, IPartial<InterfaceDeclarationSyntax>
    {
        public string Name => throw new NotImplementedException();

        public string FullName => throw new NotImplementedException();

        public IReadOnlyCollection<Modifier> Modifiers => throw new NotImplementedException();

        public void Populate(InterfaceDeclarationSyntax source)
        {
            throw new NotImplementedException();
        }
    }
}

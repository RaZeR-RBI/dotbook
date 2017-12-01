using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBook.Model
{
    public class ClassInfo : INameable, IModifiable, IPartial<ClassDeclarationSyntax>
    {
        public IReadOnlyCollection<Modifier> Modifiers => throw new NotImplementedException();

        public string Name { get; }


        public void Populate(ClassDeclarationSyntax source)
        {
            throw new NotImplementedException();
        }
    }
}

using DotBook.Model;
using DotBook.Model.Entities;
using DotBook.Model.Members;
using DotBook.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace DotBook.Tests.Model.Members
{
    public class MethodInfoTest
    {
        private IReadOnlyCollection<MethodInfo> Act(params string[] sources) =>
            new SourceInfo(CompilationUnits.FromString(sources).ToList())
                .Namespaces.First()
                .Classes.First()
                .Methods;

        [Fact]
        public void ShouldHaveParametersAndReturnType()
        {
            var source = @"
                namespace MyAssembly
                {
                    class MyClass
                    {
                        void DoSomething();
                        int GetResult<T>(T input);
                    }
                }
            ";

            var methods = Act(source);

            Assert.Equal(2, methods.Count);
            Assert.Contains(methods,
                m => m.Name == "DoSomething()" && m.ReturnType == "void" &&
                m.Signature == "void DoSomething()" &&
                m.Parameters.Count == 0);
            Assert.Contains(methods,
                m => m.Name == "GetResult<T>(T)" && m.ReturnType == "int" &&
                m.Signature == "int GetResult<T>(T input)" &&
                m.Parameters.Single().Name == "input" &&
                m.Parameters.Single().Type == "T");
        }

        [Fact]
        public void ShouldHaveParent()
        {
            var source = @"
                namespace MyAssembly
                {
                    class MyClass
                    {
                        void DoSomething();
                    }
                }
            ";

            var info = Act(source).First();
            Assert.False(info.IsRoot());
            Assert.IsType<ClassInfo>(info.Parent);
        }
    }
}

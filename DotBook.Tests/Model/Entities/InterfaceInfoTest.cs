using DotBook.Model;
using DotBook.Model.Entities;
using DotBook.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace DotBook.Tests.Model.Entities
{
    public class InterfaceInfoTest
    {

        private IReadOnlyCollection<InterfaceInfo> Act(params string[] sources) =>
            new SourceInfo(CompilationUnits.FromString(sources).ToList())
                .Namespaces.First()
                .Interfaces;

        private IReadOnlyCollection<Modifier> Expect(params Modifier[] modifier) =>
            modifier.ToList();

        private IReadOnlyCollection<Modifier> Actual(
            IReadOnlyCollection<InterfaceInfo> interfaces,
            string name) =>
            interfaces.First(e => e.Name == name).Modifiers;

        [Fact]
        public void ShouldHandleModifiers()
        {
            /* Note: Top-level types cannot be private or protected,
             * it's only for test purposes
             */
            var source = @"
                namespace MyAssembly
                {
                    private interface ImPrivate { }
                    interface ImInternal { }
                    public interface ImPublic { }
                    internal interface ImInternalToo { }
                }
            ";

            var interfaces = Act(source);

            Assert.Equal(4, interfaces.Count);
            Assert.Equal(
                Expect(Modifier.Private), Actual(interfaces, "ImPrivate"));
            Assert.Equal(
                Expect(Modifier.Internal), Actual(interfaces, "ImInternal"));
            Assert.Equal(
                Expect(Modifier.Public), Actual(interfaces, "ImPublic"));
            Assert.Equal(
                Expect(Modifier.Internal), Actual(interfaces, "ImInternalToo"));
        }
        
        [Fact]
        public void ShouldIncludeTypeParameters()
        {
            var source = @"
                namespace MyAssembly
                {
                    interface MyInterface { }

                    interface MyInterface<T1, T2> 
                        where T1 : ISomething
                        where T2 : new()
                    { }
                }
            ";

            var interfaces = Act(source);

            Assert.Equal(2, interfaces.Count);
            Assert.Contains(interfaces, i => i.Name == "MyInterface");
            Assert.Contains(interfaces, i => i.Name == "MyInterface<T1, T2>");
        } 

        [Fact]
        public void ShouldIncludeBaseTypes()
        {
            var source = @"
                namespace MyAssembly
                {
                    interface MyInterface : IInterface, IInterfaceToo<T> { }
                }
            ";

            var info = Act(source).First();

            Assert.Equal(2, info.BaseTypes.Count);
            Assert.Contains("IInterface", info.BaseTypes);
            Assert.Contains("IInterfaceToo<T>", info.BaseTypes);
        }

        [Fact]
        public void ShouldHandleProperties()
        {
            var source = @"
                namespace MyAssembly
                {
                    interface MyInterface
                    {
                        int IntProp { get; };
                        string StringProp { get; };
                    }
                }
            ";

            var properties = Act(source).First().Properties;

            Assert.Equal(2, properties.Count);
            Assert.Contains(properties,
                p => p.Name == "IntProp" && p.Type == "int");
            Assert.Contains(properties,
                p => p.Name == "StringProp" && p.Type == "string");
        }

        [Fact]
        public void ShouldHandleIndexers()
        {
            var source = @"
                namespace MyAssembly
                {
                    interface MyInterface
                    {
                        int this[int index] { get; };
                        long this[string key] { get; };
                    }
                }
            ";

            var indexers = Act(source).First().Indexers;

            Assert.Equal(2, indexers.Count);
            Assert.Contains(indexers,
                i => i.Name == "int[int]" && i.ReturnType == "int" &&
                i.HasGetter && !i.HasSetter);
            Assert.Contains(indexers,
                i => i.Name == "long[string]" && i.ReturnType == "long" &&
                i.HasGetter && !i.HasSetter);
        }

        [Fact]
        public void ShouldHandleMethods()
        {
            var source = @"
                namespace MyAssembly
                {
                    interface MyInterface
                    {
                        void DoSomething();
                        int GetResult(string input);
                    }
                }
            ";

            var methods = Act(source).First().Methods;

            Assert.Equal(2, methods.Count);
            Assert.Contains(methods,
                m => m.Name == "DoSomething()" && m.ReturnType == "void" &&
                m.Signature == "void DoSomething()" &&
                m.Parameters.Count == 0);
            Assert.Contains(methods,
                m => m.Name == "GetResult(string)" && m.ReturnType == "int" &&
                m.Signature == "int GetResult(string input)" &&
                m.Parameters.Single().Name == "input" &&
                m.Parameters.Single().Type == "string");
        }
        
        [Fact]
        public void ShouldIncludeDocumentation()
        {
            var source = @"
                namespace MyAssembly
                {
                    /// <summary>
                    /// Useful interface
                    /// </summary>
                    interface MyInterface { }
                }
            ";

            var info = Act(source).First();

            Assert.NotNull(info.Documentation);
            Assert.Contains("Useful interface", info.Documentation);
        }
        
        [Fact]
        public void ShouldHaveParent()
        {
            var source = @"
                namespace MyAssembly
                {
                    interface MyInterface { }
                }
            ";

            var info = Act(source).First();

            Assert.False(info.IsRoot());
            Assert.IsType<NamespaceInfo>(info.Parent);
        }
    }
}

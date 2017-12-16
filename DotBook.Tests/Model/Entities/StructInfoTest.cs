using DotBook.Model;
using DotBook.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace DotBook.Tests.Model.Entities
{
    public class StructInfoTest
    {
        private IReadOnlyCollection<StructInfo> Act(params string[] sources) =>
            new SourceInfo(CompilationUnits.FromString(sources).ToList())
                .Namespaces.First()
                .Structs;

        private IReadOnlyCollection<Modifier> Expect(params Modifier[] modifier) =>
            modifier.ToList();

        private IReadOnlyCollection<Modifier> Actual(
            IReadOnlyCollection<StructInfo> structs,
            string name) =>
            structs.First(s => s.Name == name).Modifiers;

        [Fact]
        public void ShouldHandleModifiers()
        {
            /* Note: Top-level types cannot be private,
             * it's only for test purposes
             */
            var source = @"
                namespace MyAssembly
                {
                    private struct ImPrivate { }
                    struct ImInternal { }
                    public struct ImPublic { }
                    internal struct ImInternalToo { }
                    unsafe struct ImUnsafe { }
                }
            ";

            var structs = Act(source);

            Assert.Equal(5, structs.Count);
            Assert.Equal(
                Expect(Modifier.Private), Actual(structs, "ImPrivate"));
            Assert.Equal(
                Expect(Modifier.Internal), Actual(structs, "ImInternal"));
            Assert.Equal(
                Expect(Modifier.Public), Actual(structs, "ImPublic"));
            Assert.Equal(
                Expect(Modifier.Internal), Actual(structs, "ImInternalToo"));
            Assert.Equal(
                Expect(Modifier.Internal, Modifier.Unsafe),
                Actual(structs, "ImUnsafe"));
        }

        [Fact]
        public void ShouldHandleInnerStructures()
        {
            var source = @"
                namespace MyAssembly
                {
                    struct OuterStruct
                    {
                        class InnerClass {}
                        struct InnerStruct {}
                        interface InnerInterface {}
                        enum InnerEnum {}
                    }
                }
            ";

            var info = Act(source).First();

            Assert.Single(info.Classes);
            Assert.Contains(info.Classes,
                c => c.FullName == "MyAssembly.OuterStruct.InnerClass");
            Assert.Single(info.Structs);
            Assert.Contains(info.Structs,
                s => s.FullName == "MyAssembly.OuterStruct.InnerStruct");
            Assert.Single(info.Interfaces);
            Assert.Contains(info.Interfaces,
                i => i.FullName == "MyAssembly.OuterStruct.InnerInterface");
            Assert.Single(info.Enums);
            Assert.Contains(info.Enums,
                e => e.FullName == "MyAssembly.OuterStruct.InnerEnum");
        }

        [Fact]
        public void ShouldHandleFields()
        {
            var source = @"
                namespace MyAssembly
                {
                    struct MyStruct
                    {
                        int IntField;
                        string StringField;
                        (byte, long) MyTuple;
                    }
                }
            ";

            var fields = Act(source).First().Fields;

            Assert.Equal(3, fields.Count);
            Assert.Contains(fields,
                f => f.Name == "IntField" && f.Type == "int");
            Assert.Contains(fields,
                f => f.Name == "StringField" && f.Type == "string");
            Assert.Contains(fields,
                f => f.Name == "MyTuple" && f.Type == "(byte, long)");
        }
        
        [Fact]
        public void ShouldHandleProperties()
        {
            var source = @"
                namespace MyAssembly
                {
                    struct MyStruct
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
                    struct MyStruct
                    {
                        int this[int index] { get; };
                        long this[string key] { get; };
                    }
                }
            ";

            var indexers = Act(source).First().Indexers;

            Assert.Equal(2, indexers.Count);
            Assert.Contains(indexers,
                i => i.Name == "int[int]" && i.Type == "int" &&
                i.HasGetter && !i.HasSetter);
            Assert.Contains(indexers,
                i => i.Name == "long[string]" && i.Type == "long" &&
                i.HasGetter && !i.HasSetter);
        }
        
        [Fact]
        public void ShouldHandleMethods()
        {
            var source = @"
                namespace MyAssembly
                {
                    struct MyStruct
                    {
                        void DoSomething() { }
                        int GetResult(string input) { return 0; }
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
        public void ShouldHandleConstructors()
        {
            var source = @"
                namespace MyAssembly
                {
                    struct MyStruct
                    {
                        MyStruct() { }
                        MyStruct(string input) { }
                    }
                }
            ";

            var constructors = Act(source).First().Constructors;

            Assert.Equal(2, constructors.Count);
            Assert.Contains(constructors,
                c => c.Name == "MyStruct()" && c.ReturnType == "void" &&
                c.Signature == "MyStruct()" &&
                c.Parameters.Count == 0);
            Assert.Contains(constructors,
                c => c.Name == "MyStruct(string)" && c.ReturnType == "void" &&
                c.Signature == "MyStruct(string input)" &&
                c.Parameters.Single().Name == "input" &&
                c.Parameters.Single().Type == "string");
        }

        [Fact]
        public void ShouldHandleOperators()
        {
            var source = @"
                namespace MyAssembly
                {
                    struct MyStruct
                    {
                        public static int operator +(byte one, byte two) { }
                    }
                }
            ";

            var actual = Act(source).First().Operators.First();

            Assert.Equal("int operator +(byte one, byte two)", actual.Signature);
        }

        [Fact]
        public void ShouldIncludeTypeParameters()
        {
            var source = @"
                namespace MyAssembly
                {
                    struct MyStruct { }

                    struct MyStruct<T1, T2> 
                        where T1 : ISomething
                        where T2 : new()
                    { }
                }
            ";

            var structs = Act(source);

            Assert.Equal(2, structs.Count);
            Assert.Contains(structs, s => s.Name == "MyStruct");
            Assert.Contains(structs, s => s.Name == "MyStruct<T1, T2>");
        }

        [Fact]
        public void ShouldIncludeBaseTypes()
        {
            var source = @"
                namespace MyAssembly
                {
                    struct MyStruct : IInterface, IInterfaceToo<T> { }
                }
            ";

            var info = Act(source).First();

            Assert.Equal(2, info.BaseTypes.Count);
            Assert.Contains("IInterface", info.BaseTypes);
            Assert.Contains("IInterfaceToo<T>", info.BaseTypes);
        }

        [Fact]
        public void ShouldIncludeDocumentation()
        {
            var source = @"
                namespace MyAssembly
                {
                    /// <summary>
                    /// Useful struct
                    /// </summary>
                    struct MyStruct { }
                }
            ";

            var info = Act(source).First();

            Assert.NotNull(info.Documentation);
            Assert.Contains("Useful struct", info.Documentation);
        }
    }
}

using DotBook.Model;
using DotBook.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace DotBook.Tests.Model.Entities
{
    public class ClassInfoTest
    {
        private IReadOnlyCollection<ClassInfo> Act(params string[] sources) =>
            new SourceInfo(CompilationUnits.FromString(sources).ToList())
                .Namespaces.First()
                .Classes;

        private IReadOnlyCollection<Modifier> Expect(params Modifier[] modifier) =>
            modifier.ToList();

        private IReadOnlyCollection<Modifier> Actual(
            IReadOnlyCollection<ClassInfo> classes,
            string name) =>
            classes.First(c => c.Name == name).Modifiers;

        [Fact]
        public void ShouldHandleModifiers()
        {
            /* Note: Top-level types cannot be private or protected,
             * it's only for test purposes
             */
            var source = @"
                namespace MyAssembly
                {
                    private class ImPrivate { }
                    class ImInternal { }
                    protected class ImProtected { }
                    public static class ImPublicStatic { }
                    sealed class ImSealed { }
                    abstract class ImAbstract { }
                    unsafe class ImUnsafe { }
                }
            ";

            var classes = Act(source);

            Assert.Equal(7, classes.Count);
            Assert.Equal(
                Expect(Modifier.Private),
                Actual(classes, "ImPrivate"));
            Assert.Equal(
                Expect(Modifier.Internal),
                Actual(classes, "ImInternal"));
            Assert.Equal(
                Expect(Modifier.Protected),
                Actual(classes, "ImProtected"));
            Assert.Equal(
                Expect(Modifier.Public, Modifier.Static),
                Actual(classes, "ImPublicStatic"));
            Assert.Equal(
                Expect(Modifier.Internal, Modifier.Sealed),
                Actual(classes, "ImSealed"));
            Assert.Equal(
                Expect(Modifier.Internal, Modifier.Abstract),
                Actual(classes, "ImAbstract"));
            Assert.Equal(
                Expect(Modifier.Internal, Modifier.Unsafe),
                Actual(classes, "ImUnsafe"));
        }

        [Fact]
        public void ShouldHandleInnerStructures()
        {
            var source = @"
                namespace MyAssembly
                {
                    class OuterClass
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
                c => c.FullName == "MyAssembly.OuterClass.InnerClass");
            Assert.Single(info.Structs);
            Assert.Contains(info.Structs,
                s => s.FullName == "MyAssembly.OuterClass.InnerStruct");
            Assert.Single(info.Interfaces);
            Assert.Contains(info.Interfaces,
                i => i.FullName == "MyAssembly.OuterClass.InnerInterface");
            Assert.Single(info.Enums);
            Assert.Contains(info.Enums,
                e => e.FullName == "MyAssembly.OuterClass.InnerEnum");
        }

        [Fact]
        public void ShouldHandleFields()
        {
            var source = @"
                namespace MyAssembly
                {
                    class MyClass
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
                    class MyClass
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
                    class MyClass
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
                    class MyClass
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
                    class MyClass
                    {
                        MyClass() { }
                        MyClass(string input) { }
                    }
                }
            ";

            var constructors = Act(source).First().Constructors;

            Assert.Equal(2, constructors.Count);
            Assert.Contains(constructors,
                c => c.Name == "MyClass()" && c.ReturnType == "void" &&
                c.Signature == "MyClass()" &&
                c.Parameters.Count == 0);
            Assert.Contains(constructors,
                c => c.Name == "MyClass(string)" && c.ReturnType == "void" &&
                c.Signature == "MyClass(string input)" &&
                c.Parameters.Single().Name == "input" &&
                c.Parameters.Single().Type == "string");
        }

        [Fact]
        public void ShouldIncludeTypeParameters()
        {
            var source = @"
                namespace MyAssembly
                {
                    class MyClass { }

                    class MyClass<T1, T2> 
                        where T1 : ISomething
                        where T2 : new()
                    { }
                }
            ";

            var classes = Act(source);

            Assert.Equal(2, classes.Count);
            Assert.Contains(classes, c => c.Name == "MyClass");
            Assert.Contains(classes, c => c.Name == "MyClass<T1, T2>");
        }

        [Fact]
        public void ShouldIncludeBaseTypes()
        {
            var source = @"
                namespace MyAssembly
                {
                    class MyClass : IInterface, BaseClass<T> { }
                }
            ";

            var info = Act(source).First();

            Assert.Equal(2, info.BaseTypes.Count);
            Assert.Contains("IInterface", info.BaseTypes);
            Assert.Contains("BaseClass<T>", info.BaseTypes);
        }
    }
}

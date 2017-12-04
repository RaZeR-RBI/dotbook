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
                    }
                }
            ";

            var fields = Act(source).First().Fields;

            Assert.Equal(2, fields.Count);
            Assert.Contains(fields,
                f => f.Name == "IntField" && f.Type == "int");
            Assert.Contains(fields,
                f => f.Name == "StringField" && f.Type == "string");
        }
    }
}

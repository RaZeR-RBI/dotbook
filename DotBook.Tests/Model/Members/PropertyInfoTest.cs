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
    public class PropertyInfoTest
    {
        private IReadOnlyCollection<PropertyInfo> Act(params string[] sources) =>
            new SourceInfo(CompilationUnits.FromString(sources).ToList())
                .Namespaces.First()
                .Classes.First()
                .Properties;

        private IReadOnlyCollection<Modifier> Expect(params Modifier[] modifier) =>
            modifier.ToList();

        private IReadOnlyCollection<Modifier> Actual(
            IReadOnlyCollection<PropertyInfo> properties,
            string name) =>
            properties.First(s => s.Name == name).Modifiers;

        [Fact]
        public void ShouldHandleModifiers()
        {
            var source = @"
                namespace MyAssembly
                {
                    class MyClass
                    {
                        private int PrivateInt { get; };
                        long PrivateLong { get; };
                        protected string ProtectedString { get; };
                        internal static MyType InternalStaticType { get; };
                        public DateTime PublicDateTime { get; };
                    }
                }
            ";

            var properties = Act(source);

            Assert.Equal(5, properties.Count);
            Assert.Equal(
                Expect(Modifier.Private), Actual(properties, "PrivateInt"));
            Assert.Equal(
                Expect(Modifier.Private), Actual(properties, "PrivateLong"));
            Assert.Equal(
                Expect(Modifier.Protected), Actual(properties, "ProtectedString"));
            Assert.Equal(
                Expect(Modifier.Internal, Modifier.Static), 
                Actual(properties, "InternalStaticType"));
            Assert.Equal(
                Expect(Modifier.Public), Actual(properties, "PublicDateTime"));
        }

        [Fact]
        public void ShouldHaveTypeAndAccessors()
        {
            var source = @"
                namespace MyAssembly
                {
                    class MyClass
                    {
                        int ReadableInt { get; };
                        long SettableLong { set; };
                        List<string> ReadableList => null;
                        bool SomeFlag { get; set; }
                    }
                }
            ";

            var props = Act(source);

            Assert.Equal(4, props.Count);
            Assert.Contains(props, f => f.Name == "ReadableInt" 
                && f.Type == "int" && f.HasGetter && !f.HasSetter);
            Assert.Contains(props, f => f.Name == "SettableLong"
                && f.Type == "long" && !f.HasGetter && f.HasSetter);
            Assert.Contains(props, f => f.Name == "ReadableList"
                && f.Type == "List<string>" && f.HasGetter && !f.HasSetter);
            Assert.Contains(props, f => f.Name == "SomeFlag"
                && f.Type == "bool" && f.HasGetter && f.HasSetter);
        }

        [Fact]
        public void ShouldHaveParent()
        {
            var source = @"
                namespace MyAssembly
                {
                    class MyClass
                    {
                        int ReadableInt { get; };
                    }
                }
            ";

            var info = Act(source).First();
            Assert.False(info.IsRoot());
            Assert.IsType<ClassInfo>(info.Parent);
        }
    }
}

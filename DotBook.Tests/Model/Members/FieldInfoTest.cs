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
    public class FieldInfoTest
    {
        private IReadOnlyCollection<FieldInfo> Act(params string[] sources) =>
            new SourceInfo(CompilationUnits.FromString(sources).ToList())
                .Namespaces.First()
                .Classes.First()
                .Fields;

        private IReadOnlyCollection<Modifier> Expect(params Modifier[] modifier) =>
            modifier.ToList();

        private IReadOnlyCollection<Modifier> Actual(
            IReadOnlyCollection<FieldInfo> fields,
            string name) =>
            fields.First(s => s.Name == name).Modifiers;

        [Fact]
        public void ShouldHandleModifiers()
        {
            var source = @"
                namespace MyAssembly
                {
                    class MyClass
                    {
                        private int PrivateInt;
                        long PrivateLong;
                        protected string ProtectedString;
                        internal static MyType InternalStaticType;
                        public DateTime PublicDateTime;
                    }
                }
            ";

            var fields = Act(source);

            Assert.Equal(5, fields.Count);
            Assert.Equal(
                Expect(Modifier.Private), Actual(fields, "PrivateInt"));
            Assert.Equal(
                Expect(Modifier.Private), Actual(fields, "PrivateLong"));
            Assert.Equal(
                Expect(Modifier.Protected), Actual(fields, "ProtectedString"));
            Assert.Equal(
                Expect(Modifier.Internal, Modifier.Static), 
                Actual(fields, "InternalStaticType"));
            Assert.Equal(
                Expect(Modifier.Public), Actual(fields, "PublicDateTime"));
        }

        [Fact]
        public void ShouldHaveTypeAndValueIfDeclared()
        {
            var source = @"
                namespace MyAssembly
                {
                    class MyClass
                    {
                        int SomeInt;
                        long DefinedLong = 1337;
                    }
                }
            ";

            var fields = Act(source);

            Assert.Equal(2, fields.Count);
            Assert.Contains(fields, 
                f => f.Name == "SomeInt" && f.Value == null && f.ReturnType == "int");
            Assert.Contains(fields, 
                f => f.Name == "DefinedLong" && f.Value == "1337" && f.ReturnType == "long");
        }
        
        [Fact]
        public void ShouldHaveParent()
        {
            var source = @"
                namespace MyAssembly
                {
                    class MyClass
                    {
                        int SomeInt;
                    }
                }
            ";

            var info = Act(source).First();
            Assert.False(info.IsRoot());
            Assert.IsType<ClassInfo>(info.Parent);
        }
    }
}

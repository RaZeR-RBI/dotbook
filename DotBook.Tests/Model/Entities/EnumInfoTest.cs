using DotBook.Model;
using DotBook.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace DotBook.Tests.Model.Entities
{
    public class EnumInfoTest
    {
        private IReadOnlyCollection<EnumInfo> Act(params string[] sources) =>
            new SourceInfo(CompilationUnits.FromString(sources).ToList())
                .Namespaces.First()
                .Enums;

        private IReadOnlyCollection<Modifier> Expect(params Modifier[] modifier) =>
            modifier.ToList();

        private IReadOnlyCollection<Modifier> Actual(
            IReadOnlyCollection<EnumInfo> enums,
            string name) =>
            enums.First(e => e.Name == name).Modifiers;

        [Fact]
        public void ShouldHandleModifiers()
        {
            /* Note: Top-level types cannot be private or protected,
             * it's only for test purposes
             */
            var source = @"
                namespace MyAssembly
                {
                    private enum ImPrivate { }
                    enum ImInternal { }
                    public enum ImPublic { }
                    internal enum ImInternalToo { }
                }
            ";

            var enums = Act(source);

            Assert.Equal(4, enums.Count);
            Assert.Equal(
                Expect(Modifier.Private), Actual(enums, "ImPrivate"));
            Assert.Equal(
                Expect(Modifier.Internal), Actual(enums, "ImInternal"));
            Assert.Equal(
                Expect(Modifier.Public), Actual(enums, "ImPublic"));
            Assert.Equal(
                Expect(Modifier.Internal), Actual(enums, "ImInternalToo"));
        }

        [Fact]
        public void ShouldSupportUnderlyingType()
        {
            var source = @"
                namespace MyAssembly
                {
                    enum DefaultEnum { }
                    enum LongEnum : long { }
                    enum ByteEnum : byte { }
                }
            ";

            var enums = Act(source);

            Assert.Equal(3, enums.Count);
            Assert.Contains(enums, 
                e => e.Name == "DefaultEnum" && e.UnderlyingType == "int");
            Assert.Contains(enums,
                e => e.Name == "LongEnum" && e.UnderlyingType == "long");
            Assert.Contains(enums,
                e => e.Name == "ByteEnum" && e.UnderlyingType == "byte");
        }

        [Fact]
        public void ShouldParseMembers()
        {
            var source = @"
                namespace MyAssembly
                {
                    enum MyEnum
                    {
                        One = 1,
                        Two,
                        Four = 4,
                        FourToo = Four
                    }
                }
            ";

            var members = Act(source).First().Values;

            Assert.Equal(4, members.Count);
            Assert.Contains(members, m => m.Key == "One" && m.Value == "1");
            Assert.Contains(members, m => m.Key == "Two" && m.Value == "");
            Assert.Contains(members, m => m.Key == "Four" && m.Value == "4");
            Assert.Contains(members, m => m.Key == "FourToo" && m.Value == "Four");
        }


        [Fact]
        public void ShouldIncludeDocumentation()
        {
            var source = @"
                namespace MyAssembly
                {
                    /// <summary>
                    /// Useful enum
                    /// </summary>
                    enum MyEnum { }
                }
            ";

            var info = Act(source).First();

            Assert.NotNull(info.Documentation);
            Assert.Contains("Useful enum", info.Documentation);
        }
    }
}

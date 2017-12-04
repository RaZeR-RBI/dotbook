using DotBook.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace DotBook.Tests.Model
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
            var source = @"
                namespace MyAssembly
                {
                    private struct ImPrivate { }
                    struct ImInternal { }
                    public struct ImPublic { }
                    internal struct ImInternalToo { }
                }
            ";

            var structs = Act(source);

            Assert.Equal(4, structs.Count);
            Assert.Equal(
                Expect(Modifier.Private), Actual(structs, "ImPrivate"));
            Assert.Equal(
                Expect(Modifier.Internal), Actual(structs, "ImInternal"));
            Assert.Equal(
                Expect(Modifier.Public), Actual(structs, "ImPublic"));
            Assert.Equal(
                Expect(Modifier.Internal), Actual(structs, "ImInternalToo"));
        }
    }
}

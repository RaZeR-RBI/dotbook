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
    }
}

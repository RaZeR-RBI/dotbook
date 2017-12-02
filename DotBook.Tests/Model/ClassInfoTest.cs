using DotBook.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace DotBook.Tests.Model
{
    public class ClassInfoTest
    {
        private IReadOnlyCollection<ClassInfo> Act(params string[] sources) =>
            new SourceInfo(CompilationUnits.FromString(sources).ToList())
                .Namespaces.First()
                .Classes;

        private IReadOnlyCollection<Modifier> Expect(params Modifier[] modifier) =>
            modifier.ToList();

        private IReadOnlyCollection<Modifier> Actual(IReadOnlyCollection<ClassInfo> classes,
            string name) =>
            classes.First(c => c.Name == name).Modifiers;

        [Fact]
        public void ShouldHandleModifiers()
        {
            var source = @"
                namespace MyAssembly
                {
                    private class ImPrivate { }
                    class ImPrivateToo { }
                    protected class ImProtected { }
                    public static class ImPublicStatic { }
                    internal sealed class ImInternalSealed { }
                }
            ";

            var classes = Act(source);

            Assert.Equal(5, classes.Count);
            Assert.Equal(
                Expect(Modifier.Private),
                Actual(classes, "ImPrivate"));
            Assert.Equal(
                Expect(Modifier.Private),
                Actual(classes, "ImPrivateToo"));
            Assert.Equal(
                Expect(Modifier.Protected),
                Actual(classes, "ImProtected"));
            Assert.Equal(
                Expect(Modifier.Public, Modifier.Static),
                Actual(classes, "ImPublicStatic"));
            Assert.Equal(
                Expect(Modifier.Internal, Modifier.Sealed),
                Actual(classes, "ImInternalSealed"));
        }
    }
}

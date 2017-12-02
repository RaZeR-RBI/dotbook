using DotBook.Model;
using System;
using System.Linq;
using Xunit;

namespace DotBook.Tests
{
    public class SourceInfoTest
    {
        private SourceInfo Act(params string[] sources) =>
            new SourceInfo(CompilationUnits.FromString(sources).ToList());

        [Fact]
        public void ShouldFindNamespaces()
        {
            var source = "namespace MySuperAssembly { }";

            var sourceInfo = Act(source);

            Assert.Single(sourceInfo.Namespaces);
            Assert.Equal("MySuperAssembly", sourceInfo.Namespaces.First().FullName);
        }

        [Fact]
        public void ShouldHandleMultipleNamespaces()
        {
            var source1 = "namespace MySuperAssembly { }";
            var source2 = "namespace MySuperAssembly.CoolFeature { }";
            var source3 = "namespace MySuperAssembly { /* blah */ }";

            var sourceInfo = Act(source1, source2, source3);

            Assert.Equal(2, sourceInfo.Namespaces.Count);
            Assert.Contains(sourceInfo.Namespaces, n => n.FullName == "MySuperAssembly");
            Assert.Contains(sourceInfo.Namespaces, n => n.FullName == "MySuperAssembly.CoolFeature");
        }
    }
}

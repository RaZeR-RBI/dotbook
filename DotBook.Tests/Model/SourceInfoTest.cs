using DotBook.Model;
using DotBook.Processing;
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

            var info = Act(source);

            Assert.Single(info.Namespaces);
            Assert.Single(info.ChildrenNodes);
            Assert.True(info.IsRoot());
            Assert.Contains(info.Namespaces, n => n.FullName == "MySuperAssembly");
        }

        [Fact]
        public void ShouldHandleMultipleNamespaces()
        {
            var source1 = "namespace MySuperAssembly { }";
            var source2 = "namespace MySuperAssembly.CoolFeature { }";
            var source3 = "namespace MySuperAssembly { /* blah */ }";

            var info = Act(source1, source2, source3);

            Assert.Equal(2, info.Namespaces.Count);
            Assert.Equal(2, info.ChildrenNodes.Count());
            Assert.True(info.IsRoot());
            Assert.Contains(info.Namespaces, n => n.FullName == "MySuperAssembly");
            Assert.Contains(info.Namespaces, n => n.FullName == "MySuperAssembly.CoolFeature");
        }
    }
}

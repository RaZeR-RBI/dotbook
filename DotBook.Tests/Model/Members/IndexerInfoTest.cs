using DotBook.Model;
using DotBook.Model.Members;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace DotBook.Tests.Model.Members
{
    public class IndexerInfoTest
    {
        private IReadOnlyCollection<IndexerInfo> Act(params string[] sources) =>
            new SourceInfo(CompilationUnits.FromString(sources).ToList())
                .Namespaces.First()
                .Classes.First()
                .Indexers;

        private IReadOnlyCollection<Modifier> Expect(params Modifier[] modifier) =>
            modifier.ToList();

        private IReadOnlyCollection<Modifier> Actual(
            IReadOnlyCollection<IndexerInfo> indexers,
            string name) =>
            indexers.First(s => s.Name == name).Modifiers;

        [Fact]
        public void ShouldHandleModifiers()
        {
            var source = @"
                namespace MyAssembly
                {
                    class MyClass
                    {
                        private int this[int index];
                        long this[string key];
                        protected string this[string key, string format];
                    }
                }
            ";

            var indexers = Act(source);

            Assert.Equal(3, indexers.Count);
            Assert.Equal(
                Expect(Modifier.Private), Actual(indexers, "int[int]"));
            Assert.Equal(
                Expect(Modifier.Private), Actual(indexers, "long[string]"));
            Assert.Equal(
                Expect(Modifier.Protected), Actual(indexers, "string[string, string]"));
        }

        [Fact]
        public void ShouldHaveTypeAndAccessors()
        {
            var source = @"
                namespace MyAssembly
                {
                    class MyClass
                    {
                        int this[int index] { get; };
                        long this[string key] { set; };
                        List<string> this[Func<string, bool> predicate]  => null;
                        bool this[Func<string, bool> predicate] { get; set; }
                    }
                }
            ";

            var props = Act(source);

            Assert.Equal(4, props.Count);
            Assert.Contains(props, f => f.Name == "int[int]" 
                && f.Type == "int" && f.HasGetter && !f.HasSetter);
            Assert.Contains(props, f => f.Name == "long[string]"
                && f.Type == "long" && !f.HasGetter && f.HasSetter);
            Assert.Contains(props, f => f.Name == "List<string>[Func<string, bool>]"
                && f.Type == "List<string>" && f.HasGetter && !f.HasSetter);
            Assert.Contains(props, f => f.Name == "bool[Func<string, bool>]"
                && f.Type == "bool" && f.HasGetter && f.HasSetter);
        }
    }
}

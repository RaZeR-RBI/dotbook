using DotBook.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace DotBook.Tests.Model
{
    public class NamespaceInfoTest
    {
        private NamespaceInfo Act(params string[] sources) =>
            new SourceInfo(CompilationUnits.FromString(sources).ToList())
                .Namespaces.First();

        [Fact]
        public void ShouldFindClasses()
        {
            var source = @"
                namespace MyAssembly
                {
                    public class PreciousClass { }
                }
            ";

            var classes = Act(source).Classes;

            Assert.Single(classes);
            Assert.Contains(classes, c => c.FullName == "MyAssembly.PreciousClass");
        }

        [Fact]
        public void ShouldHandlePartialClasses()
        {
            var source1 = @"
                namespace MyAssembly
                {
                    public partial class PreciousClass { }
                }
            ";

            var source2 = @"
                namespace MyAssembly
                {
                    public partial class PreciousClass { }
                    public class OtherPreciousClass { }
                }
            ";

            var classes = Act(source1, source2).Classes;

            Assert.Equal(2, classes.Count);
            Assert.Contains(classes, c => c.FullName == "MyAssembly.PreciousClass");
            Assert.Contains(classes, c => c.FullName == "MyAssembly.OtherPreciousClass");
        }

        [Fact]
        public void ShouldFindStructs()
        {
            var source = @"
                namespace MyAssembly
                {
                    public struct PreciousStruct { }
                }
            ";

            var structs = Act(source).Structs;

            Assert.Single(structs);
            Assert.Contains(structs, s => s.FullName == "MyAssembly.PreciousStruct");
        }

        [Fact]
        public void ShouldHandlePartialStructs()
        {
            var source1 = @"
                namespace MyAssembly
                {
                    public partial struct PreciousStruct { }
                }
            ";

            var source2 = @"
                namespace MyAssembly
                {
                    public partial struct PreciousStruct { }
                    public struct OtherPreciousStruct { }
                }
            ";

            var structs = Act(source1, source2).Structs;

            Assert.Equal(2, structs.Count);
            Assert.Contains(structs, s => s.FullName == "MyAssembly.PreciousStruct");
            Assert.Contains(structs, s => s.FullName == "MyAssembly.OtherPreciousStruct");
        }

        [Fact]
        public void ShouldFindInterfaces()
        {
            var source = @"
                namespace MyAssembly
                {
                    public interface IPrecious { }
                }
            ";

            var interfaces = Act(source).Interfaces;

            Assert.Single(interfaces);
            Assert.Contains(interfaces, i => i.FullName == "MyAssembly.IPrecious");
        }

        [Fact]
        public void ShouldHandlePartialInterfaces()
        {
            var source1 = @"
                namespace MyAssembly
                {
                    public partial interface IPrecious { }
                }
            ";

            var source2 = @"
                namespace MyAssembly
                {
                    public partial interface IPrecious { }
                    public interface IOtherPrecious { }
                }
            ";

            var interfaces = Act(source1, source2).Interfaces;

            Assert.Equal(2, interfaces.Count);
            Assert.Contains(interfaces, i => i.FullName == "MyAssembly.IPrecious");
            Assert.Contains(interfaces, i => i.FullName == "MyAssembly.IOtherPrecious");
        }

        [Fact]
        public void ShouldFindEnums()
        {
            var source = @"
                namespace MyAssembly
                {
                    public enum PreciousEnum { }
                }
            ";

            var enums = Act(source).Enums;

            Assert.Single(enums);
            Assert.Contains(enums, e => e.FullName == "MyAssembly.PreciousEnum");
        }
    }
}

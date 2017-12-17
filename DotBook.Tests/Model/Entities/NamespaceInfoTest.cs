using DotBook.Model;
using DotBook.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace DotBook.Tests.Model.Entities
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

            var ns = Act(source);
            var classes = ns.Classes;

            Assert.Single(ns.ChildrenNodes);
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

            var ns = Act(source1, source2);
            var classes = ns.Classes;
            
            Assert.Equal(2, ns.ChildrenNodes.Count());
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

            var ns = Act(source);
            var structs = ns.Structs;

            Assert.Single(ns.ChildrenNodes);
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

            var ns = Act(source1, source2);
            var structs = ns.Structs;

            Assert.Equal(2, ns.ChildrenNodes.Count());
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

            var ns = Act(source);
            var interfaces = ns.Interfaces;

            Assert.Single(ns.ChildrenNodes);
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

            var ns = Act(source1, source2);
            var interfaces = ns.Interfaces;

            Assert.Equal(2, ns.ChildrenNodes.Count());
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
                    public enum OtherEnum { }
                }
            ";

            var ns = Act(source);
            var enums = ns.Enums;

            Assert.Equal(2, ns.ChildrenNodes.Count());
            Assert.Equal(2, enums.Count);
            Assert.Contains(enums, e => e.FullName == "MyAssembly.PreciousEnum");
            Assert.Contains(enums, e => e.FullName == "MyAssembly.OtherEnum");
        }

        [Fact]
        public void ShouldHaveSourceInfoAsParent()
        {
            var source = @"namespace MyAssembly { }";

            var ns = Act(source);

            Assert.False(ns.IsRoot());
            Assert.IsType<SourceInfo>(ns.ParentNode);
        }
    }
}

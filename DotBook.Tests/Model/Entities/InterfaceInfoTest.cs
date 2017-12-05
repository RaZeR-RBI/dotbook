using DotBook.Model;
using DotBook.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace DotBook.Tests.Model.Entities
{
    public class InterfaceInfoTest
    {

        private IReadOnlyCollection<InterfaceInfo> Act(params string[] sources) =>
            new SourceInfo(CompilationUnits.FromString(sources).ToList())
                .Namespaces.First()
                .Interfaces;

        private IReadOnlyCollection<Modifier> Expect(params Modifier[] modifier) =>
            modifier.ToList();

        private IReadOnlyCollection<Modifier> Actual(
            IReadOnlyCollection<InterfaceInfo> Interfaces,
            string name) =>
            Interfaces.First(e => e.Name == name).Modifiers;

        [Fact]
        public void ShouldHandleModifiers()
        {
            /* Note: Top-level types cannot be private or protected,
             * it's only for test purposes
             */
            var source = @"
                namespace MyAssembly
                {
                    private interface ImPrivate { }
                    interface ImInternal { }
                    public interface ImPublic { }
                    internal interface ImInternalToo { }
                }
            ";

            var interfaces = Act(source);

            Assert.Equal(4, interfaces.Count);
            Assert.Equal(
                Expect(Modifier.Private), Actual(interfaces, "ImPrivate"));
            Assert.Equal(
                Expect(Modifier.Internal), Actual(interfaces, "ImInternal"));
            Assert.Equal(
                Expect(Modifier.Public), Actual(interfaces, "ImPublic"));
            Assert.Equal(
                Expect(Modifier.Internal), Actual(interfaces, "ImInternalToo"));
        }
        
        [Fact]
        public void ShouldIncludeTypeParameters()
        {
            var source = @"
                namespace MyAssembly
                {
                    interface MyInterface { }

                    interface MyInterface<T1, T2> 
                        where T1 : ISomething
                        where T2 : new()
                    { }
                }
            ";

            var interfaces = Act(source);

            Assert.Equal(2, interfaces.Count);
            Assert.Contains(interfaces, i => i.Name == "MyInterface");
            Assert.Contains(interfaces, i => i.Name == "MyInterface<T1, T2>");
        } 

        [Fact]
        public void ShouldIncludeBaseTypes()
        {
            var source = @"
                namespace MyAssembly
                {
                    interface MyInterface : IInterface, IInterfaceToo<T> { }
                }
            ";

            var info = Act(source).First();

            Assert.Equal(2, info.BaseTypes.Count);
            Assert.Contains("IInterface", info.BaseTypes);
            Assert.Contains("IInterfaceToo<T>", info.BaseTypes);
        }
    }
}

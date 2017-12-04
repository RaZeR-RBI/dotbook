using DotBook.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using static System.Collections.Generic.KeyValuePair;

namespace DotBook.Tests.Utils
{
    public class OrderedDictionaryTest
    {
        private OrderedDictionary<string, int> Act(
            params KeyValuePair<string, int>[] pairs) =>
            new OrderedDictionary<string, int>(pairs);

        [Fact]
        public void ShouldPreserveOrder()
        {
            var apples = Create("apples", 1);
            var oranges = Create("oranges", 2);

            var one = Act(apples, oranges);
            var two = Act(oranges, apples);

            Assert.Equal(2, one.Keys.Count);
            Assert.Equal(2, two.Keys.Count);
            Assert.Equal(2, one.Values.Count);
            Assert.Equal(2, two.Values.Count);
            Assert.True(one.First().Equals(apples));
            Assert.True(one.Last().Equals(oranges));
            Assert.True(two.First().Equals(oranges));
            Assert.True(two.Last().Equals(apples));
        }
    }
}

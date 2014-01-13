using System;
using System.Linq;
using Automata.Core;
using NUnit.Framework;

namespace Automata.Tests
{
    [TestFixture]
    public class ContextTest
    {
        [Test]
        public void TestIsAmericaSummerTime()
        {
            var t1 = new DateTime(2014, 01, 01);
            Assert.False(t1.IsAmericaSummerTime());
            var t2 = new DateTime(2014, 03, 09, 0, 0, 0);
            Assert.False(t2.IsAmericaSummerTime());

            // the day, 2:00am EST becomes 3:00am EDT immediately
            var t3 = new DateTime(2014, 03, 09, 4, 0, 0);
            Assert.True(t3.IsAmericaSummerTime());
            var t4 = new DateTime(2014, 11, 02, 4, 0, 0);
            Assert.False(t4.IsAmericaSummerTime());
        }

        [Test]
        public void TestIsUnitedKingdomSummerTime()
        {
        }
    }
}

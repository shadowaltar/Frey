using System;
using System.Globalization;
using System.Linq;
using Automata.Core;
using Automata.Core.Extensions;
using Automata.Entities;
using Moq;
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
        public void TestEasternTimeToUTC()
        {
            // EST
            var time = "2013-11-22 14:00:10";
            var dt = DateTime.ParseExact(time, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            var newDt = dt.AmericaToUTC0();
            Assert.True(newDt.Hour == 19);

            // EDT
            time = "2013-5-1 14:00:10";
            dt = DateTime.ParseExact(time, "yyyy-M-d HH:mm:ss", CultureInfo.InvariantCulture);
            newDt = dt.AmericaToUTC0();
            Assert.True(newDt.Hour == 18);
        }

        [Test]
        public void TestIsForexMarketTradingSession()
        {
            var time = "2014-01-17 16:59:59"; // NY session end
            var dt = time.ToDateTime("yyyy-M-d HH:mm:ss").AmericaToUTC0();
            Assert.True(dt.IsInForexMarketTradingSession());
        }

        [Test]
        public void TestFindTradingSessionRange()
        {
            var time = "2014-01-17 16:59:59"; // NY session end
            var dt = time.ToDateTime("yyyy-M-d HH:mm:ss").AmericaToUTC0();
            var mock = new Mock<Forex>();
            StaticFileDataAccess.GetTradingSessionTimeRange(dt, mock.Object);
            Assert.True(dt.IsInForexMarketTradingSession());
        }
    }
}

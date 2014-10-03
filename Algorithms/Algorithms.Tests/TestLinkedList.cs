using System;
using Algorithms.Collections;
using NUnit.Framework;

namespace Algorithms.Tests
{
    [TestFixture]
    public class TestLinkedList
    {
        [Test]
        public void TestAdd()
        {
            var list = new LinkedList<string> { "A", "B", "C", "D" };
            Assert.AreEqual("A", list[0]);
            Assert.AreEqual("B", list[1]);
            Assert.AreEqual("C", list[2]);
            Assert.AreEqual("D", list[3]);
        }

        [Test]
        public void TestRemove()
        {
            var list = new LinkedList<string> { "A", "B", "C", "D", "E", "F", "G", "H" };
            list.Remove("C");
            Assert.AreEqual("A", list[0]);
            Assert.AreEqual("B", list[1]);
            Assert.AreEqual("D", list[2]);
            Assert.AreEqual("F", list[4]);
            list.Remove("G");
            Assert.AreEqual("H", list[5]);
            list.Remove("H");
            Assert.AreEqual("F", list[4]);
            list.Remove("A");
            Assert.AreEqual("B", list[0]);
        }

        [Test]
        public void TestRemoveAt()
        {
            var list = new LinkedList<string> { "A", "B", "C", "D", "E", "F", "G", "H" };
            list.RemoveAt(2);
            Assert.AreEqual("A", list[0]);
            Assert.AreEqual("B", list[1]);
            Assert.AreEqual("D", list[2]);
            Assert.AreEqual("F", list[4]);
            list.RemoveAt(5);
            Assert.AreEqual("H", list[5]);
            list.RemoveAt(5);
            Assert.AreEqual("F", list[4]);
            list.RemoveAt(0);
            Assert.AreEqual("B", list[0]);
        }

        [Test]
        public void TestClear()
        {
            var list = new LinkedList<string> { "A", "B", "C", "D", "E", "F", "G", "H" };
            list.Clear();
            Assert.AreEqual(list.Count, 0);
            Assert.Throws(typeof(IndexOutOfRangeException), () => { var a = list[0]; });
        }
    }
}

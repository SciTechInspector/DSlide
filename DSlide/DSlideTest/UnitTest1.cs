using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using DSlide;

namespace DSlideTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void SimplePropertyTests()
        {
            var obj = new DiamondTest();
            Assert.IsTrue(obj.FirstName == null);
            Assert.IsTrue(obj.LastName == null);

            obj.FirstName = "Bob";
            obj.LastName = "Morane";

            Assert.IsTrue(obj.FirstName == "Bob");
            Assert.IsTrue(obj.LastName == "Morane");

            Assert.IsTrue(obj.FullName == "Bob Morane");
        }

        [TestMethod]
        public void TestFindNearestLessThanInSortedList()
        {
            var sorted = new SortedList<int, int>();
            sorted.Add(2, 2);
            sorted.Add(5, 5);
            sorted.Add(8, 8);
            sorted.Add(14, 14);
            sorted.Add(82, 82);
            sorted.Add(192, 192);

            Assert.IsTrue(sorted.FindIndexOfNearestLessOrEqualToKey(1) == -1);
            Assert.IsTrue(sorted.FindIndexOfNearestLessOrEqualToKey(2) == 0);
            Assert.IsTrue(sorted.FindIndexOfNearestLessOrEqualToKey(3) == 0);
            Assert.IsTrue(sorted.FindIndexOfNearestLessOrEqualToKey(5) == 1);
            Assert.IsTrue(sorted.FindIndexOfNearestLessOrEqualToKey(6) == 1);
            Assert.IsTrue(sorted.FindIndexOfNearestLessOrEqualToKey(7) == 1);
            Assert.IsTrue(sorted.FindIndexOfNearestLessOrEqualToKey(8) == 2);
            Assert.IsTrue(sorted.FindIndexOfNearestLessOrEqualToKey(11) == 2);
            Assert.IsTrue(sorted.FindIndexOfNearestLessOrEqualToKey(14) == 3);
            Assert.IsTrue(sorted.FindIndexOfNearestLessOrEqualToKey(15) == 3);
            Assert.IsTrue(sorted.FindIndexOfNearestLessOrEqualToKey(82) == 4);
            Assert.IsTrue(sorted.FindIndexOfNearestLessOrEqualToKey(83) == 4);
            Assert.IsTrue(sorted.FindIndexOfNearestLessOrEqualToKey(191) == 4);
            Assert.IsTrue(sorted.FindIndexOfNearestLessOrEqualToKey(192) == 5);
            Assert.IsTrue(sorted.FindIndexOfNearestLessOrEqualToKey(193) == 5);
        }
    }
}

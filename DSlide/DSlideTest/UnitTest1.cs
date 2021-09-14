using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DSlideTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void SimpleGetAndSet()
        {
            var obj = new DiamondTest();
            Assert.IsTrue(obj.FirstName == null);
            Assert.IsTrue(obj.LastName == null);

            obj.FirstName = "Bob";
            obj.LastName = "Morane";

            Assert.IsTrue(obj.FirstName == "Bob");
            Assert.IsTrue(obj.LastName == "Morane");
        }
    }
}

using SyncMeAppLibrary;

namespace UT.SyncMeApp.TestProjects
{
    [TestClass]
    public class UtilsTests
    {
        [DataRow(5, "ms")]
        [DataRow(5000, "s")]
        [DataRow(300000, "min")]
        [DataRow(18000000, "h")]
        [DataRow(432000000, "d")]
        [TestMethod]
        public void TimerUnitTest(double expectedValue, string unit)
        {
            Assert.AreEqual(Utils.ConvertToMiliseconds(5, unit), expectedValue);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void UnknownTimerUnitTest()
        {
            var testInt = Utils.ConvertToMiliseconds(1,"kg");
        }
    }
}

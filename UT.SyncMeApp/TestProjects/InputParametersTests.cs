using SyncMeAppLibrary;

namespace UT.SyncMeApp.TestProjects
{
    [TestClass]
    public class InputParametersTests
    {
        [DataRow("testdirectory", "'c:\\somedirectory\\anotherdirectory'")]
        [DataRow("TestDirectory", "'c:\\somedirectory\\anotherdirectory'")] // tests case insensitivity of input parameter
        [TestMethod]
        public void GetSubstringTest(string parameterName, string expectedValue)
        {
            string[] mockArgs =  {$"{parameterName}={expectedValue}", "someparameter=someparameter"};
            
            string outputvalue = Utils.GetSubstring(mockArgs, parameterName);

            Assert.AreEqual(expectedValue, outputvalue);

        }
    }
}
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.IO;
using System.Net;

namespace Salesforce.WinSDK.Net
{
    [TestClass]
    public class HttpCallTest
    {
        [TestMethod]
        public void testExecuteGet() {
            HttpCall c = HttpCall.createGet("http://www.google.com");
            String response = c.execute().Result.ResponseBody;
            Assert.IsTrue(response.Contains("Google Search"), "Wrong response");
        }
    }
}

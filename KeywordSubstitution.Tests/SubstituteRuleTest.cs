//
// NOTE: If there are rules that are not being tested here, it's because I don't know how to provide the necessary VS services used by those rules.
//

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KeywordSubstitution.SubstituteRules;
using KeywordSubstitution.SubstituteRules.Detail;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.IO;

namespace KeywordSubstitution.Tests
{
    [TestClass]
    public class SubstituteRuleTest
    {
        SubstituteRuleManager _manager = new SubstituteRuleManager();

        public SubstituteRuleTest()
        {
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
        }

        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        public class RuleTestInfo
        {
            public string Input;
            public string ExpectedOutput;

            public RuleTestInfo(string input, string expectedOutput)
            {
                Input = input;
                ExpectedOutput = expectedOutput;
            }
        }

        [TestMethod]
        public void TestIncrementNumber()
        {
            var dataProvider = new SubstituteRuleTestDataProvider();
            testRule(new RuleTestInfo("$IncrementNumber: -1$", "$IncrementNumber: 0$"), dataProvider);
            testRule(new RuleTestInfo("$IncrementNumber: $", "$IncrementNumber: 1$"), dataProvider);
            testRule(new RuleTestInfo("$IncrementNumber: 1$", "$IncrementNumber: 2$"), dataProvider);
        }

        [TestMethod]
        public void TestDateTimeNumber()
        {
            var dataProvider = new SubstituteRuleTestDataProvider();
            var cultureIDs = new string[] { "en-GB", "en-US", "nb-NO", "ja-JP" };
            var cultureBackup = System.Threading.Thread.CurrentThread.CurrentCulture;
            foreach (string cultureID in cultureIDs)
            {
                // Switch culture to make sure the date-time-format is the same regardless of culture
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cultureID);

                // Note: This must be run very quickly for the test to succeed, so that the times match
                string dateTimeStr = DateTimeOffset.Now.ToString(DateTimeRule.DateTimeFormat);
                testRule(new RuleTestInfo("$DateTime: $", string.Format("$DateTime: {0}$", dateTimeStr)), dataProvider);

                Assert.IsTrue(Regex.IsMatch(dateTimeStr, @"\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}\+\d{2}:\d{2}"));
            }
        }

        [TestMethod]
        public void TestUserName()
        {
            var dataProvider = new SubstituteRuleTestDataProvider();
            string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            testRule(new RuleTestInfo("$UserName: $", string.Format("$UserName: {0}$", userName)), dataProvider);
        }

        [TestMethod]
        public void TestMachineName()
        {
            var dataProvider = new SubstituteRuleTestDataProvider();
            testRule(new RuleTestInfo("$MachineName: $", string.Format("$MachineName: {0}$", Environment.MachineName)), dataProvider);
        }

        [TestMethod]
        public void TestFilePath()
        {
            var dataProvider = new SubstituteRuleTestDataProvider();
            string filePath = @"C:/my/test/path";
            dataProvider.DocumentInfo.pbstrMkDocument = filePath;
            testRule(new RuleTestInfo("$FilePath: $", string.Format("$FilePath: {0}$", filePath)), dataProvider);
        }

        [TestMethod]
        public void TestFileName()
        {
            var dataProvider = new SubstituteRuleTestDataProvider();
            string fileName = "myname";
            string filePath = Path.Combine(@"C:/my/test/path", fileName);
            dataProvider.DocumentInfo.pbstrMkDocument = filePath;
            testRule(new RuleTestInfo("$FileName: $", string.Format("$FileName: {0}$", fileName)), dataProvider);
        }

        private void testRule(RuleTestInfo rti, SubstituteRuleTestDataProvider dataProvider)
        {
            string output = null;
            _manager.ExecuteRules(out output, rti.Input, dataProvider);
            Assert.AreEqual(rti.ExpectedOutput, output);
        }
    }
}

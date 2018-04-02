using localization.Localization;
using localization.tests.TestClasses;
using localization.tests.UnitTests.Localization;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LocalizationTests.tests.UnitTests.Localization
{
    [TestClass]
    public class LocalizedRouteConventionTest
    {
        [TestInitialize]
        public void TestInitialize()
        {
            LocalizationDataHandlerTest.InitDataHandler();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            LocalizationDataHandlerTest.ResetDataHandler();
        }

        [TestMethod]
        public void ApplyTest()
        {
            LocalizedRouteConvention routeConvention = new LocalizedRouteConvention();

            ApplicationModel applicationModel = new ApplicationModel()
            {
                
            };

            routeConvention.Apply(applicationModel);

            Assert.Fail();
        }

        [TestMethod]
        public void GetParameterNameTest()
        {
            LocalizedRouteConvention routeConvention = new LocalizedRouteConvention();

            List<TestInputExpected> inputsAndExpectations = new List<TestInputExpected>() {
                new TestInputExpected("{action}", "action"),
                new TestInputExpected("{ CoNtRoLLeR }", "controller"),
                new TestInputExpected("{ moo:id = 1337}", "moo"),
                new TestInputExpected("{howl=1337}", "howl"),
                new TestInputExpected("{filename:length(8,16)}", "filename"),
                new TestInputExpected("{ssn:regex(^\\d{{3}}-\\d{{2}}-\\d{{4}}$)}", "ssn"),
                new TestInputExpected("{0}", "0"),
                // I'm not sure what to do with this one yet.
                new TestInputExpected("{*slug}", "*slug")
            };

            foreach (TestInputExpected test in inputsAndExpectations)
            {
                string input = test.Input as string;
                string expected = test.Expected as string;
                string name = routeConvention.GetParameterName(input);

                Assert.AreEqual(expected, name);
            }
        }
    }
}

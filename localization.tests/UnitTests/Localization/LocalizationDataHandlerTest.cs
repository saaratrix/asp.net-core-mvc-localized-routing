using localization.Localization;
using localization.tests.TestClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace localization.tests.UnitTests.Localization
{
    [TestClass]
    public class LocalizationDataHandlerTest
    {
        public static void InitDataHandler()
        {
            LocalizationDataHandler.DefaultCulture = "en";
            LocalizationDataHandler.SupportedCultures = new List<string>()
            {
                "en",
                "fi",
                "sv"
            };
        }

        /// <summary>
        /// Reset the ControllerRoutes property so there are no lingering action routes.
        /// </summary>
        public static void ResetDataHandler()
        {
            LocalizationDataHandler.DefaultCulture = "en";
            LocalizationDataHandler.DefaultController = "Home";
            LocalizationDataHandler.DefaultAction = "Index";
            LocalizationDataHandler.SupportedCultures = new List<string>();

            var type = typeof(LocalizationDataHandler);
            var propInfo = type.GetField("_controllerRoutes", BindingFlags.Static | BindingFlags.NonPublic);            
            if (propInfo != null)
            {
                propInfo.SetValue(null, new ConcurrentDictionary<string, CultureControllerData>());
            }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            InitDataHandler();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            ResetDataHandler();
        }

        // Tests the Url part of the response
        [TestMethod]
        public void GetUrlHrefTest()
        {
            string defaultCulture = LocalizationDataHandler.DefaultCulture;
            // Controllers
            LocalizationDataHandler.AddControllerData("Home", defaultCulture, "/Home");
            LocalizationDataHandler.AddControllerData("Home", "fi", "/Koti");
            LocalizationDataHandler.AddControllerData("Home", "sv", "/Hem");

            LocalizationDataHandler.AddControllerData("Test", defaultCulture, "/Test");
            LocalizationDataHandler.AddControllerData("Test", "fi", "/Testi");
            LocalizationDataHandler.AddControllerData("Test", "sv", "/Test");

            // Actions
            LocalizationDataHandler.AddActionData("Home", "Index", defaultCulture, "/", "Index", null);
            LocalizationDataHandler.AddActionData("Home", "Index", "fi", "/", "Index", null);
            LocalizationDataHandler.AddActionData("Home", "Index", "sv", "/", "Index", null);

            LocalizationDataHandler.AddActionData("Test", "TestAction", defaultCulture, "/TestAction", "Test Action", null);
            LocalizationDataHandler.AddActionData("Test", "TestAction", "fi", "/TestiToiminta", "Testi Toiminta", null);
            LocalizationDataHandler.AddActionData("Test", "TestAction", "sv", "/TestHandling", "Testi Toiminta", null);

            List<TestInputExpected> inputsAndExpectations = new List<TestInputExpected>()
            {
                new TestInputExpected(new TestActionControllerData("tESt", "teSTAction"), "test/testaction"),
                new TestInputExpected(new TestActionControllerData("test", "testAction", null, "fi"), "/fi/testi/testitoiminta")
            };

            foreach (var test in inputsAndExpectations)
            {
                var input = test.Input as TestActionControllerData;
                string expected = test.Expected as string;
                var result = LocalizationDataHandler.GetUrl(input.Controller, input.Controller, defaultCulture);                
            }
            
            Assert.Fail();
        }

        // Tests the link
        [TestMethod]
        public void GetUrlLinkTest()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void GetOrderedParametersTest()
        {
            string defaultCulture = LocalizationDataHandler.DefaultCulture;

            LocalizationDataHandler.AddControllerData("Test", defaultCulture, "/Test");
            LocalizationDataHandler.AddActionData("Test", "TestAction", defaultCulture, "/TestAction", "Test Action", new List<string>() { "param" });
            LocalizationDataHandler.AddActionData("Test", "TestAction2", defaultCulture, "/TestAction2", "Test Action2", new List<string>() { "param", "param2" });

            List<TestInputExpected> inputsAndExpectations = new List<TestInputExpected>()
            {
                // Test with no routevalues so will not find any parameter routes
                new TestInputExpected(new TestActionControllerData() {
                    Action = "TestAction",
                    Controller = "Test",
                    RouteValues = new Dictionary<string, string>()
                }, ""),
                new TestInputExpected(new TestActionControllerData("TeST", "TestACTIon", new Dictionary<string, string>()
                    {
                        { "param", "1337" }
                    }), "/1337"),
                new TestInputExpected(new TestActionControllerData("Test", "TestAction2", new Dictionary<string, string>()
                    {
                        { "param", "1" },
                        { "param2", "2" }
                    }), "/1/2"),
                new TestInputExpected(new TestActionControllerData("Test", "TestAction2", new Dictionary<string, string>()
                    {
                        { "param", "1" }                       
                    }), "/1"),
                // Fails
                new TestInputExpected(new TestActionControllerData("Fail", "TestAction"), ""),
                new TestInputExpected(new TestActionControllerData("Test", "FailAction"), ""),
                // Testing incorrect parameters from route data
                new TestInputExpected(new TestActionControllerData("Test", "TestAction2", new Dictionary<string, string>()
                    {
                        { "fail1", "1" },
                        { "param2", "2" }
                    }), "")
            };

            foreach (TestInputExpected test in inputsAndExpectations)
            {
                TestActionControllerData input = test.Input as TestActionControllerData;
                string expected = test.Expected as string;
                string culture = LocalizationDataHandler.GetOrderedParameters(input.Controller, input.Action, input.RouteValues);
                Assert.AreEqual(expected, culture);
            }           
        }

        [TestMethod]
        public void GetCultureFromUrlTest()
        {
            string defaultCulture = LocalizationDataHandler.DefaultCulture;

            List<TestInputExpected> inputsAndExpectations = new List<TestInputExpected>()
            {
                new TestInputExpected("/moo", defaultCulture),
                new TestInputExpected("/sv/moo", "sv"),
                new TestInputExpected("/fi/", "fi"),
                // Failing tests
                new TestInputExpected("/fi", defaultCulture),
                new TestInputExpected("/sv_Sv", defaultCulture),
                new TestInputExpected("", defaultCulture),
                new TestInputExpected("/", defaultCulture)
            };

            foreach (TestInputExpected test in inputsAndExpectations)
            {
                string input = test.Input as string;
                string expected = test.Expected as string;
                string culture = LocalizationDataHandler.GetCultureFromUrl(input);
                Assert.AreEqual(expected, culture);
            }
        }       
    }
}

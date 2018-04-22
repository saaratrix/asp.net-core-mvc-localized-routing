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
            LocalizationDataHandler.SupportedCultures = new HashSet<string>()
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
            LocalizationDataHandler.SupportedCultures = new HashSet<string>();

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
            LocalizationDataHandler.AddControllerData("Home", defaultCulture, "Home");
            LocalizationDataHandler.AddControllerData("Home", "fi", "fi/Koti");
            LocalizationDataHandler.AddControllerData("Home", "sv", "sv/hem");

            LocalizationDataHandler.AddControllerData("Test", defaultCulture, "Test");
            LocalizationDataHandler.AddControllerData("Test", "fi", "fi/Testi");
            LocalizationDataHandler.AddControllerData("Test", "sv", "sv/Test");

            // Actions
            LocalizationDataHandler.AddActionData("Home", "Index", defaultCulture, "", "Index", new List<string>());
            //LocalizationDataHandler.AddActionData("Home", "Index", "fi", "/", "Index", new List<string>());
            LocalizationDataHandler.AddActionData("Home", "Index", "sv", "", "Index", new List<string>());

            LocalizationDataHandler.AddActionData("Home", "About", defaultCulture, "about", "About", new List<string>());
            LocalizationDataHandler.AddActionData("Home", "About", "fi", "miestä", "Miestä", new List<string>());
            LocalizationDataHandler.AddActionData("Home", "About", "sv", "Om", "Om", new List<string>());

            LocalizationDataHandler.AddActionData("Test", "Index", defaultCulture, "", "", new List<string>());
            LocalizationDataHandler.AddActionData("Test", "Index", "fi", "", "Testi", new List<string>());
            LocalizationDataHandler.AddActionData("Test", "Index", "sv", "", "Test", new List<string>());

            LocalizationDataHandler.AddActionData("Test", "TestAction", defaultCulture, "testaction", "Test Action", new List<string>());
            LocalizationDataHandler.AddActionData("Test", "TestAction", "fi", "TestiToiminta", "Testi Toiminta", new List<string>());
            LocalizationDataHandler.AddActionData("Test", "TestAction", "sv", "TestHandling", "Testi Handling", new List<string>());

            List<TestInputExpected> inputsAndExpectations = new List<TestInputExpected>()
            {
                // Default actions
                new TestInputExpected(new TestActionControllerData("home", "index", null, defaultCulture), new LocalizedUrlResult() { Url = "/", LinkName = "" }),
                new TestInputExpected(new TestActionControllerData("hOme", "index", null, "fi"), new LocalizedUrlResult() { Url = "/fi", LinkName = "Index" }),
                new TestInputExpected(new TestActionControllerData("hoMe", "INdex", null, "sv"), new LocalizedUrlResult() { Url = "/sv", LinkName = "Index" }),
                // About actions
                new TestInputExpected(new TestActionControllerData("home", "about", null, defaultCulture), new LocalizedUrlResult() { Url = "/Home/about", LinkName = "" }),
                new TestInputExpected(new TestActionControllerData("hOme", "about", null, "fi"), new LocalizedUrlResult() { Url = "/fi/Koti/miestä", LinkName = "Miestä" }),
                new TestInputExpected(new TestActionControllerData("hoMe", "about", null, "sv"), new LocalizedUrlResult() { Url = "/sv/hem/Om", LinkName = "Om" }),

                //// Test actions
                //new TestInputExpected(new TestActionControllerData("test", "index"), new LocalizedUrlResult() { Url = "/Test", LinkName = "" }),
                new TestInputExpected(new TestActionControllerData("test", "inDex", null, "fi"), new LocalizedUrlResult() { Url = "/fi/Testi", LinkName = "Testi" }),
                new TestInputExpected(new TestActionControllerData("test", "index", null, "sv"), new LocalizedUrlResult() { Url = "/sv/Test", LinkName = "Test" }),

                // Test actions
                new TestInputExpected(new TestActionControllerData("tESt", "teSTAction"), new LocalizedUrlResult() { Url = "/Test/testaction", LinkName = "" }),
                new TestInputExpected(new TestActionControllerData("test", "testAction", null, "fi"), new LocalizedUrlResult() { Url = "/fi/Testi/TestiToiminta", LinkName = "Testi Toiminta" }),
                new TestInputExpected(new TestActionControllerData("teST", "testActIOn", null, "sv"), new LocalizedUrlResult() { Url = "/sv/Test/TestHandling", LinkName = "Testi Handling" }),
                
                // Fails
            };

            List<TestActionControllerData> throwingTests = new List<TestActionControllerData>()
            {
                // Invalid controller
                new TestActionControllerData("homez", "index", null, defaultCulture),
                // Invalid action
                new TestActionControllerData("home", "notindex", null, defaultCulture)                
            };

            foreach (var test in inputsAndExpectations)
            {
                var input = test.Input as TestActionControllerData;
                LocalizedUrlResult expected = (LocalizedUrlResult)test.Expected;
                var result = LocalizationDataHandler.GetUrl(input.Controller, input.Action, input.Culture);

                Assert.AreEqual(expected.Url, result.Url);
                Assert.AreEqual(expected.LinkName, result.LinkName, String.Format("For {0}/{1} {2}", input.Controller, input.Action, input.Culture));
            }    
            
            foreach (var test in throwingTests)
            {
                Assert.ThrowsException<ArgumentException>(() => LocalizationDataHandler.GetUrl(test.Controller, test.Action, test.Culture));                
            }
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

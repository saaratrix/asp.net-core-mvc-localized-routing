using localization.Localization;
using localization.Localization.CultureRouteData;
using localization.tests.TestClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace localization.tests.UnitTests.Localization
{
    [TestClass]
    public class LocalizationRouteDataHandlerTest
    {
        public static void InitDataHandler()
        {
            LocalizationRouteDataHandler.DefaultCulture = "en";
            LocalizationRouteDataHandler.SupportedCultures = new Dictionary<string, string>()
            {
                { "en", "English" },
                { "fi", "Suomeksi" },
                { "sv", "Svenska" }
            };
        }

        /// <summary>
        /// Reset the ControllerRoutes property so there are no lingering action routes.
        /// </summary>
        public static void ResetDataHandler()
        {
            LocalizationRouteDataHandler.DefaultCulture = "en";
            LocalizationRouteDataHandler.DefaultController = "Home";
            LocalizationRouteDataHandler.DefaultAction = "Index";
            LocalizationRouteDataHandler.SupportedCultures = new Dictionary<string, string>();

            LocalizationRouteDataHandler.ControllerRoutes.Clear();            
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
        public void GetUrlTest()
        {
            string defaultCulture = LocalizationRouteDataHandler.DefaultCulture;
            // Controllers
            LocalizationRouteDataHandler.AddControllerRouteData("Home", defaultCulture, "Home");
            LocalizationRouteDataHandler.AddControllerRouteData("Home", "fi", "fi/Koti");
            LocalizationRouteDataHandler.AddControllerRouteData("Home", "sv", "sv/hem");

            LocalizationRouteDataHandler.AddControllerRouteData("Test", defaultCulture, "Test");
            LocalizationRouteDataHandler.AddControllerRouteData("Test", "fi", "fi/Testi");
            LocalizationRouteDataHandler.AddControllerRouteData("Test", "sv", "sv/Test");

            // Actions
            LocalizationRouteDataHandler.AddActionRouteData("Home", "Index", defaultCulture, "", "Index", new List<string>());
            
            LocalizationRouteDataHandler.AddActionRouteData("Home", "Index", "sv", "", "Index", new List<string>());

            LocalizationRouteDataHandler.AddActionRouteData("Home", "About", defaultCulture, "about", "About", new List<string>());
            LocalizationRouteDataHandler.AddActionRouteData("Home", "About", "fi", "meistä", "Meistä", new List<string>());
            LocalizationRouteDataHandler.AddActionRouteData("Home", "About", "sv", "Om", "Om", new List<string>());

            LocalizationRouteDataHandler.AddActionRouteData("Test", "Index", defaultCulture, "", "", new List<string>());
            LocalizationRouteDataHandler.AddActionRouteData("Test", "Index", "fi", "", "Testi", new List<string>());
            LocalizationRouteDataHandler.AddActionRouteData("Test", "Index", "sv", "", "Test", new List<string>());

            LocalizationRouteDataHandler.AddActionRouteData("Test", "TestAction", defaultCulture, "testaction", "Test Action", new List<string>());
            LocalizationRouteDataHandler.AddActionRouteData("Test", "TestAction", "fi", "TestiToiminta", "Testi Toiminta", new List<string>());
            LocalizationRouteDataHandler.AddActionRouteData("Test", "TestAction", "sv", "TestHandling", "Testi Handling", new List<string>());

            List<TestInputExpected> inputsAndExpectations = new List<TestInputExpected>()
            {
                // Default actions
                new TestInputExpected(new TestActionControllerData("home", "index", null, defaultCulture), new LocalizationUrlResult() { Url = "/", LinkName = "" }),
                new TestInputExpected(new TestActionControllerData("hOme", "index", null, "fi"), new LocalizationUrlResult() { Url = "/fi", LinkName = "Index" }),
                new TestInputExpected(new TestActionControllerData("hoMe", "INdex", null, "sv"), new LocalizationUrlResult() { Url = "/sv", LinkName = "Index" }),
                // About actions
                new TestInputExpected(new TestActionControllerData("home", "about", null, defaultCulture), new LocalizationUrlResult() { Url = "/Home/about", LinkName = "" }),
                new TestInputExpected(new TestActionControllerData("hOme", "about", null, "fi"), new LocalizationUrlResult() { Url = "/fi/Koti/meistä", LinkName = "Meistä" }),
                new TestInputExpected(new TestActionControllerData("hoMe", "about", null, "sv"), new LocalizationUrlResult() { Url = "/sv/hem/Om", LinkName = "Om" }),

                //// Test actions
                //new TestInputExpected(new TestActionControllerData("test", "index"), new LocalizedUrlResult() { Url = "/Test", LinkName = "" }),
                new TestInputExpected(new TestActionControllerData("test", "inDex", null, "fi"), new LocalizationUrlResult() { Url = "/fi/Testi", LinkName = "Testi" }),
                new TestInputExpected(new TestActionControllerData("test", "index", null, "sv"), new LocalizationUrlResult() { Url = "/sv/Test", LinkName = "Test" }),

                // Test actions
                new TestInputExpected(new TestActionControllerData("tESt", "teSTAction"), new LocalizationUrlResult() { Url = "/Test/testaction", LinkName = "" }),
                new TestInputExpected(new TestActionControllerData("test", "testAction", null, "fi"), new LocalizationUrlResult() { Url = "/fi/Testi/TestiToiminta", LinkName = "Testi Toiminta" }),
                new TestInputExpected(new TestActionControllerData("teST", "testActIOn", null, "sv"), new LocalizationUrlResult() { Url = "/sv/Test/TestHandling", LinkName = "Testi Handling" }),
                
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
                LocalizationUrlResult expected = (LocalizationUrlResult)test.Expected;
                var result = LocalizationRouteDataHandler.GetUrl(input.Controller, input.Action, input.Culture);

                Assert.AreEqual(expected.Url, result.Url);
                Assert.AreEqual(expected.LinkName, result.LinkName, String.Format("For {0}/{1} {2}", input.Controller, input.Action, input.Culture));
            }    
            
            foreach (var test in throwingTests)
            {
                Assert.ThrowsException<ArgumentException>(() => LocalizationRouteDataHandler.GetUrl(test.Controller, test.Action, test.Culture));                
            }
        }                

        [TestMethod]
        public void GetOrderedParametersTest()
        {
            string defaultCulture = LocalizationRouteDataHandler.DefaultCulture;

            LocalizationRouteDataHandler.AddControllerRouteData("Test", defaultCulture, "/Test");
            LocalizationRouteDataHandler.AddActionRouteData("Test", "TestAction", defaultCulture, "/TestAction", "Test Action", new List<string>() { "param" });
            LocalizationRouteDataHandler.AddActionRouteData("Test", "TestAction2", defaultCulture, "/TestAction2", "Test Action2", new List<string>() { "param", "param2" });

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
                string culture = LocalizationRouteDataHandler.GetOrderedParameters(input.Controller, input.Action, input.RouteValues);
                Assert.AreEqual(expected, culture);
            }           
        }

        [TestMethod]
        public void GetCultureFromUrlTest()
        {
            string defaultCulture = LocalizationRouteDataHandler.DefaultCulture;

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
                string culture = LocalizationRouteDataHandler.GetCultureFromUrl(input);
                Assert.AreEqual(expected, culture);
            }
        }       
    }
}

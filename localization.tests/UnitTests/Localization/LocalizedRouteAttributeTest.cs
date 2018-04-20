using localization.Localization;
using localization.tests.TestClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace localization.tests.UnitTests.Localization
{
    [TestClass]
    public class LocalizedRouteAttributeTest
    {
        // TODO: Constructor tests? 
        [TestMethod]
        public void LocalizedRouteAttributeConstructorTest()
        {
            List<TestInputExpected> singleParameter = new List<TestInputExpected>()
            {
                new TestInputExpected(new LocalizedRouteData("en", "", ""), new LocalizedRouteData("en", "", "")),
                new TestInputExpected(new LocalizedRouteData("fi", "", ""), new LocalizedRouteData("fi", "", "")),
                new TestInputExpected(new LocalizedRouteData("sv", "", ""), new LocalizedRouteData("sv", "", ""))
            };

            List<TestInputExpected> twoParameters = new List<TestInputExpected>()
            {
                new TestInputExpected(new LocalizedRouteData("en", "some route", ""), new LocalizedRouteData("en", "some_route", "Some Route")),
                new TestInputExpected(new LocalizedRouteData("fi", "Testi_testi", ""), new LocalizedRouteData("fi", "Testi_testi", "Testi Testi")),
                new TestInputExpected(new LocalizedRouteData("sv", "en-test-route", ""), new LocalizedRouteData("sv", "en-test-route", "En-Test-Route"))
            };

            List<TestInputExpected> allParameters = new List<TestInputExpected>()
            {
                new TestInputExpected(new LocalizedRouteData("en", "some route", "some batman route"), new LocalizedRouteData("en", "some_route", "some batman route")),                
                new TestInputExpected(new LocalizedRouteData("sv", "en-test-route", "en test route"), new LocalizedRouteData("sv", "en-test-route", "en test route")),
                new TestInputExpected(new LocalizedRouteData("fi", "Testi_testi", ""), new LocalizedRouteData("fi", "Testi_testi", "Testi Testi")),
                new TestInputExpected(new LocalizedRouteData("fi", "Testi_testi", null), new LocalizedRouteData("fi", "Testi_testi", "Testi Testi")),
            };

            foreach (var test in singleParameter)
            {
                var input = test.Input as LocalizedRouteData;
                var expected = test.Expected as LocalizedRouteData;

                var attribute = new LocalizedRouteAttribute(input.Culture);

                Assert.AreEqual(expected.Culture, attribute.Culture, "Single paramater");
                Assert.AreEqual(expected.Route, attribute.Route, "Single paramater");
                Assert.AreEqual(expected.Link, attribute.Link, "Single paramater");
            }

            foreach (var test in twoParameters)
            {
                var input = test.Input as LocalizedRouteData;
                var expected = test.Expected as LocalizedRouteData;

                var attribute = new LocalizedRouteAttribute(input.Culture, input.Route);

                Assert.AreEqual(expected.Culture, attribute.Culture, "Two Parameters");
                Assert.AreEqual(expected.Route, attribute.Route, "Two Parameters");
                Assert.AreEqual(expected.Link, attribute.Link, "Two Parameters");
            }

            foreach (var test in allParameters)
            {
                var input = test.Input as LocalizedRouteData;
                var expected = test.Expected as LocalizedRouteData;

                var attribute = new LocalizedRouteAttribute(input.Culture, input.Route, input.Link);

                Assert.AreEqual(expected.Culture, attribute.Culture, "All parameters");
                Assert.AreEqual(expected.Route, attribute.Route, "All parameters");
                Assert.AreEqual(expected.Link, attribute.Link, "All parameters");
            }
        }

        [TestMethod]
        public void ConvertRouteToLinkTest()
        {
            List<TestInputExpected> inputsAndExpectations = new List<TestInputExpected>()
            {
                new TestInputExpected(new ConvertRouteToLinkInput("fi", "koti"), "Koti"),
                new TestInputExpected(new ConvertRouteToLinkInput("fi", "ota_yhteyttä"), "Ota Yhteyttä"),
                new TestInputExpected(new ConvertRouteToLinkInput("en", ""), "")
            };

            foreach (var test in inputsAndExpectations)
            {
                var input = test.Input as ConvertRouteToLinkInput;
                string expected = test.Expected as string;

                string result = LocalizedRouteAttribute.ConvertRouteToLink(input.Culture, input.Route);

                Assert.AreEqual(expected, result);
            }
        }

        private class LocalizedRouteData
        {
            public string Culture { get; set; }
            public string Route { get; set; }
            public string Link { get; set; }

            public LocalizedRouteData(string culture, string route, string link)
            {
                Culture = culture;
                Route = route;
                Link = link;
            }
        }

        private class ConvertRouteToLinkInput
        {
            public string Culture { get; set; }
            public string Route { get; set; }            

            public ConvertRouteToLinkInput(string culture, string route)
            {
                Culture = culture;
                Route = route;                
            }
        }
    }
}

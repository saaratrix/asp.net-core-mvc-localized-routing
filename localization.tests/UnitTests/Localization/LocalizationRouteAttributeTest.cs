using localization.Localization;
using localization.tests.TestClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace localization.tests.UnitTests.Localization
{
    [TestClass]
    public class LocalizationRouteAttributeTest
    {
        // TODO: Constructor tests? 
        [TestMethod]
        public void LocalizationRouteAttributeConstructorTest()
        {
            List<TestInputExpected> singleParameter = new List<TestInputExpected>()
            {
                new TestInputExpected(new LocalizationRouteData("en", "", ""), new LocalizationRouteData("en", "", "")),
                new TestInputExpected(new LocalizationRouteData("fi", "", ""), new LocalizationRouteData("fi", "", "")),
                new TestInputExpected(new LocalizationRouteData("sv", "", ""), new LocalizationRouteData("sv", "", ""))
            };

            List<TestInputExpected> twoParameters = new List<TestInputExpected>()
            {
                new TestInputExpected(new LocalizationRouteData("en", "some route", ""), new LocalizationRouteData("en", "some_route", "Some Route")),
                new TestInputExpected(new LocalizationRouteData("fi", "Testi_testi", ""), new LocalizationRouteData("fi", "Testi_testi", "Testi Testi")),
                new TestInputExpected(new LocalizationRouteData("sv", "en-test-route", ""), new LocalizationRouteData("sv", "en-test-route", "En Test Route")),
                new TestInputExpected(new LocalizationRouteData("fi", "exampleFi", ""), new LocalizationRouteData("fi", "exampleFi", "ExampleFi"))
            };

            List<TestInputExpected> allParameters = new List<TestInputExpected>()
            {
                new TestInputExpected(new LocalizationRouteData("en", "some route", "some batman route"), new LocalizationRouteData("en", "some_route", "some batman route")),                
                new TestInputExpected(new LocalizationRouteData("sv", "en-test-route", "en test route"), new LocalizationRouteData("sv", "en-test-route", "en test route")),
                new TestInputExpected(new LocalizationRouteData("fi", "Testi_testi", ""), new LocalizationRouteData("fi", "Testi_testi", "Testi Testi")),
                new TestInputExpected(new LocalizationRouteData("fi", "Testi_testi", null), new LocalizationRouteData("fi", "Testi_testi", "Testi Testi")),
            };

            foreach (var test in singleParameter)
            {
                var input = test.Input as LocalizationRouteData;
                var expected = test.Expected as LocalizationRouteData;

                var attribute = new LocalizationRouteAttribute(input.Culture);

                Assert.AreEqual(expected.Culture, attribute.Culture, "Single paramater");
                Assert.AreEqual(expected.Route, attribute.Route, "Single paramater");
                Assert.AreEqual(expected.Link, attribute.Link, "Single paramater");
            }

            foreach (var test in twoParameters)
            {
                var input = test.Input as LocalizationRouteData;
                var expected = test.Expected as LocalizationRouteData;

                var attribute = new LocalizationRouteAttribute(input.Culture, input.Route);

                Assert.AreEqual(expected.Culture, attribute.Culture, "Two Parameters");
                Assert.AreEqual(expected.Route, attribute.Route, "Two Parameters");
                Assert.AreEqual(expected.Link, attribute.Link, "Two Parameters");
            }

            foreach (var test in allParameters)
            {
                var input = test.Input as LocalizationRouteData;
                var expected = test.Expected as LocalizationRouteData;

                var attribute = new LocalizationRouteAttribute(input.Culture, input.Route, input.Link);

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
                new TestInputExpected(new ConvertRouteToLinkInput("en", ""), ""),
                new TestInputExpected(new ConvertRouteToLinkInput("fi", "exampleFI"), "ExampleFI"),
                new TestInputExpected(new ConvertRouteToLinkInput("fi", "exampleFi_sv-tESt"), "ExampleFi Sv TESt"),
                new TestInputExpected(new ConvertRouteToLinkInput("fi", "a"), "A"),
                new TestInputExpected(new ConvertRouteToLinkInput("fi", "_a___b_"), "A B"),
            };

            foreach (var test in inputsAndExpectations)
            {
                var input = test.Input as ConvertRouteToLinkInput;
                string expected = test.Expected as string;

                string result = LocalizationRouteAttribute.ConvertRouteToLink(input.Culture, input.Route);

                Assert.AreEqual(expected, result);
            }
        }

        private class LocalizationRouteData
        {
            public string Culture { get; set; }
            public string Route { get; set; }
            public string Link { get; set; }

            public LocalizationRouteData(string culture, string route, string link)
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

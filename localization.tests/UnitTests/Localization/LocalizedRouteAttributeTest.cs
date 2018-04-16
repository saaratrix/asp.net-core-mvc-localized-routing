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
            Assert.Fail();
        }

        [TestMethod]
        public void ConvertRouteToLinkTest()
        {
            List<TestInputExpected> inputsAndExpectations = new List<TestInputExpected>()
            {
                new TestInputExpected(new ConvertRouteToLinkInput("koti", "fi"), "Koti"),
                new TestInputExpected(new ConvertRouteToLinkInput("ota_yhteyttä", "fi"), "Ota Yhteyttä"),
                new TestInputExpected(new ConvertRouteToLinkInput("", "en"), "")
            };

            foreach (var test in inputsAndExpectations)
            {
                var input = test.Input as ConvertRouteToLinkInput;
                string expected = test.Expected as string;

                string result = LocalizedRouteAttribute.ConvertRouteToLink(input.Route, input.Culture);

                Assert.AreEqual(expected, result);
            }
        }

        private class ConvertRouteToLinkInput
        {
            public string Route { get; set; }
            public string Culture { get; set; }

            public ConvertRouteToLinkInput(string route, string culture)
            {
                Route = route;
                Culture = culture;
            }
        }
    }
}

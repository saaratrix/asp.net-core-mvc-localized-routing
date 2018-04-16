using localization.Localization;
using localization.tests.TestClasses;
using localization.tests.UnitTests.Localization;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
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
        LocalizedRouteConvention _localizedRouteConvention;

        [TestInitialize]
        public void TestInitialize()
        {
            _localizedRouteConvention = new LocalizedRouteConvention();
        }

        [TestMethod]
        public void GetLocalizedControllerNameTest()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void ParseParameterTemplateTest()
        {
            List<TestInputExpected> inputsAndExpectations = new List<TestInputExpected>()
            {
                new TestInputExpected("", new ParseParameterTemplateExpected("", new List<string>())),
                new TestInputExpected("/{controller}/{action}", new ParseParameterTemplateExpected("/{controller}/{action}", new List<string>())),
                new TestInputExpected("/{controller}/{action}/{index}/{ moo=1337 }", new ParseParameterTemplateExpected("/{controller}/{action}/{index}/{ moo=1337 }", new List<string>() { "index", "moo" })),
                new TestInputExpected("/Error/{0}", new ParseParameterTemplateExpected("/{0}", new List<string>() { "0" })),
                new TestInputExpected("/Parameter/{index}/{test}", new ParseParameterTemplateExpected("/{index}/{test}", new List<string>() { "index", "test" }))
            };

            foreach (TestInputExpected test in inputsAndExpectations)
            {
                string inputTemplate = test.Input as string;
                List<string> sortedRouteParameters = new List<string>();
                var expected = test.Expected as ParseParameterTemplateExpected;
                string parameterTemplate = _localizedRouteConvention.ParseParameterTemplate(inputTemplate, sortedRouteParameters);

                Assert.AreEqual(expected.ParameterTemplate, parameterTemplate);
                CollectionAssert.AreEqual(expected.SortedRouteParameters, sortedRouteParameters);
            }            
        }

        [TestMethod]
        public void GetParameterNameTest()
        {
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
                string name = _localizedRouteConvention.GetParameterName(input);

                Assert.AreEqual(expected, name);
            }
        }

        private class ParseParameterTemplateExpected
        {
            public string ParameterTemplate { get; set; }
            public List<string> SortedRouteParameters { get; set; }

            public ParseParameterTemplateExpected(string parameterTemplate, List<string> sortedRouteParameters)
            {
                ParameterTemplate = parameterTemplate;
                SortedRouteParameters = sortedRouteParameters;
            }
        }
    }
}

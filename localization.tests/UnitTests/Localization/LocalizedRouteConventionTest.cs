using localization.Localization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace LocalizationTests.UnitTests.Localization
{
    [TestClass]
    public class LocalizedRouteConventionTest
    {
        [TestMethod]
        public void GetParameterNameTest()
        {
            LocalizedRouteConvention routeConvention = new LocalizedRouteConvention();

            List<string> input = new List<string>() { "{action}", "{ CoNtRoLLeR }", "{ moo:id = 1337}", "{howl=1337}" };
            List<string> expectedOutput = new List<string>() { "action", "controller", "moo", "howl" };

            for (int i = 0; i < input.Count; i++)
            {
                string name = routeConvention.GetParameterName(input[i]);

                Assert.AreEqual(expectedOutput[i], name);
            }
        }
    }
}

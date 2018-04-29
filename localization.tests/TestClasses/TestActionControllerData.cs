using localization.Localization;
using System;
using System.Collections.Generic;
using System.Text;

namespace localization.tests.TestClasses
{
    class TestActionControllerData
    {
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Culture { get; set; } = LocalizationRouteDataHandler.DefaultCulture;
        public Dictionary<string, string> RouteValues { get; set; } = null;

        public TestActionControllerData()
        {

        }

        public TestActionControllerData(string controller, string action)
        {
            Controller = controller;
            Action = action;
        }

        public TestActionControllerData(string controller, string action, Dictionary<string, string> routeValues)
            : this(controller, action)
        {
            RouteValues = routeValues;
        }

        public TestActionControllerData(string controller, string action, Dictionary<string, string> routeValues, string culture)
            : this(controller, action, routeValues)
        {
            Culture = culture;
        }
    }
}
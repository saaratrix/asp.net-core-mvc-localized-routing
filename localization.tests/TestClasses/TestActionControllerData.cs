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
        public string Culture { get; set; } = LocalizationDataHandler.DefaultCulture;
        public Dictionary<string, string> RouteValues { get; set; } = null;

        public TestActionControllerData()
        {

        }

        public TestActionControllerData(string a_controller, string a_action)
        {
            Controller = a_controller;
            Action = a_action;
        }

        public TestActionControllerData(string a_controller, string a_action, Dictionary<string, string> a_routeValues)
            : this(a_controller, a_action)
        {
            RouteValues = a_routeValues;
        }

        public TestActionControllerData(string a_controller, string a_action, Dictionary<string, string> a_routeValues, string a_culture)
            : this(a_controller, a_action, a_routeValues)
        {
            Culture = a_culture;
        }
    }
}
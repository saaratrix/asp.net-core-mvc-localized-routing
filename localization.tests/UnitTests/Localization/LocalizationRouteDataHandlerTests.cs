using System.Linq;
using localization.Localization;
using localization.tests.TestClasses;

namespace localization.tests.UnitTests.Localization
{
	using NUnit.Framework;

	[TestFixture]
	public class LocalizationRouteDataHandlerTests
	{
		private const string DefaultCulture = "en";
		
		[Test]
		public void AddControllerRouteData_ValidTest()
		{
			string controller = "test";
			string culture = "fi";
			string route = "testi";
			
			TestTearDownWorked();
			
			LocalizationRouteDataHandler.AddControllerRouteData(controller, culture, route);
			
			Assert.IsTrue(LocalizationRouteDataHandler.ControllerRoutes.Count == 1, "A controller route should be added.");
			Assert.IsTrue(LocalizationRouteDataHandler.ControllerRoutes.ContainsKey(controller), "Controller should be correctly added.");

			var controllerRoute = LocalizationRouteDataHandler.ControllerRoutes[controller];
			
			Assert.IsTrue(controllerRoute.ControllerName == controller, "Controller name is the same as the controller input.");
			
			Assert.IsTrue(controllerRoute.Routes.Count == 1, "1 should be route added.");
			Assert.IsTrue(controllerRoute.Routes.First(r => r.Key == "fi").Value == route, "Finnish route should be the one added.");
			
			Assert.IsTrue(LocalizationRouteDataHandler.LocalizedControllerNames.Count == 1, "One localized controller should be added to LocalizedControllerNames.");
			Assert.IsTrue(LocalizationRouteDataHandler.LocalizedControllerNames[$"{culture}/{route}"] == controllerRoute, "Localized controller should be equal to controller route.");
			
			Assert.IsTrue(controllerRoute.Actions.Count == 0, "No actions added because no method to add actions was called.");
			
			// Add a second culture
			string otherCulture      = "sv-FI";
			string otherCultureRoute = "testa";
			LocalizationRouteDataHandler.AddControllerRouteData(controller, otherCulture, otherCultureRoute);
			
			Assert.IsTrue(controllerRoute.Routes.Count == 2, "other culture route was added to controller");
			Assert.IsTrue(controllerRoute.Routes.First(r => r.Key == otherCulture).Value == otherCultureRoute, "Correct other culture & route was added for controller.");
			
			Assert.IsTrue(LocalizationRouteDataHandler.LocalizedControllerNames.Count == 2, "2 localized controller should exist after adding other culture route.");
			Assert.IsTrue(LocalizationRouteDataHandler.LocalizedControllerNames[$"{otherCulture}/{otherCultureRoute}"] == controllerRoute, "Other localized culture controller should be the same as the controller route.");
			
			// Add a second route
			string otherController = "testb";
			string otherRoute = "testii";
			
			LocalizationRouteDataHandler.AddControllerRouteData(otherController, culture, otherRoute);
			Assert.IsTrue(LocalizationRouteDataHandler.ControllerRoutes.Count == 2, "Second controller route should be added");
			Assert.IsTrue(LocalizationRouteDataHandler.ControllerRoutes.ContainsKey(otherController), "Correct second controller should be added");

			var otherControllerRoute = LocalizationRouteDataHandler.ControllerRoutes[otherController];
			
			Assert.IsTrue(LocalizationRouteDataHandler.LocalizedControllerNames.Count == 3, "3 localized controller names should be added to LocalizedControllerNames.");
			Assert.IsTrue(LocalizationRouteDataHandler.LocalizedControllerNames[$"{culture}/{otherRoute}"] == otherControllerRoute, "Other localized controller should be equal to other controller route.");
		}

		[Test]
		[TestCase("test", "", "testi")]
		[TestCase("test", "fi", "")]
		[TestCase("test", "", "")]
		[TestCase("test", null, null)]
		public void AddControllerRouteData_InvalidTest(string controller, string culture, string route)
		{
			TestTearDownWorked();
			
			LocalizationRouteDataHandler.AddControllerRouteData(controller, culture, route);

			// Test this because there is static data!
			Assert.IsTrue(LocalizationRouteDataHandler.ControllerRoutes.Count == 1, "ControllerRoute should be added.");
			Assert.IsTrue(LocalizationRouteDataHandler.LocalizedControllerNames.Count == 0, "No Localized controller names added because of bad input data.");

			var controllerRoute = LocalizationRouteDataHandler.ControllerRoutes[controller];
			Assert.IsTrue(controllerRoute.Routes.Count == 0, "No routes should have been added");
			Assert.IsTrue(controllerRoute.Actions.Count == 0, "No actions should be added");
		}
		
		[Test]
		public void AddControllerRouteData_FromAddActionTest()
		{
			TestTearDownWorked();

			string controller = "testcontroller";
			string action = "testaction";
			
			LocalizationRouteDataHandler.AddActionRouteData(controller, action, "", "");
			Assert.IsTrue(LocalizationRouteDataHandler.ControllerRoutes.ContainsKey(controller));
		}

		[Test]
		public void AddActionRouteData_ValidTest()
		{
			string controller = "testcontroller";
			string action     = "testaction";
			string culture    = "fi";
			string route      = "testi";
			
			LocalizationRouteDataHandler.AddActionRouteData(controller, action, culture, route);

			var controllerRoute = LocalizationRouteDataHandler.ControllerRoutes[controller];
			
			Assert.IsTrue(controllerRoute.Actions.Count == 1, "Action should be added");
			Assert.IsTrue(controllerRoute.Actions.ContainsKey(action), "Correct action was added");
			
			var actionRoute = controllerRoute.Actions[action];
			Assert.IsTrue(actionRoute.ActionName == action, "ActionName was correctly set.");
			Assert.IsTrue(actionRoute.Routes.Count == 1, "1 route was added to action");
			Assert.IsTrue(actionRoute.Routes.First(r => r.Key == culture).Value == route, "Correct culture & route was added for action.");
			
			Assert.IsTrue(controllerRoute.LocalizedActionNames.Count == 1, "1 localized action should exist.");
			Assert.IsTrue(controllerRoute.LocalizedActionNames[$"{culture}/{route}"] == actionRoute, "Localized action should be the same as the action route.");

			// Add second culture
			string otherCulture = "sv-FI";
			string otherCultureRoute = "testa";
			LocalizationRouteDataHandler.AddActionRouteData(controller, action, otherCulture, otherCultureRoute);
			
			Assert.IsTrue(actionRoute.Routes.Count == 2, "other culture route was added to action");
			Assert.IsTrue(actionRoute.Routes.First(r => r.Key == otherCulture).Value == otherCultureRoute, "Correct other culture & route was added for action.");
			
			Assert.IsTrue(controllerRoute.LocalizedActionNames.Count == 2, "2 localized actions should exist after adding other culture route.");
			Assert.IsTrue(controllerRoute.LocalizedActionNames[$"{otherCulture}/{otherCultureRoute}"] == actionRoute, "Localized action should be the same as the action route.");
			
			// Add second route
			string otherAction = "testactionb";
			string otherRoute = "testii";
			
			LocalizationRouteDataHandler.AddActionRouteData(controller, otherAction, culture, otherRoute);
			Assert.IsTrue(controllerRoute.Actions.Count == 2, "Other action should be added");
			var otherActionRoute = controllerRoute.Actions[otherAction];
			
			Assert.IsTrue(actionRoute != otherActionRoute, "Other action added is not the same as first action added.");
			Assert.IsTrue(otherActionRoute.Routes.Count == 1, "Other action should have added route");
			Assert.IsTrue(otherActionRoute.Routes.First(r => r.Key == "fi").Value == otherRoute);
			
			Assert.IsTrue(controllerRoute.LocalizedActionNames.Count == 3, "3 localized action should exist after adding other route.");
			Assert.IsTrue(controllerRoute.LocalizedActionNames[$"{culture}/{otherRoute}"] == otherActionRoute);
		}
			
		[Test]
		[TestCase("testcontroller", "testaction", "", "testi")]
		[TestCase("testcontroller", "testaction", "fi", "")]
		[TestCase("testcontroller", "testaction", "", "")]
		[TestCase("testcontroller", "testaction", null, null)]
		public void AddActionRouteData_InvalidTest(string controller, string action, string culture, string route)
		{
			TestTearDownWorked();
			
			LocalizationRouteDataHandler.AddActionRouteData(controller, action, culture, route);

			var controllerRoute = LocalizationRouteDataHandler.ControllerRoutes[controller];
			Assert.IsTrue(controllerRoute.Actions.Count == 1, "An action was added.");
			Assert.IsTrue(controllerRoute.Actions[action].Routes.Count == 0, "No routes was added for action");
		}

		[Test]
		public void GetRouteDataTest()
		{
			Assert.Fail("Not Implemented.");
		}
		
		[SetUp]
		public void SetUp()
		{
			TestLocalizationRouteDataHandlerHelper.SetUpLocalizationRouteDataHandlerHelper(DefaultCulture);
		}

		[TearDown]
		public void TearDown()
		{
			TestLocalizationRouteDataHandlerHelper.TearDownLocalizationRouteDataHandlerHelper();
		}

		/// <summary>
		/// Just to make sure that the test starts with no data from previous sessions.
		/// </summary>
		public static void TestTearDownWorked()
		{
			// Test this because there is static data!
			Assert.IsTrue(LocalizationRouteDataHandler.ControllerRoutes.Count == 0, "No route should be added at start of test.");
			Assert.IsTrue(LocalizationRouteDataHandler.LocalizedControllerNames.Count == 0, "No LocalizedControllerNames should be added at the start of test.");
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using localization.Localization;
using localization.tests.TestClasses;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace localization.tests.UnitTests.Localization
{
	using NUnit.Framework;

	[TestFixture]
	public class LocalizationRouteConventionTests
	{
		private const string DefaultCulture = "en";
		
		private LocalizationRouteConvention routeConvention;

		[Test]
		public void AddControllerRoutes_Valid_Test()
		{
			var type = typeof(TestController);
			var testController = new ControllerModel(type.GetTypeInfo(), new []
			{
					new LocalizationRouteAttribute("fi", "Testi"),
					new LocalizationRouteAttribute("sv")
			});
			testController.ControllerName = "Test";
			routeConvention.AddControllerRoutes(testController);
			
			Assert.IsTrue(LocalizationRouteDataHandler.ControllerRoutes.ContainsKey("test"), "Controller routes should have added test controller.");
			var controllerRoute = LocalizationRouteDataHandler.ControllerRoutes["test"];

			Assert.IsTrue(controllerRoute.Routes.ContainsKey("fi"), "Finnish culture should be added.");
			Assert.IsTrue(controllerRoute.Routes.First(r => r.Key == "fi").Value == "Testi", "Finnish route should have correct value.");
			Assert.IsTrue(!controllerRoute.Routes.ContainsKey("sv"), "Swedish culture should not be added because route was empty.");
			// Should not contain default culture
			Assert.IsTrue(!controllerRoute.Routes.ContainsKey(DefaultCulture), "default culture shouldn't be added.");
			
			Assert.Fail("Need to update, simplify checks and move it to LocalizationRouteDataHandler tests instead.");
		}

		[Test]
		public void AddActionRoutes_Valid_Test()
		{
			var type = GetType();
			string methodName = "ActionsTestMethod";
			
			var actionModel = new ActionModel(type.GetMethod(methodName), new []
			{
					new LocalizationRouteAttribute("fi", "TestiA"),
					new LocalizationRouteAttribute("sv")
			});
			actionModel.ActionName = methodName;
			var controllerName = "test";
			
			LocalizationRouteDataHandler.AddControllerRouteData(controllerName, DefaultCulture, "");
			routeConvention.AddActionRoutes(controllerName, actionModel);
			
			var controllerRoute = LocalizationRouteDataHandler.ControllerRoutes[controllerName];
			Assert.IsTrue(controllerRoute.Actions.ContainsKey(methodName.ToLower()), "Test method data should exist.");

			var action = controllerRoute.Actions[methodName.ToLower()];
			
			Assert.IsTrue(action.Routes.ContainsKey("fi"), "Finnish culture for action should be added.");
			Assert.IsTrue(action.Routes.First(r => r.Key == "fi").Value == "TestiA", "Finnish action should have correct route.");
			Assert.IsTrue(!action.Routes.ContainsKey("sv"), "Swedish culture for action should not be added.");
			Assert.IsTrue(!action.Routes.ContainsKey(DefaultCulture), "Default culture should not be added to action culture.");

			controllerName = "othercontroller";
			routeConvention.AddActionRoutes(controllerName, actionModel);
			
			Assert.IsTrue(LocalizationRouteDataHandler.ControllerRoutes.ContainsKey(controllerName));
			controllerRoute = LocalizationRouteDataHandler.ControllerRoutes[controllerName];
			Assert.IsTrue(controllerRoute.Actions.ContainsKey(methodName.ToLower()), "Test method data should exist for a second controller.");
			
			Assert.Fail("Need to update, simplify checks and move it to LocalizationRouteDataHandler tests instead.");
		}

		[SetUp]
		public void SetUp()
		{
			LocalizationRouteDataHandler.DefaultCulture = DefaultCulture;
			routeConvention = new LocalizationRouteConvention();
		}

		[TearDown]
		public void TearDown()
		{
			LocalizationRouteDataHandler.DefaultCulture    = "";
			LocalizationRouteDataHandler.ControllerRoutes.Clear();
		}
		
		/// <summary>
		/// Used by the tests to get a MethodInfo.
		/// </summary>
		public void ActionsTestMethod() { }
	}
}
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
					new LocalizationRouteAttribute("sv"),
					new LocalizationRouteAttribute("", "テスト")
			});
			testController.ControllerName = "Test";
			routeConvention.AddControllerRoutes(testController);
			
			Assert.IsTrue(LocalizationRouteDataHandler.ControllerRoutes.ContainsKey("test"), "Controller routes should have added test controller.");
			var controllerRoute = LocalizationRouteDataHandler.ControllerRoutes["test"];

			Assert.IsTrue(controllerRoute.Routes.Count == 1, "Only 1 route should have been added.");
		}

		[Test]
		public void AddActionRoutes_Valid_Test()
		{
			var type = GetType();
			string methodName = "ActionsTestMethod";
			
			var actionModel = new ActionModel(type.GetMethod(methodName), new []
			{
					new LocalizationRouteAttribute("fi", "TestiA"),
					new LocalizationRouteAttribute("sv"),
					new LocalizationRouteAttribute("", "テスト")
			});
			actionModel.ActionName = methodName;
			var controllerName = "test";
			
			LocalizationRouteDataHandler.AddControllerRouteData(controllerName, DefaultCulture, "");
			routeConvention.AddActionRoutes(controllerName, actionModel);
			
			var controllerRoute = LocalizationRouteDataHandler.ControllerRoutes[controllerName];
			Assert.IsTrue(controllerRoute.Actions.ContainsKey(methodName.ToLower()), "Test method data should exist.");

			Assert.IsTrue(controllerRoute.Actions[methodName.ToLower()].Routes.Count == 1, "Only 1 action should have been added");
		}

		[SetUp]
		public void SetUp()
		{
			TestLocalizationRouteDataHandlerHelper.SetUpLocalizationRouteDataHandlerHelper(DefaultCulture);
			routeConvention = new LocalizationRouteConvention();
		}

		[TearDown]
		public void TearDown()
		{
			TestLocalizationRouteDataHandlerHelper.TearDownLocalizationRouteDataHandlerHelper();
		}
		
		/// <summary>
		/// Used by the tests to get a MethodInfo.
		/// </summary>
		public void ActionsTestMethod() { }
	}
}
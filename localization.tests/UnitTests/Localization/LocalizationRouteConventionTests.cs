using System;
using System.Collections.Generic;
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

			Assert.IsTrue(controllerRoute.Names.ContainsKey("fi"), "finnish culture should be added.");
			Assert.IsTrue(!controllerRoute.Names.ContainsKey("sv"), "swedish culture should not be added because route was empty.");
			// Should not contain default culture
			Assert.IsTrue(!controllerRoute.Names.ContainsKey(DefaultCulture), "default culture shouldn't be added.");
		}
		
		[Test]
		public void AddControllerRoutes_UnknownCulture_ThrowsTest()
		{
			var type = typeof(TestController);
			ControllerModel testController = new ControllerModel(type.GetTypeInfo(), new List<LocalizationRouteAttribute>()
			{
					new LocalizationRouteAttribute("fi", "Testi"),
					new LocalizationRouteAttribute("sv"),
					new LocalizationRouteAttribute("ja", "テスト")
			});
			testController.ControllerName = "Test";
			Assert.Throws<Exception>(() => routeConvention.AddControllerRoutes(testController) );
		}

		[Test]
		public void AddActionRoutes_Valid_Test()
		{
			var type = GetType();
			string methodName = "ActionsTestMethod";
			
			var actionModel = new ActionModel(type.GetMethod("ActionsTestMethod"), new []
			{
					new LocalizationRouteAttribute("fi", "TestiA"),
					new LocalizationRouteAttribute("sv")
			});
			var controllerName = "test";
			
			LocalizationRouteDataHandler.AddControllerRouteData(controllerName, DefaultCulture, "");
			routeConvention.AddActionRoutes(controllerName, actionModel);
			
			var controllerRoute = LocalizationRouteDataHandler.ControllerRoutes[controllerName];
			Assert.IsTrue(controllerRoute.Actions.ContainsKey(methodName.ToLower()), "Test method data should exist");

			var action = controllerRoute.Actions[methodName.ToLower()];

			Assert.Fail("Not implemented");
		}

		[Test]
		public void AddActionRoutes_UnknownCulture_Test()
		{
			Assert.Fail("Not Implemented");
		}

		[SetUp]
		public void SetUp()
		{
			LocalizationRouteDataHandler.DefaultCulture = DefaultCulture;
			LocalizationRouteDataHandler.SupportedCultures = new Dictionary<string, string>()
			{
					{ DefaultCulture, "English" },
					{ "fi", "Suomeksi" },
					{ "sv", "Svenska" }
			};
			
			routeConvention = new LocalizationRouteConvention();
		}

		[TearDown]
		public void TearDown()
		{
			LocalizationRouteDataHandler.DefaultCulture    = "en";
			LocalizationRouteDataHandler.SupportedCultures = new Dictionary<string, string>();

			LocalizationRouteDataHandler.ControllerRoutes.Clear();
		}
		
		/// <summary>
		/// Used by the tests to get a MethodInfo.
		/// </summary>
		public void ActionsTestMethod() { }
	}
}
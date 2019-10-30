using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace localization.Localization
{
	/// <summary>
	/// This class goes over all controllers and actions to generate the localization route data.
	/// </summary>
	public class LocalizationRouteConvention : IApplicationModelConvention
	{
		public void Apply(ApplicationModel application)
		{
			foreach (var controller in application.Controllers)
			{
				AddControllerRoutes(controller);

				foreach (var actionModel in controller.Actions)
				{
					AddActionRoutes(controller.ControllerName, actionModel);
				}
			}
		}

		public void AddControllerRoutes(ControllerModel controller)
		{
			var attributes = controller.Attributes.OfType<LocalizationRouteAttribute>().ToList();

			foreach (var attribute in attributes)
			{
				// There is no need to add route data for an empty attribute.
				// Because it's the same as falling back to default culture.
				if (string.IsNullOrEmpty(attribute.Route))
					continue;
				
				if (!LocalizationRouteDataHandler.SupportedCultures.ContainsKey(attribute.Culture))
					throw new Exception($"Culture: {attribute.Culture} not found for controller: {controller.ControllerName}");
				
				LocalizationRouteDataHandler.AddControllerRouteData(controller.ControllerName, attribute.Culture, attribute.Route);
			}
		}

		public void AddActionRoutes(string controllerName, ActionModel action)
		{
			var attributes = action.Attributes.OfType<LocalizationRouteAttribute>().ToList();

			foreach (var attribute in attributes)
			{
				if (string.IsNullOrEmpty(attribute.Route))
					continue;
				
				if (!LocalizationRouteDataHandler.SupportedCultures.ContainsKey(attribute.Culture))
					throw new Exception($"Culture: {attribute.Culture} not found for controller: {controllerName} action: {action.ActionName}");
				
				LocalizationRouteDataHandler.AddActionRouteData(controllerName, action.ActionName, attribute.Culture, attribute.Route);
			}
		}
	}
}
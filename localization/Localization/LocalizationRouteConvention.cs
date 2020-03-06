using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Routing;

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
				if (string.IsNullOrWhiteSpace(attribute.Culture) || string.IsNullOrWhiteSpace(attribute.Route))
					continue;

				LocalizationRouteDataHandler.AddControllerRouteData(controller.ControllerName, attribute.Culture, attribute.Route);
			}
		}

		public void AddActionRoutes(string controllerName, ActionModel action)
		{
			var attributes = action.Attributes.OfType<LocalizationRouteAttribute>().ToList();

			var parameters = action.Parameters.Select(p => p.ParameterName);
			// Get Methods, because it can be many ... :)
			var methodRouteInfos = GetActionMethods(action);

			foreach (var (method, template) in methodRouteInfos)
			{
				// example/{param1}/other/{param2}
				var routeFragments = GetRouteFragments(template, parameters);
				
				foreach (var attribute in attributes)
				{
					if (string.IsNullOrWhiteSpace(attribute.Culture) || string.IsNullOrWhiteSpace(attribute.Route))
						continue;
					// Adds localized action and the action
					LocalizationRouteDataHandler.AddActionRouteData(controllerName, action.ActionName, attribute.Culture, attribute.Route);
					LocalizationRouteDataHandler.AddMethodRouteData(controllerName, action.ActionName, method, routeFragments, attribute.Culture, attribute.Route);
				}
			}
		}

		public List<RouteFragmentType> GetRouteFragments(string template)
		{
			var result = new List<RouteFragmentType>();
			var templateParts = template.Split('/');
			foreach (var part in templateParts)
			{
				var trimmedPart = part.Trim();
				if (trimmedPart.StartsWith("{"))
					// If the parameters would require indexing based on name this could be done here as well.
					result.Add(RouteFragmentType.Parameter);
				else
					result.Add(RouteFragmentType.Template);
			}

			return result;
		}

		/// <summary>
		/// Get the HttpMethod and the route.
		/// </summary>
		/// <param name="action"></param>
		/// <returns></returns>
		public List<(HttpMethod, string)> GetActionMethods(ActionModel action)
		{
			List<(HttpMethod, string)> result = new List<(HttpMethod, string)>();

			var    routeAttribute = action.Attributes.OfType<RouteAttribute>().FirstOrDefault();
			string routeTemplate  = routeAttribute?.Template ?? action.ActionName;

			foreach (var attribute in action.Attributes)
			{
				if (!(attribute is HttpMethodAttribute httpMethodAttribute))
					continue;

				string template   = !string.IsNullOrEmpty(httpMethodAttribute.Template) ? httpMethodAttribute.Template : routeTemplate;
				var    httpMethod = new HttpMethod(httpMethodAttribute.HttpMethods.First());
				
				result.Add((httpMethod, template));
			}

			// If no attribute was found then add route as HttpMethod.Get.
			if (result.Count == 0)
			{
				result.Add((HttpMethod.Get, routeTemplate));
			}

			return result;
		}
	}
}
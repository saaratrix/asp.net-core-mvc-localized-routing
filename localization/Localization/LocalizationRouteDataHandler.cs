using System;
using System.Collections.Generic;
using System.Linq;
using localization.Localization.CultureRouteData;

namespace localization.Localization
{
	/// <summary>
	/// LocalizationRouteDataHandler handles storing all the localized route data and generating the localized urls.
	/// It also has the supported cultures and default culture.
	/// Set the supported cultures and default culture either in the class itself or in Startup.cs
	/// </summary>
	public static class LocalizationRouteDataHandler
	{
		/// <summary>
		/// The default culture.
		/// </summary>
		public static string DefaultCulture { get; set; }
		
		/// <summary>
		/// The controller name localized to be able to do a reverse lookup.
		/// Eg: {culture}/{controller} - fi/ohjain
		/// </summary>
		public static Dictionary<string, CultureControllerRouteData> LocalizedControllerNames { get; } = new Dictionary<string, CultureControllerRouteData>();
		public static Dictionary<string, CultureControllerRouteData> ControllerRoutes { get; } = new Dictionary<string, CultureControllerRouteData>();
		
		public static void AddControllerRouteData(string controller, string? culture, string? route)
		{
			controller = controller.ToLower();
			     
			if (!ControllerRoutes.ContainsKey(controller))
				ControllerRoutes.Add(controller, new CultureControllerRouteData(controller));

			if (string.IsNullOrWhiteSpace(culture) || string.IsNullOrWhiteSpace(route)) 
				return;
			
			ControllerRoutes[controller].Routes.TryAdd(culture, route);
			LocalizedControllerNames.Add($"{culture}/{route}", ControllerRoutes[controller]);
		}

		public static void AddActionRouteData(string controller, string action, string? culture, string? route)
		{
			controller = controller.ToLower();
			action = action.ToLower();
			// Add controller route data if it doesn't already exist or we can't add the actions.
			if (!ControllerRoutes.ContainsKey(controller))
				AddControllerRouteData(controller, null, null);

			var controllerRoute = ControllerRoutes[controller];

			if (!controllerRoute.Actions.ContainsKey(action))
				controllerRoute.Actions.Add(action, new CultureActionRouteData(action));

			if (string.IsNullOrWhiteSpace(culture) || string.IsNullOrWhiteSpace(route))
				return;
			
			controllerRoute.Actions[action].Routes.TryAdd(culture, route);
			controllerRoute.LocalizedActionNames.Add($"{culture}/{route}", controllerRoute.Actions[action]);
		}

		/// <summary>
		/// The input data comes in localized form so we need to return the ASP.NET controller & action names. 
		/// </summary>
		/// <param name="controller">A localized controller route.</param>
		/// <param name="action">A localized action route.</param>
		/// <param name="culture">The culture used to do reverse lookup from localized route.</param>
		/// <returns></returns>
		public static LocalizationRouteData GetRouteData(string controller, string action, string culture)
		{
			controller = controller.ToLower();
			action = action.ToLower();
			
			var controllerRoute = GetControllerRouteData(controller, culture);
			// If no controller route data is found there is no point to look for an action, so return early.
			if (controllerRoute == null)
				return new LocalizationRouteData(null, controller, action);
			// For example if controller was in finnish this will make sure that it's the controller name used by ASP.NET.
			controller = controllerRoute.ControllerName;
			action = GetActionRouteData(controllerRoute, action, culture);
			
			return new LocalizationRouteData(null, controller, action);
		}

		private static CultureControllerRouteData? GetControllerRouteData(string controller, string culture)
		{
			// First we get the localized controller
			string localizedControllerKey = $"{culture}/{controller}";
			var controllerRoute = LocalizedControllerNames.ContainsKey(localizedControllerKey) 
					? LocalizedControllerNames[localizedControllerKey] : null;

			if (controllerRoute == null)
				controllerRoute = ControllerRoutes.ContainsKey(controller) ? ControllerRoutes[controller] : null;
			
			return controllerRoute;
		}

		private static string GetActionRouteData(CultureControllerRouteData controllerRoute, string action, string culture)
		{
			string localizedActionKey = $"{culture}/{action}";
			var actionRoute = controllerRoute.LocalizedActionNames.ContainsKey(localizedActionKey) 
					? controllerRoute.LocalizedActionNames[localizedActionKey] : null;

			if (actionRoute == null)
				actionRoute = controllerRoute.Actions.ContainsKey(action) ? controllerRoute.Actions[action] : null;
			// Set the action as the actionName used by ASP.NET or the input action.
			action = actionRoute?.ActionName ?? action;

			return action;
		}
	}
}
using System.Collections.Generic;
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
		
		public static void AddControllerRouteData(string controller, string culture, string route)
		{
			controller = controller.ToLower();
			     
			if (!ControllerRoutes.ContainsKey(controller))
				ControllerRoutes.Add(controller, new CultureControllerRouteData(controller));

			if (string.IsNullOrWhiteSpace(culture) || string.IsNullOrWhiteSpace(route)) 
				return;
			
			ControllerRoutes[controller].Routes.TryAdd(culture, route);
			LocalizedControllerNames.Add($"{culture}/{route}", ControllerRoutes[controller]);
		}

		public static void AddActionRouteData(string controller, string action, string culture, string route)
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
		/// The input data comes in localized form so we need to return the normal controller and action.
		/// </summary>
		/// <param name="controller"></param>
		/// <param name="action"></param>
		/// <returns></returns>
		public static LocalizationRouteData GetRouteData(string controller, string action)
		{
			controller = controller.ToLower();
			action = action.ToLower();
			
			return null;
		}
	}
}
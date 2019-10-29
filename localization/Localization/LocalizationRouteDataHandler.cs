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
		/// The dictionary of all supported cultures.
		/// Key = Culture Name
		/// Value = Display Name
		/// Example: en, English
		/// </summary>
		public static Dictionary<string, string> SupportedCultures { get; set; }
		
		public static Dictionary<string, CultureControllerRouteData> ControllerRoutes { get; } = new Dictionary<string, CultureControllerRouteData>();
		
		public static void AddControllerRouteData(string controller, string culture, string route)
		{
			string controllerKey = controller.ToLower();
            
			// If the controller doesn't exist, create it!            
			if (!ControllerRoutes.ContainsKey(controllerKey))
			{                
				ControllerRoutes.TryAdd(controllerKey, new CultureControllerRouteData());
			}            
			ControllerRoutes[controllerKey].Names.TryAdd(culture, route);	
		}

		public static void AddActionRouteData(string controller, string action, string culture, string route)
		{
			
		}
	}
}
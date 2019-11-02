using System.Collections.Generic;

namespace localization.Localization.CultureRouteData
{
	public class CultureControllerRouteData
	{
		public string ControllerName { get; }
		/// <summary>
		/// Different routes of the controller for different cultures.
		/// It's for binding the dynamic map route data or generating a localized url.
		/// </summary>
		public Dictionary<string, string> Routes { get; } = new Dictionary<string, string>();
		/// <summary>
		/// The action in its localized name so we can do a reverse lookup.
		/// Eg: {culture}/{action} - fi/toiminto
		/// </summary>
		public Dictionary<string, CultureActionRouteData> LocalizedActionNames { get; } = new Dictionary<string, CultureActionRouteData>();
		public Dictionary<string, CultureActionRouteData> Actions { get; } = new Dictionary<string, CultureActionRouteData>();

		public CultureControllerRouteData(string controllerName)
		{
			ControllerName = controllerName;
		}
	}
}
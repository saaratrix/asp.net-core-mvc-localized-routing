using System.Collections.Generic;

namespace localization.Localization.CultureRouteData
{
	public class CultureControllerRouteData
	{
		/// <summary>
		/// Different names of the controller in different cultures.
		/// It's used when binding the dynamic map route data or generating a localized url.
		/// </summary>
		public Dictionary<string, string> Names { get; } = new Dictionary<string, string>();
			
		public Dictionary<string, CultureActionRouteData> Actions { get; } = new Dictionary<string,CultureActionRouteData>();
		
	}
}
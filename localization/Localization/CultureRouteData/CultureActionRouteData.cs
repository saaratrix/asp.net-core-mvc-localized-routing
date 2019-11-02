using System.Collections.Generic;

namespace localization.Localization.CultureRouteData
{
	public class CultureActionRouteData
	{
		public string ActionName { get; }
		/// <summary>
		/// Different routes of the action for different cultures.
		/// It's for binding the dynamic map route data or generating a localized url.
		/// </summary>
		public Dictionary<string, string> Routes { get; } = new Dictionary<string, string>();

		public CultureActionRouteData(string actionName)
		{
			ActionName = actionName;
		}
	}
}
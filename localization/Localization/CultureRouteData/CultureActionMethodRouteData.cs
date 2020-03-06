using System.Collections.Generic;
using System.Net.Http;

namespace localization.Localization.CultureRouteData
{
	public class CultureActionMethodRouteData
	{
		public HttpMethod Method { get; }
		/// <summary>
		/// Different routes of the action for different cultures.
		/// It's for binding the dynamic map route data or generating a localized url.
		/// </summary>
		public Dictionary<string, string> Routes { get; } = new Dictionary<string, string>();
		
		/// <summary>
		/// Value = Parameter name
		/// </summary>
		public List<string> Parameters { get; } = new List<string>();

		public CultureActionMethodRouteData(HttpMethod method)
		{
			Method = method;
		}
	}
}
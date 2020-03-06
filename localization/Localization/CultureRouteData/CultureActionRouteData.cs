using System.Collections.Generic;
using System.Net.Http;

namespace localization.Localization.CultureRouteData
{
	public class CultureActionRouteData
	{
		public string ActionName { get; }
		
		public Dictionary<HttpMethod, CultureActionMethodRouteData> Methods { get; } = new Dictionary<HttpMethod, CultureActionMethodRouteData>();

		public CultureActionRouteData(string actionName)
		{
			ActionName = actionName;
		}
	}
}
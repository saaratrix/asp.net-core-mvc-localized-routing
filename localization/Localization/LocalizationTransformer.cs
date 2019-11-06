using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;

namespace localization.Localization
{
	public class LocalizationTransformer : DynamicRouteValueTransformer
	{
		public override async ValueTask<RouteValueDictionary> TransformAsync(HttpContext httpContext, RouteValueDictionary values)
		{
			if (!values.ContainsKey("culture") || !values.ContainsKey("controller"))
				return values;

			string controller = (string)values["controller"];
			string action = (string) values["action"];
			string culture = "fi";//(string) values["culture"];

			var routeData = LocalizationRouteDataHandler.GetRouteData(controller, action, culture);

			values["controller"] = routeData.Controller;
			values["action"] = routeData.Action;

			return values;
		}
	}
}
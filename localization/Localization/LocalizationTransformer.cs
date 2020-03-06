using System.Net.Http;
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
			string culture = (string) values["culture"];

			var routeData = LocalizationRouteDataHandler.GetRouteData(controller, action, HttpMethod.Get, culture);

			values["controller"] = routeData.Controller;
			values["action"] = routeData.Action;

			if (values.ContainsKey("params") && values["params"] != null)
			{
				var parameters = ((string) values["params"]).Split("/");
				values["index"] = parameters[0];
				values["test"] = parameters[1];
			}

			return values;
		}
	}
}
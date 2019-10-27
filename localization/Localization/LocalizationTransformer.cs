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

			values["controller"] = "Home";
			values["action"] = "Privacy";

			return values;
		}
	}
}
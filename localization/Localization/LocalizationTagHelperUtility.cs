using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace localization.Localization
{
    /// <summary>
    /// Shared utility between the TagHelpers.
    /// </summary>
    public static class LocalizationTagHelperUtility
    {
        /// <summary>
        /// Get the url and url with parameter data if possible.
        /// </summary>
        /// <param name="context">The context to get attributes and route values from.</param>
        /// <param name="culture">The culture to get the url result for.</param>
        /// <returns></returns>
        public static LocalizationUrlResult GetUrlResult(TagHelperContext context, string culture)
        {
            string controllerName = context.AllAttributes["asp-controller"].Value as string;
            string actionName = context.AllAttributes["asp-action"].Value as string;

            var urlResult = LocalizationRouteDataHandler.GetUrl(controllerName, actionName, culture);

            Dictionary<string, string> routeValues = GetRouteValues(context.AllAttributes);
            if (routeValues != null)
            {
                urlResult.Url += LocalizationRouteDataHandler.GetOrderedParameters(controllerName, actionName, routeValues);
            }

            return urlResult;
        }

        /// <summary>
        /// Checks all attributes on a tag-helper element if any are asp-all-route-data or asp-route-*
        /// Returns null if no route data was found.
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetRouteValues(ReadOnlyTagHelperAttributeList attributes)
        {
            bool hasAllRouteData = attributes.TryGetAttribute("asp-all-route-data", out TagHelperAttribute allRouteValues);

            if (hasAllRouteData)
            {
                return allRouteValues.Value as Dictionary<string, string>;
            }
            else
            {
                // You can write the attributes as ASP-ROUTE-  and the name will be ASP-ROUTE so need to ignore case
                IEnumerable<TagHelperAttribute> routeAttributes = attributes.Where(x => x.Name.StartsWith("asp-route-", StringComparison.OrdinalIgnoreCase));

                if (routeAttributes.Any())
                {
                    Dictionary<string, string> routeValues = new Dictionary<string, string>();

                    foreach (TagHelperAttribute attribute in routeAttributes)
                    {
                        // asp-route- is 10 characters long
                        string parameter = attribute.Name.Substring(10).ToLower();
                        routeValues.Add(parameter, attribute.Value as string);
                    }

                    return routeValues;
                }
            }

            return null;
        }
    }
}

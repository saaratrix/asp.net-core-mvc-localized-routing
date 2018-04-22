using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace localization.Localization
{
    [HtmlTargetElement("a", Attributes = CultureAttributeName)]
    [HtmlTargetElement("a", Attributes = KeepLinkAttributeName)]
    public class CultureActionLinkTagHelper : TagHelper
    {
        private const string CultureAttributeName = "cms-culture";
        private const string KeepLinkAttributeName = "cms-keep-link";
        /// <summary>
        /// The culture attribute
        /// </summary>        
        [HtmlAttributeName(CultureAttributeName)]
        public string Culture { get; set; }

        [HtmlAttributeName(KeepLinkAttributeName)]
        public bool KeepLink { get; set; } = false;

        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            // This can happen if for example the ViewData["culture"] isn't set
            // like for the errorcontroller which doesn't inherit from LocalizationController.
            if (string.IsNullOrEmpty(Culture))
            {
                Culture = CultureInfo.CurrentCulture.Name;
            }            

            string controllerName = context.AllAttributes["asp-controller"].Value as string;
            string actionName = context.AllAttributes["asp-action"].Value as string;

            Dictionary<string, string> routeValues = GetRouteValues(context.AllAttributes);

            LocalizedUrlResult urlResult = LocalizationDataHandler.GetUrl(controllerName, actionName, Culture);            
            if (routeValues != null)
            {
                urlResult.Url += LocalizationDataHandler.GetOrderedParameters(controllerName, actionName, routeValues);
            }

            output.Attributes.SetAttribute("href", urlResult.Url);

            if (!KeepLink && urlResult.LinkName != "")
            {
                output.Content.SetContent(urlResult.LinkName);
            }

            return Task.FromResult(0);
        }

        private Dictionary<string, string> GetRouteValues(ReadOnlyTagHelperAttributeList attributes)
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

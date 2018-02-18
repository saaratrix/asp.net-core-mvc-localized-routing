using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace localization.Localization
{
    [HtmlTargetElement("form", Attributes = CultureAttributeName)]
    public class CultureFormLinkTagHelper : TagHelper
    {
        private const string CultureAttributeName = "cms-culture";
        /// <summary>
        /// The culture attribute
        /// </summary>        
        [HtmlAttributeName(CultureAttributeName)]
        public string Culture { get; set; }

        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (string.IsNullOrEmpty(Culture))
            {
                Culture = LocalizationDataHandler.DefaultCulture;
            }

            // NOTE: This currently removes ?querystring and #fragments
                      
            
            // Get the controllerName and actionName
            string controllerName = context.AllAttributes["asp-controller"].Value as string;
            string actionName = context.AllAttributes["asp-action"].Value as string;

            LocalizedUrlResult result = LocalizationDataHandler.GetUrl(controllerName, actionName, Culture);
            string finalUrl = result.Url;            

            output.Attributes.SetAttribute("action", finalUrl);

            return base.ProcessAsync(context, output);
        }
    }
}

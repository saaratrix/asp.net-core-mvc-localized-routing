using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace localization.Localization
{
    [HtmlTargetElement("a", Attributes = CultureAttributeName)]
    public class CultureActionLinkTagHelper : TagHelper
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
                
            // Get the controllerName and actionName
            string controllerName = context.AllAttributes["asp-controller"].Value as string;
            string actionName = context.AllAttributes["asp-action"].Value as string;

            LocalizedUrlResult result = LocalizationDataHandler.GetUrl(controllerName, actionName, Culture);
            
            output.Attributes.SetAttribute("href", result.Url);
            if (result.LinkName != "")
            {
                output.Content.SetContent(result.LinkName);
            }           

            return Task.FromResult(0);
        }
    }
}

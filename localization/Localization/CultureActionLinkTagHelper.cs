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

            LocalizedUrlResult urlResult = LocalizationTagHelperUtility.GetUrlResult(context, Culture);

            output.Attributes.SetAttribute("href", urlResult.Url);

            if (!KeepLink && urlResult.LinkName != "")
            {
                output.Content.SetContent(urlResult.LinkName);
            }

            return Task.FromResult(0);
        }
    }
}

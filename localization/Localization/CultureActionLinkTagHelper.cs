using System.Globalization;
using System.Threading.Tasks;
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
        /// The culture to use attribute.
        /// </summary>        
        [HtmlAttributeName(CultureAttributeName)]
        public string Culture { get; set; }

        /// <summary>
        /// If the anchor tags innerText should kept or not.
        /// If true the value in the view is kept.
        /// If false then the value is the LocalizedUrlResult.LinkName .
        /// </summary>
        [HtmlAttributeName(KeepLinkAttributeName)]
        public bool KeepLink { get; set; } = false;

        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            // This happens for example if cms-culture="" is left empty
            // Which means the current culture set by the RequestProvider is used.
            if (string.IsNullOrEmpty(Culture))
            {
                Culture = CultureInfo.CurrentCulture.Name;
            }

            var urlResult = LocalizationTagHelperUtility.GetUrlResult(context, Culture);

            output.Attributes.SetAttribute("href", urlResult.Url);

            if (!KeepLink && urlResult.LinkName != "")
            {
                output.Content.SetContent(urlResult.LinkName);
            }

            return Task.FromResult(0);
        }
    }
}

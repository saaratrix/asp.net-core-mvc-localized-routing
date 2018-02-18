using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            // The url for <a href="">
            string finalHref;

            // Get the original attribute so we can add asp-route-... values
            TagHelperAttribute hrefAttribute = output.Attributes.FirstOrDefault(x => x.Name == "href");
            string originalHref = hrefAttribute.Value as string;

            string originalCulture = LocalizationDataHandler.GetCultureFromUrl(originalHref);

            // Get the controllerName and actionName
            string controllerName = context.AllAttributes["asp-controller"].Value as string;
            string actionName = context.AllAttributes["asp-action"].Value as string;

            LocalizedUrlResult result = LocalizationDataHandler.GetUrl(controllerName, actionName, Culture);

            // If the two cultures don't match then replace the originalHref URL part with the new result URL
            // So that the URL is non-destructive of any other parts that's on the URL such as #? or /parameter1/parameter2
            if (originalCulture != Culture)
            {
                LocalizedUrlResult originalResult = LocalizationDataHandler.GetUrl(controllerName, actionName, originalCulture);

                // Use Uri instead and add fragment & query after reconstructing base url?
                // Currently doesn't take that into account very well
                string resultUrl = result.Url;
                // Add / if last character isn't a /
                if (resultUrl[ resultUrl.Length - 1 ] != '/')
                {
                    resultUrl = resultUrl + "/";
                }

                int originalPartsCount = originalResult.Url.Count(x => x == '/');
                int partsFound = 0;

                StringBuilder sb = new StringBuilder(resultUrl);
                foreach (char letter in originalHref)
                {
                    if (partsFound <= originalPartsCount)
                    {
                        if (letter == '/')
                        {
                            partsFound++;
                        }
                        else if (letter == '?')
                        {
                            partsFound = originalPartsCount;
                        }
                        else if (letter == '#')
                        {
                            partsFound = originalPartsCount;
                        }
                    }
                    else
                    {
                        sb.Append(letter);
                    }
                }

                finalHref = sb.ToString();
            }
            else
            {
                // Currently this makes default controller ( Home ) the url:
                // "/Home/{Action}"   but for finnish & swedish it's just /fi/{Action}  and /sv/{Action}
                // Meaning the controller name for finnish & swedish is actually /fi/ and /sv/! 
                finalHref = originalHref;
            }
            
            output.Attributes.SetAttribute("href", finalHref);
            if (result.LinkName != "")
            {   
                output.Content.SetContent(result.LinkName);
            }

            return Task.FromResult(0);
        }
    }
}

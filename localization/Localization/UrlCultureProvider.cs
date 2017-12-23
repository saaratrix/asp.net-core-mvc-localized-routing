using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;

namespace localization.Localization
{
    /// <summary>
    /// Determines the culture information for a request via the value of the start of a url.
    /// </summary>
    public class UrlCultureProvider : RequestCultureProvider
    {       
        /// <summary>
        /// The default culture if none is found
        /// </summary>
        public string DefaultCulture { get; set; } = LocalizationDataHandler.DefaultCulture;

        /// <summary>
        /// The supported cultures from url
        /// </summary>
        public IList<CultureInfo> SupportedCultures {get; set;}

        public UrlCultureProvider(IList<CultureInfo> a_supportedCultures)
        {
            SupportedCultures = a_supportedCultures;
        }              

        public override Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            string url = httpContext.Request.Path.Value;
            int pathLength = url.Length;
            // example: /fi
            if (pathLength >= 3)
            {   
                if (url.Length >= 4)
                {
                    // If the 4th character isn't a /   for example
                    // /fi/...   then return default culture
                    if (url[3] != '/' )
                    {
                        return Task.FromResult(new ProviderCultureResult(DefaultCulture));
                    }
                }

                // Remove the /
                string startPath = url.Substring(1, 2);

                foreach (CultureInfo culture in SupportedCultures)
                {
                    if (culture.Name == startPath)
                    {
                        return Task.FromResult(new ProviderCultureResult(culture.Name));
                    }
                }                
            }

            return Task.FromResult(new ProviderCultureResult(DefaultCulture));
        }
    }
}

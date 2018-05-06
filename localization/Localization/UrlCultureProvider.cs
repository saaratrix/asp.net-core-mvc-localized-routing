using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;

namespace localization.Localization
{
    /// <summary>
    /// Determines the culture information for a request via the value of the start of a url.
    /// Needs to be used in Startup.ConfigureServices().
    /// </summary>
    public class UrlCultureProvider : RequestCultureProvider
    {       
        /// <summary>
        /// The default culture if none is found.
        /// </summary>
        public string DefaultCulture { get; set; } = LocalizationRouteDataHandler.DefaultCulture;

        /// <summary>
        /// The supported cultures from url.
        /// </summary>
        public IList<CultureInfo> SupportedCultures {get; set;}

        public UrlCultureProvider(IList<CultureInfo> supportedCultures)
        {
            SupportedCultures = supportedCultures;
        }              

        public override Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            string url = httpContext.Request.Path.Value;
            // Example: /fi
            if (url.Length >= 3)
            {   
                if (url.Length >= 4)
                {
                    // If the 4th character isn't a /
                    // Example: /Home , then return default culture                    
                    if (url[3] != '/' )
                    {
                        return Task.FromResult(new ProviderCultureResult(DefaultCulture));
                    }
                }

                // Get the /value/ value
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

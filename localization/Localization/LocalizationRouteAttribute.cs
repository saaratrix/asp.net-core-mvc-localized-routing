using System;
using System.Collections.Generic;
using System.Globalization;

namespace localization.Localization
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]   
    public class LocalizationRouteAttribute : Attribute
    {
        /// <summary>
        /// The character to replace whitespace with in the input route, like 
        /// "batman rocks" => "batman_rocks" as route
        /// </summary>
        private const char WhiteSpaceReplacement = '_';

        /// <summary>
        /// The characters to split a route on to generate a more link friendly url.
        /// For example some_route => Some Route
        /// </summary>
        private static readonly char[] RouteToLinkSplitCharacters = new char[] { '_', '-' };

        /// <summary>
        /// The culture string representation, en, fi, sv e.t.c.!
        /// </summary>
        public string Culture { get; set; }
        /// <summary>
        /// The route, no need for /.
        /// It is case sensitive.
        /// Meaning "roUTe" would create the route "/roUTe"
        /// </summary>
        public string Route { get; set; }
        /// <summary>
        /// What the link text is for a Html.ActionLink
        /// </summary>
        public string Link { get; set; }

        public LocalizationRouteAttribute()
        {                        
        }

        public LocalizationRouteAttribute(string culture)
            :this(culture, "", null)
        {

        }

        /// <summary>
        /// Attribute used by LocalizationConvention to create all the routes.
        /// Defaults Link to null
        /// </summary>
        /// <param name="culture"></param>
        /// <param name="route"></param>
        public LocalizationRouteAttribute(string culture, string route)
           : this(culture, route, null)
        {

        }

        /// <summary>
        /// Attribute used by LocalizationConvention to create all the routes.
        /// </summary>
        /// <param name="culture"></param>
        /// <param name="route"></param>
        /// <param name="link">If not defined the value is route with first letter capitalized</param>
        public LocalizationRouteAttribute(string culture, string route, string link)
        {
            Culture = culture;
            Route = route;
            // Replace all the spaces with the whitespace replacement character
            Route = Route.Replace(' ', WhiteSpaceReplacement);            
            
            // If the link is null then set it to the route
            if (string.IsNullOrEmpty(link))
            {   
                Link = ConvertRouteToLink(Culture, Route);         
            }
            else
            {
                Link = link;                
            }                     
        }

        /// <summary>
        /// Convert a route value to a link friendly value.
        /// Example of "some_route" converts to "Some Route"
        /// </summary>
        /// <param name="route"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static string ConvertRouteToLink(string culture, string route)
        {
            CultureInfo cultureInfo = new CultureInfo(culture, false);            

            string[] routeParts = route.Split(RouteToLinkSplitCharacters);
            List<string> parsedParts = new List<string>();

            for (int i = 0; i < routeParts.Length; i++)
            {
                string routePart = routeParts[i];
                if (routePart.Length == 0)
                {
                    continue;
                }                
                // The reason for doing this instead of TextInfo.ToTitleCase()
                // Is because ToTitleCase would convert batMAN to Batman instead of BatMAN.

                // Uppercase first letter
                char letter = Char.ToUpper(routePart[0], cultureInfo);
                // Then add the rest!
                routePart = routePart.Length > 1 ? routePart.Substring(1) : "";

                parsedParts.Add(letter + routePart);                
            }           

            return String.Join(" ", parsedParts);
        }        
    }
}
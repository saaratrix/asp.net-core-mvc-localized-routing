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

        public LocalizationRouteAttribute()
        {                        
        }

        public LocalizationRouteAttribute(string culture)
            :this(culture, "")
        {

        }
        

        /// <summary>
        /// Attribute used by LocalizationConvention to create all the routes.
        /// </summary>
        /// <param name="culture"></param>
        /// <param name="route"></param>
        /// <param name="link">If not defined the value is route with first letter capitalized</param>
        public LocalizationRouteAttribute(string culture, string route)
        {
            Culture = culture;
            Route = route;
            // Replace all the spaces with the whitespace replacement character
            Route = Route.Replace(' ', WhiteSpaceReplacement);     
        }
    }
}
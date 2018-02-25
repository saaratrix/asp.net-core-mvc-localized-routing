using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace localization.Localization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]   
    public class LocalizedRouteAttribute : Attribute
    {
        /// <summary>
        /// The character to replace whitespace with in the input route, like 
        /// "batman rocks" => "batman_rocks" as route
        /// </summary>
        private const char WhiteSpaceReplacement = '_';

        /// <summary>
        /// The culture string representation, en, en-Us e.t.c.!
        /// </summary>
        public string Culture { get; set; }
        /// <summary>
        /// The route, no need for / 
        /// </summary>
        public string Route { get; set; }
        /// <summary>
        /// What the link text is for a Html.ActionLink
        /// </summary>
        public string Link { get; set; }

        public LocalizedRouteAttribute()
        {                        
        }

        public LocalizedRouteAttribute(string a_culture)
            :this(a_culture, "", null)
        {

        }

        /// <summary>
        /// Attribute used by LocalizationConvention to create all the routes.
        /// Defaults Link to null
        /// </summary>
        /// <param name="a_culture"></param>
        /// <param name="a_route"></param>
        public LocalizedRouteAttribute(string a_culture, string a_route)
           : this(a_culture, a_route, null)
        {

        }

        /// <summary>
        /// Attribute used by LocalizationConvention to create all the routes.
        /// </summary>
        /// <param name="a_culture"></param>
        /// <param name="a_route"></param>
        /// <param name="a_link">If not defined the value is route with first letter capitalized</param>
        public LocalizedRouteAttribute(string a_culture, string a_route, string a_link)
        {
            Culture = a_culture;
            Route = a_route;
            // Replace all the spaces with the whitespace replacement character
            Route = Route.Replace(' ', WhiteSpaceReplacement);
                
            // If the link is null then set it to the route
            if (a_link == null)
            {
                CultureInfo cultureInfo = new CultureInfo(Culture, false);
                TextInfo textInfo = cultureInfo.TextInfo;
                // Do the opposite that route does,  replace the whitespace replacement characters with whitespace!
                Link = textInfo.ToTitleCase(Route.Replace(WhiteSpaceReplacement, ' '));                
            }
            else
            {
                Link = a_link;                
            }
                     
        }
    }
}

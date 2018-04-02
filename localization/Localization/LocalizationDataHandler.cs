using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace localization.Localization
{
    /// <summary>
    /// The class that has all the localization data like routes, supported cultures, default culture.
    /// Set this either in the class itself or Startup.cs
    /// </summary>
    public static class LocalizationDataHandler
    {
        /// <summary>
        /// The default culture
        /// </summary>
        public static string DefaultCulture { get; set; }
        /// <summary>
        /// The list of all supported cultures
        /// </summary>
        public static List<string> SupportedCultures { get; set; }

        public static string DefaultController { get; set; } = "Home";
        public static string DefaultAction { get; set; } = "Index";

        // This is for unit testing so that the dictionary can be reset between tests.
        // Otherwise the property would be an auto property!
        private static ConcurrentDictionary<string, CultureControllerData> _controllerRoutes = new ConcurrentDictionary<string, CultureControllerData>();
        /// <summary>
        /// All the routes and their cultural representation, example:
        /// home => names { home, koti },  actions { index, about }
        ///     action about => names { about, miestä }
        /// </summary>
        // Will never get modified after initialization is done.
        private static ConcurrentDictionary<string, CultureControllerData> ControllerRoutes
        {
            get
            {
                return _controllerRoutes;
            }
        } 

        public static void AddControllerData(string a_controller, string a_culture, string a_route)
        {
            string controllerKey = a_controller.ToLower();
            
            // If the controller doesn't exist, create it!            
            if (!ControllerRoutes.ContainsKey(controllerKey))
            {                
                ControllerRoutes.TryAdd(controllerKey, new CultureControllerData());
            }            
            ControllerRoutes[controllerKey].Names.TryAdd(a_culture, a_route);
        }

        /// <summary>
        /// Add the action data.  Will throw exception if the controller doesn't exist
        /// </summary>
        /// <param name="a_controller"></param>
        /// <param name="a_action"></param>
        /// <param name="a_culture"></param>
        /// <param name="a_route"></param>
        /// <param name="a_linkName"></param>
        public static void AddActionData(string a_controller, string a_action, string a_culture, string a_route, string a_linkName, List<string> a_routeParameters)
        {            
            string actionKey = a_action.ToLower();           

            CultureControllerData controllerData = ControllerRoutes[a_controller.ToLower()];
            if (!controllerData.Actions.ContainsKey(actionKey))
            {
                controllerData.Actions.TryAdd(actionKey, new CultureActionData(a_routeParameters));
            }

            controllerData.Actions[actionKey].UrlData.TryAdd(a_culture, new CultureUrlData(a_route, a_linkName));
        }
        
        /// <summary>
        /// Get the url for a controller & action based on culture
        /// </summary>
        /// <param name="a_controller"></param>
        /// <param name="a_action"></param>
        /// <param name="a_culture"></param>
        /// <returns></returns>
        public static LocalizedUrlResult GetUrl(string a_controller, string a_action, string a_culture)
        {
            LocalizedUrlResult result = new LocalizedUrlResult();
            string controllerKey = a_controller.ToLower();
            string actionKey = a_action.ToLower();

            if (ControllerRoutes.ContainsKey(controllerKey))
            {
                CultureControllerData controllerData = ControllerRoutes[controllerKey];

                if (controllerData.Actions.ContainsKey(actionKey))
                {
                    // Ok now we have the controller name and action data name!
                    CultureActionData actionData = controllerData.Actions[actionKey];
                    bool removeController = false;

                    // Check if culture is default culture
                    if (a_culture == DefaultCulture)
                    {
                        if (a_action.Equals(DefaultAction, StringComparison.CurrentCultureIgnoreCase))
                        {
                            a_action = "";

                            if (a_controller.Equals(DefaultController, StringComparison.CurrentCultureIgnoreCase))
                            {                                
                                removeController = true;
                            }
                        }

                        if (!removeController)
                        {
                            a_controller += "/";
                        } 
                        else
                        {
                            a_controller = "";
                        }                        

                        result.Url = "/" + a_controller + a_action;
                        result.LinkName = a_action;  
                    }
                    // If the culture isn't default culture
                    else
                    {   
                        CultureUrlData linkData = actionData.UrlData.ContainsKey(a_culture) ? actionData.UrlData[a_culture] : actionData.UrlData[DefaultCulture];
                        // If the controller doesn't exist add the culture prefix to it stays in the culture prefix space.
                        string controllerName = controllerData.Names.ContainsKey(a_culture) ? controllerData.Names[a_culture] : a_culture + "/" + a_controller;
                        string actionName = linkData.Route;                        
                        // If the controllerName isn't the default one add a /
                        // If not it would be for example /fi/accountLogin    instead of /fi/account/login
                        if (!a_controller.Equals(DefaultController, StringComparison.CurrentCultureIgnoreCase) || !actionName.Equals(DefaultAction, StringComparison.CurrentCultureIgnoreCase))
                        {
                            // So it becomes => /culture/controller/                             
                            controllerName += "/";                                                       
                        }                       

                        result.Url = "/" + controllerName + actionName;
                        result.LinkName = linkData.Link;
                    }                    
                }
                // Return just the controller?
                else
                {
                    
                }
            }

            return result;
        }

        /// <summary>
        /// For example: /{controller}/{action}/{param1}/{param2}
        /// Then it will return values of {param1}/{param2} in the right order based off routeValues
        /// </summary>
        /// <param name="controllerName"></param>
        /// <param name="actionName"></param>
        /// <param name="routeValues"></param>
        /// <returns></returns>
        public static string GetOrderedParameters(string a_controller, string a_action, Dictionary<string, string> a_routeValues)
        {
            string controllerKey = a_controller.ToLower();
            string actionKey = a_action.ToLower();

            string result = "";

            if (ControllerRoutes.ContainsKey(controllerKey))
            {
                CultureControllerData controllerData = ControllerRoutes[controllerKey];

                if (controllerData.Actions.ContainsKey(actionKey))
                {
                    CultureActionData actionData = controllerData.Actions[actionKey];
                    if (actionData.ParametersData != null)
                    {
                        foreach (string parameter in actionData.ParametersData)
                        {
                            if (a_routeValues.ContainsKey(parameter))
                            {                                
                                result += "/" + a_routeValues[parameter];                                
                            }
                            // Otherwise we found parameter data that isn't accounted for.                           
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Get the culture from an url by checking if the href starts with /culture/
        /// So there is possibility of a collision if a controller is called a culture!  
        /// So don't name them cultures!!
        /// Note: CultureInfo.CurrentCulture is a good way of getting the culture for the current request.
        /// </summary>
        /// <param name="a_url"></param>
        /// <returns></returns>
        public static string GetCultureFromUrl(string a_url)
        {            
            foreach(string culture in SupportedCultures)
            {
                if (a_url.StartsWith("/" + culture + "/"))
                {
                    return culture;                    
                }
            }
            return DefaultCulture;
        }
    }
}

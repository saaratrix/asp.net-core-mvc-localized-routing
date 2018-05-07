using localization.Localization.CultureRouteData;
using System;
using System.Collections.Generic;

namespace localization.Localization
{
    /// <summary>
    /// LocalizationRouteDataHandler handles storing all the localized route data and generating the localized urls.
    /// It also has the supported cultures and default culture.
    /// Set the supported cultures and default culture either in the class itself or in Startup.cs
    /// </summary>
    public static class LocalizationRouteDataHandler
    {
        /// <summary>
        /// The default culture.
        /// </summary>
        public static string DefaultCulture { get; set; }
        /// <summary>
        /// The dictionary of all supported cultures.
        /// Key = Culture Name
        /// Value = Display Name
        /// Example: en, English
        /// </summary>
        public static Dictionary<string, string> SupportedCultures { get; set; }        

        public static string DefaultController { get; set; } = "Home";
        public static string DefaultAction { get; set; } = "Index";

        // Example for controller Home:
        // Home { 
        //      Names = [ home, fi/koti ],  
        //      Actions = { 
        //          About = {
        //              UrlData = {
        //                  en = { Url = about, Link = About },
        //                  fi = { Url = meistä, Link = Meistä }
        //              },
        //              ParametersData = []
        //          }
        //      }
        // }     
        /// <summary>
        /// All the routes and their cultural representation.        
        /// </summary>
        // Will never get modified after initialization is done so Dictionary should be thread safe.        
        public static Dictionary<string, CultureControllerRouteData> ControllerRoutes { get; } = new Dictionary<string, CultureControllerRouteData>();

        /// <summary>
        /// Add Controller Route data 
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="culture"></param>
        /// <param name="route"></param>
        public static void AddControllerRouteData(string controller, string culture, string route)
        {  
            string controllerKey = controller.ToLower();
            
            // If the controller doesn't exist, create it!            
            if (!ControllerRoutes.ContainsKey(controllerKey))
            {                
                ControllerRoutes.TryAdd(controllerKey, new CultureControllerRouteData());
            }            
            ControllerRoutes[controllerKey].Names.TryAdd(culture, route);
        }

        /// <summary>
        /// Add the action data.  
        /// Will throw exception if the controller doesn't exist
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="culture"></param>
        /// <param name="route"></param>
        /// <param name="linkName"></param>
        public static void AddActionRouteData(string controller, string action, string culture, string route, string linkName, List<string> routeParameters)
        {            
            string actionKey = action.ToLower();           

            CultureControllerRouteData controllerData = ControllerRoutes[controller.ToLower()];
            if (!controllerData.Actions.ContainsKey(actionKey))
            {
                controllerData.Actions.TryAdd(actionKey, new CultureActionRouteData(routeParameters));
            }           

            controllerData.Actions[actionKey].UrlData.TryAdd(culture, new CultureUrlRouteData(route, linkName));
        }
        
        /// <summary>
        /// Get the url for a controller & action based on culture
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static LocalizationUrlResult GetUrl(string controller, string action, string culture)
        {
            LocalizationUrlResult result = new LocalizationUrlResult();
            string controllerKey = controller.ToLower();
            string actionKey = action.ToLower();

            if (ControllerRoutes.ContainsKey(controllerKey))
            {
                CultureControllerRouteData controllerData = ControllerRoutes[controllerKey];

                if (controllerData.Actions.ContainsKey(actionKey))
                {
                    bool isDefaultController = controller.Equals(DefaultController, StringComparison.OrdinalIgnoreCase);
                    bool isDefaultAction = action.Equals(DefaultAction, StringComparison.OrdinalIgnoreCase);
                    bool isDefaultCulture = culture == DefaultCulture;

                    // Ok now we have the controller name and action data name!
                    CultureActionRouteData actionData = controllerData.Actions[actionKey];
                    CultureUrlRouteData linkData = actionData.UrlData.ContainsKey(culture) ? actionData.UrlData[culture] : actionData.UrlData[DefaultCulture];
                    
                    string controllerUrl = controllerData.Names.ContainsKey(culture) ? controllerData.Names[culture] : "";
                    // The actionUrl is "" for default action
                    string actionUrl = linkData.Route;
                    // TODO: Evaluate if default culture also should use the linkData 
                    // The cms-keep-link attribute would be used otherwise.
                    string linkName = isDefaultCulture ? "" : linkData.Link;

                    // If default controller & action then the url should be
                    // Default: /
                    // Others:  /culture
                    if (isDefaultController && isDefaultAction)
                    {
                        controllerUrl = isDefaultCulture ? "" : culture;
                    }
                    else
                    {
                        // Check if culture is default culture
                        if (!isDefaultCulture)
                        {                            
                            // If the controller doesn't exist add the culture as prefix to the controller name
                            if (!controllerData.Names.ContainsKey(culture))
                            {
                                controllerUrl = culture + "/" + controller;
                            }                            
                        }
                    }                    

                    // So that the url is {controller}/{action} instead of {controller}{action}
                    if (!isDefaultAction)
                    {                                      
                        controllerUrl += "/";
                    }

                    result.Url = "/" + controllerUrl + actionUrl;
                    result.LinkName = linkName;
                }
                // A controller was found with an incorrect action.                 
                else
                {
                    // Return just the controller url? 
                    // For now explicitly throw an exception!
                    throw new ArgumentException("A controller was found without a valid action. Check that the action key is correct.");
                }
            }
            // No controller was found
            else
            {
                // As for the invalid argument more gracefully throw the error?
                throw new ArgumentException("No controller was found with that name. Check that the controller key is correct.");
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
        public static string GetOrderedParameters(string controller, string action, Dictionary<string, string> routeValues)
        {
            string controllerKey = controller.ToLower();
            string actionKey = action.ToLower();

            string result = "";

            if (ControllerRoutes.ContainsKey(controllerKey))
            {
                CultureControllerRouteData controllerData = ControllerRoutes[controllerKey];

                if (controllerData.Actions.ContainsKey(actionKey))
                {
                    CultureActionRouteData actionData = controllerData.Actions[actionKey];
                    if (actionData.ParametersData != null)
                    {
                        foreach (string parameter in actionData.ParametersData)
                        {
                            if (routeValues.ContainsKey(parameter))
                            {                                
                                result += "/" + routeValues[parameter];                                
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
        /// Note: CultureInfo.CurrentCulture.Name is a good way of getting the culture for the current request.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetCultureFromUrl(string url)
        {            
            foreach(var kvp in SupportedCultures)
            {
                if (url.StartsWith("/" + kvp.Key + "/"))
                {
                    return kvp.Key;                    
                }
            }
            return DefaultCulture;
        }
    }
}

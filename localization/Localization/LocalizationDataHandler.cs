using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

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
        public static Dictionary<string, string> SupportedCultures { get; set; }
        

        public static string DefaultController { get; set; } = "Home";
        public static string DefaultAction { get; set; } = "Index";

        // This is for unit testing so that the dictionary can be reset between tests.
        // Otherwise the property would be an auto property!
        private static ConcurrentDictionary<string, CultureControllerData> _controllerRoutes = new ConcurrentDictionary<string, CultureControllerData>();
        /// <summary>
        /// All the routes and their cultural representation, example:
        /// home => names { home, koti },  actions { index, about }
        ///     action about => names { about, meistä }
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

            string controllerUrl;
            string actionUrl;

            if (ControllerRoutes.ContainsKey(controllerKey))
            {
                CultureControllerData controllerData = ControllerRoutes[controllerKey];

                if (controllerData.Actions.ContainsKey(actionKey))
                {
                    bool isDefaultController = a_controller.Equals(DefaultController, StringComparison.OrdinalIgnoreCase);
                    bool isDefaultAction = a_action.Equals(DefaultAction, StringComparison.OrdinalIgnoreCase);

                    // Ok now we have the controller name and action data name!
                    CultureActionData actionData = controllerData.Actions[actionKey];                    

                    // Check if culture is default culture
                    if (a_culture == DefaultCulture)
                    {
                        if (isDefaultAction && isDefaultController)
                        {                            
                            result.Url = "/";                            
                        }
                        else
                        {
                            CultureUrlData linkData = actionData.UrlData[DefaultCulture];

                            if (isDefaultAction)
                            {
                                actionUrl = "";
                                controllerUrl = controllerData.Names[DefaultCulture];
                            }
                            else
                            {
                                // Add a / at the end of the controller url
                                controllerUrl = controllerData.Names[DefaultCulture] + "/";
                                actionUrl = linkData.Route;
                            }

                            result.Url = "/" + controllerUrl + actionUrl;
                        }
                        // So linkName isn't null
                        result.LinkName = "";
                        // We could set the LinkName however by not setting it the AnchorLinkTagHelper will keep the initial value.
                        // Which I believe offers a higher degree of customization.
                        // If there is a cms-link-override="false" then it might be worth to add override by default even for default culture. 
                        //result.LinkName = actionData.UrlData[DefaultCulture].Link;  
                    }
                    // If the culture isn't default culture
                    else
                    {     
                        CultureUrlData linkData = actionData.UrlData.ContainsKey(a_culture) ? actionData.UrlData[a_culture] : actionData.UrlData[DefaultCulture];

                        if (isDefaultController && isDefaultAction)
                        {
                            result.Url = "/" + a_culture;
                        }
                        else
                        {                            
                            // If the controller doesn't exist add the culture prefix to it stays in the culture prefix space.
                            controllerUrl = controllerData.Names.ContainsKey(a_culture) ? controllerData.Names[a_culture] : a_culture + "/" + a_controller;
                            actionUrl = linkData.Route;
                            // If the controllerName isn't the default one add a /
                            // If not it would be for example /fi/accountLogin    instead of /fi/account/login
                            if (!isDefaultAction)
                            {
                                // So it becomes => /culture/controller/                             
                                controllerUrl += "/";
                            }

                            result.Url = "/" + controllerUrl + actionUrl;
                        }
                        
                        result.LinkName = linkData.Link;
                    }                    
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
        /// Note: CultureInfo.CurrentCulture.Name is a good way of getting the culture for the current request.
        /// </summary>
        /// <param name="a_url"></param>
        /// <returns></returns>
        public static string GetCultureFromUrl(string a_url)
        {            
            foreach(var kvp in SupportedCultures)
            {
                if (a_url.StartsWith("/" + kvp.Key + "/"))
                {
                    return kvp.Key;                    
                }
            }
            return DefaultCulture;
        }
    }
}

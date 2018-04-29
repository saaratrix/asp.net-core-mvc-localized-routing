using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace localization.Localization
{
    /// <summary>
    /// Sets up all routes based on the [LocalizationRoute] attributes for all the controllers and controllers' actions.
    /// </summary>
    public class LocalizationRouteConvention : IApplicationModelConvention
    {
        public LocalizationRouteConvention()
        {            
        }

        public void Apply(ApplicationModel applicationModel)
        {
            foreach (ControllerModel controller in applicationModel.Controllers)
            {
                // If the controllerName is the same as the base controller for localization go next since it doesn't have any actions
                // Since it's a controller it ends up in applicationModel.Controllers which is why we just continue;
                if (controller.ControllerName == "Localization")
                {
                    continue;
                }

                // Do the controller            
                AddControllerRoutes(controller);
                // Do the actions!
                AddActionRoutes(controller);                       
            }
        }

        /// <summary>
        /// Add an AttributeRouteModel to a SelectorModel list.
        /// It also tries to set the first entry of the list if the AttributeRouteModel is null there.
        /// </summary>
        /// <param name="selectorModels"></param>
        /// <param name="attributeRouteModel"></param>
        public void AddAttributeRouteModel(IList<SelectorModel> selectorModels, AttributeRouteModel attributeRouteModel)
        {
            // Override what seems to be default SelectorModel
            if (selectorModels.Count == 1 && selectorModels[0].AttributeRouteModel == null)
            {
                selectorModels[0].AttributeRouteModel = attributeRouteModel;
            }
            else
            {
                selectorModels.Add(new SelectorModel
                {
                    AttributeRouteModel = attributeRouteModel
                });
            }            
        }

        /// <summary>
        /// Set up the localized routes for the controller model.
        /// It uses the [LocalizationRoute] attributes and if no attributes are found it uses culture/controllerName.        
        /// </summary>
        /// <param name="controllerModel"></param>
        public void AddControllerRoutes(ControllerModel controllerModel)
        {            
            string controllerName = controllerModel.ControllerName;

            // If the controller is the default controller then add the "/", "/culture" routes.
            // If we don't do this then "/" or "/culture" would throw 404.
            // Instead /Default would be the only way to access the default controller.
            if (controllerName.Equals(LocalizationRouteDataHandler.DefaultController, StringComparison.Ordinal))
            {
                // Set up the "/", "/culture1", "/culture2" route templates for all supported cultures.                
                foreach(var kvp in LocalizationRouteDataHandler.SupportedCultures)
                {
                    string template = LocalizationRouteDataHandler.DefaultCulture == kvp.Key ? "" : kvp.Key;

                    AttributeRouteModel defaultRoute = new AttributeRouteModel();
                    defaultRoute.Template = template;
                    AddAttributeRouteModel(controllerModel.Selectors, defaultRoute);
                }
            }
            
            LocalizationRouteDataHandler.AddControllerRouteData(controllerName, LocalizationRouteDataHandler.DefaultCulture, controllerName);

            // Create the route for the controller to /Default.
            // Since DefaultController also should be reachable by /Default.
            // This adds the /Default route template to controllerModel.Selectors so it is reachable by both / and /Default.          
            AttributeRouteModel controllerRoute = new AttributeRouteModel();
            controllerRoute.Template = controllerModel.ControllerName;                    
            AddAttributeRouteModel(controllerModel.Selectors, controllerRoute);

            AddControllerLocalizedRoutes(controllerModel);
        }
        
        /// <summary>
        /// Add the localized routes for the controller model
        /// </summary>
        /// <param name="controllerModel"></param>
        public void AddControllerLocalizedRoutes(ControllerModel controllerModel)
        {
            // Get all the [LocalizationRoute] Attributes from the controller
            var controllerLocalizations = controllerModel.Attributes.OfType<LocalizationRouteAttribute>().ToList();            
            string controllerName = controllerModel.ControllerName;

            // Keep track of which cultures did not have a [LocalizationRoute] attribute so they can have one added programmatically.
            HashSet<string> notFoundCultures = LocalizationRouteDataHandler.SupportedCultures.Select(kvp => kvp.Key).ToHashSet();
            notFoundCultures.Remove(LocalizationRouteDataHandler.DefaultCulture);

            // Loop over all [LocalizationRoute] attributes
            foreach (LocalizationRouteAttribute attribute in controllerLocalizations)
            {
                string template = attribute.Culture;
                // If the attributeRoute isn't empty then we use the route name
                if (!String.IsNullOrEmpty(attribute.Route))
                {
                    // Add / if the route doesn't start with /
                    // Otherwise the template would be {culture}{route}. 
                    // Instead of {culture}/{route}
                    if (!attribute.Route.StartsWith("/"))
                    {
                        template += "/";
                    }
                    template += attribute.Route;
                }
                // If attribute.Route is empty then we use the controller name so it's not an empty name.
                else
                {
                    template += "/" + controllerName;
                }
                
                AttributeRouteModel localRoute = new AttributeRouteModel();
                localRoute.Template = template;
                AddAttributeRouteModel(controllerModel.Selectors, localRoute);

                // Add the route to the localizations dictionary
                LocalizationRouteDataHandler.AddControllerRouteData(controllerName, attribute.Culture, template);
                // Remove it from the not Found Cultures list since the culture had a [LocalizationRoute] attribute.                
                notFoundCultures.Remove(attribute.Culture);
            }

            // Add the remaining cultures that didn't have [LocalizationRoute] attributes.
            foreach (string culture in notFoundCultures)
            {
                string template = culture;
                if (!controllerName.Equals(LocalizationRouteDataHandler.DefaultController, StringComparison.CurrentCultureIgnoreCase))
                {
                    template += "/" + controllerName;
                }

                AttributeRouteModel localRoute = new AttributeRouteModel();
                localRoute.Template = template;
                AddAttributeRouteModel(controllerModel.Selectors, localRoute);

                LocalizationRouteDataHandler.AddControllerRouteData(controllerName, culture, template);
            }
        }

        /// <summary>
        /// Adds the localized routes for a controller
        /// </summary>
        /// <param name="controllerModel"></param>
        public void AddActionRoutes(ControllerModel controllerModel)
        {            
            string controllerName = controllerModel.ControllerName;
            // All the new localized actions to add to the controllerModel after all calculations.
            List<ActionModel> newActions = new List<ActionModel>();
            // Loop through all the actions to add their routes and also get the localized actions
            foreach (ActionModel action in controllerModel.Actions)
            {                
                string actionName = action.ActionName;
                // If any parameters are needed such as /{index}
                string parameterTemplate = "";

                SelectorModel defaultSelectionModel = action.Selectors.FirstOrDefault(x => x.AttributeRouteModel != null);

                List<string> sortedRouteParameters = new List<string>();

                // If there is no [Route()] Attribute then create one for the route.
                if (defaultSelectionModel == null || defaultSelectionModel.AttributeRouteModel == null)
                {
                    AttributeRouteModel attributeRouteModel = new AttributeRouteModel();
                    
                    if (action.Parameters.Count > 0)
                    {
                        foreach (ParameterModel parameter in action.Parameters)
                        {                            
                            sortedRouteParameters.Add(parameter.ParameterName.ToLower());
                            // TODO: ParseParameterTemplate? I think you can't skip the [Route] attribute if you want parameters.
                        }
                    }

                    if (!action.ActionName.Equals(LocalizationRouteDataHandler.DefaultAction, StringComparison.Ordinal))
                    {
                        attributeRouteModel.Template = actionName;
                        // Add the action name as it is eg: about will be about!
                        LocalizationRouteDataHandler.AddActionRouteData(controllerName, actionName, LocalizationRouteDataHandler.DefaultCulture, actionName, actionName, sortedRouteParameters);
                    }
                    else
                    {
                        // For DefaultAction we don't want to have a template.
                        // Because the default action is reachable with /Controller as the url.
                        attributeRouteModel.Template = "";                        
                        LocalizationRouteDataHandler.AddActionRouteData(controllerName, actionName, LocalizationRouteDataHandler.DefaultCulture, "", controllerName, sortedRouteParameters);
                    }

                    AddAttributeRouteModel(action.Selectors, attributeRouteModel);
                }
                // If a route already existed then check for parameter arguments to add to the cultural routes
                else
                {                    
                    string template = defaultSelectionModel.AttributeRouteModel.Template;

                    parameterTemplate = ParseParameterTemplate(template, sortedRouteParameters);

                    LocalizationRouteDataHandler.AddActionRouteData(controllerName, actionName, LocalizationRouteDataHandler.DefaultCulture, actionName, actionName, sortedRouteParameters);
                }

                var localizedActions = CreateLocalizedActionRoutes(controllerModel, action, parameterTemplate, sortedRouteParameters);
                newActions.AddRange(localizedActions);
            } // End foreach controllerModel.Actions

            // Now add all the new actions to the controller
            foreach (ActionModel action in newActions)
            {
                controllerModel.Actions.Add(action);
            }            
        }

        /// <summary>
        /// Create the new list of action models for each localized route.
        /// </summary>
        /// <param name="controllerModel"></param>
        /// <param name="actionModel"></param>
        /// <param name="parameterTemplate"></param>
        /// <param name="sortedRouteParameters"></param>
        /// <returns></returns>
        public List<ActionModel> CreateLocalizedActionRoutes(ControllerModel controllerModel, ActionModel actionModel, string parameterTemplate, List<string> sortedRouteParameters)
        {
            string controllerName = controllerModel.ControllerName;
            string actionName = actionModel.ActionName;
            var actionLocalizationsAttributes = actionModel.Attributes.OfType<LocalizationRouteAttribute>().ToList();

            List<ActionModel> localizedActions = new List<ActionModel>();

            // For default actions we need to check if the [LocalizationRoute] Attribute exists or not.
            // This is so we can name the default action after the controller
            // For example otherwise HomeController for finnish Culture would be Home instead of Koti.
            if (actionName.Equals(LocalizationRouteDataHandler.DefaultAction, StringComparison.OrdinalIgnoreCase))
            {
                HashSet<string> cultures = LocalizationRouteDataHandler.SupportedCultures.Select(kvp => kvp.Key).ToHashSet();
                
                cultures.Remove(LocalizationRouteDataHandler.DefaultCulture);       
                cultures.RemoveWhere(x => actionLocalizationsAttributes.FirstOrDefault(attr => attr.Culture == x) != null);
                
                // Iterate over all controllers that had a [LocalizationRoute]
                foreach (string culture in cultures)
                {
                    // The localized controller name is the link for the index action
                    string localizedControllerName = GetLocalizedControllerName(controllerModel, culture);
                    if (!localizedControllerName.Equals(controllerName, StringComparison.OrdinalIgnoreCase))
                    {                        
                        LocalizationRouteDataHandler.AddActionRouteData(controllerName, actionName, culture, "", localizedControllerName, sortedRouteParameters);
                    }
                }
            }
            
            foreach (LocalizationRouteAttribute attribute in actionLocalizationsAttributes)
            {
                string route = attribute.Route + parameterTemplate;
                // This copies all existing Attributes on the ActionModel,  [Route] [HttpGet] e.t.c.
                // Source file: https://github.com/aspnet/Mvc/blob/dev/src/Microsoft.AspNetCore.Mvc.Core/ApplicationModels/ActionModel.cs
                ActionModel newLocalizedActionModel = new ActionModel(actionModel);

                // Clear the Selectors or it will have shared selector data from default route.
                // This however clears the ActionConstraints like [HttpGet] and [HttpPost]
                newLocalizedActionModel.Selectors.Clear();
                AttributeRouteModel newLocalizedAttributeRouteModel = new AttributeRouteModel();                 
                newLocalizedAttributeRouteModel.Template = route;
                // Add the new actionModel for adding to controller later
                localizedActions.Add(newLocalizedActionModel);

                AddAttributeRouteModel(newLocalizedActionModel.Selectors, newLocalizedAttributeRouteModel);
                // Bug mentioned by anonymous through a comment on blog.
                // This is where the [HttpGet], [HttpPost] constraints are added back after being cleared earlier.                                 
                foreach (var actionConstraint in actionModel.Selectors.Where(x => x.ActionConstraints.Count > 0).SelectMany(x => x.ActionConstraints))
                {
                    newLocalizedActionModel.Selectors[0].ActionConstraints.Add(actionConstraint);
                }

                string linkName = attribute.Link;
                // If the action is default (Index) and there is no LinkName set
                // Then the linkName should be the controllers name 
                if (actionName.Equals(LocalizationRouteDataHandler.DefaultAction, StringComparison.OrdinalIgnoreCase) && !String.IsNullOrEmpty(linkName))
                {
                    linkName = GetLocalizedControllerName(controllerModel, attribute.Culture);
                }

                // Add the localized route for the action
                // Example of final route:  "fi/koti" + "/" + "ota_yhteyttä"
                LocalizationRouteDataHandler.AddActionRouteData(controllerName, actionName, attribute.Culture, attribute.Route, linkName, sortedRouteParameters);
            }

            return localizedActions;
        }

        /// <summary>
        /// Get the localized controller name for a specific culture.
        /// If there is no [LocalizationRoute] attribute the controllerMode.ControllerName is returned.
        /// </summary>
        /// <param name="controllerModel"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public string GetLocalizedControllerName(ControllerModel controllerModel, string culture)
        {
            var localizationRouteAttributes = controllerModel.Attributes.OfType<LocalizationRouteAttribute>().ToList();

            string name = controllerModel.ControllerName;

            foreach (LocalizationRouteAttribute attribute in localizationRouteAttributes)
            {
                if (attribute.Culture == culture)
                {
                    if (!String.IsNullOrWhiteSpace(attribute.Link))
                    {
                        name = attribute.Link;
                    }
                    // If attribute.Link isn't set
                    // Then we extract the Link name from the Route
                    else if (!String.IsNullOrWhiteSpace(attribute.Route))
                    {
                        name = LocalizationRouteAttribute.ConvertRouteToLink(culture, attribute.Route);
                    }

                    break;
                }
            }

            return name;
        }

        /// <summary>
        /// Parses the input template and returns a parsed template that only contains the {parameter} values.
        /// It also adds parameters it encounters to the sortedRouteParameters list.
        /// </summary>
        /// <param name="template"></param>
        /// <param name="sortedRouteParameters"></param>
        /// <returns></returns>
        public string ParseParameterTemplate(string template, List<string> sortedRouteParameters)
        {
            string parameterTemplate = "";
            // Check if the route has parameters
            string[] actionComponents = template.Split('/');

            for (int i = 0; i < actionComponents.Length; i++)
            {
                string actionComponent = actionComponents[i];
                // In case of "/action/"
                if (actionComponent.Length == 0)
                {
                    continue;
                }

                // Check if first character starts with {
                if (actionComponent[0] == '{')
                {
                    // Extract the name
                    // Example of an part: { moo = 5 }
                    string name = GetParameterName(actionComponent);

                    // TODO: Evaluate if continue should be used for action or controller
                    if (name != "action" && name != "controller")
                    {
                        sortedRouteParameters.Add(name);
                    }

                    parameterTemplate += "/" + actionComponents[i];
                }
            }

            return parameterTemplate;
        }

        // More information: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/routing#route-constraint-reference
        /// <summary>
        /// Gets the parameter name from a {parameter}
        /// </summary>
        /// <param name="actionComponent"></param>
        /// <returns></returns>
        public string GetParameterName(string actionComponent)
        {
            // Example: { param:int = 1 }
            string name = actionComponent.Split(new char[] { '=', ':' }).First();
            // Remove whitespace since that's invalid!
            // Also remove the { character with SubString(1)
            name = Regex.Replace(name, @"\s+", "").Substring(1);
            // If last character is } which it can be if it wasn't split on '=' or ':'
            if (name[name.Length - 1] == '}')
            {
                // Then remove that!
                name = name.Remove(name.Length - 1);
            }

            return name.ToLower();
        }
    }
}

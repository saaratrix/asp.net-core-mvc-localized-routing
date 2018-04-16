using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace localization.Localization
{
    public class LocalizedRouteConvention : IApplicationModelConvention
    {
        public string DefaultCulture { get; set; }        

        public LocalizedRouteConvention()
        {
            DefaultCulture = LocalizationDataHandler.DefaultCulture;
        }

        public void Apply(ApplicationModel application)
        {
            foreach (ControllerModel controller in application.Controllers)
            {    
                // If the controllerName is the same as the base controller for localization go next since it's irrelevant!
                // Basically Localization is a controller with 0 actions. Since it's what all other controllers inherit from.
                // It still ends up in application.Controllers which is why we just continue; here.
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
        /// <param name="a_selectorModels"></param>
        /// <param name="a_attributeRouteModel"></param>
        public void AddAttributeRouteModel(IList<SelectorModel> a_selectorModels, AttributeRouteModel a_attributeRouteModel)
        {
            // Override what seems to be default SelectorModel
            if (a_selectorModels.Count == 1 && a_selectorModels[0].AttributeRouteModel == null)
            {
                a_selectorModels[0].AttributeRouteModel = a_attributeRouteModel;
            }
            else
            {
                a_selectorModels.Add(new SelectorModel
                {
                    AttributeRouteModel = a_attributeRouteModel
                });
            }            
        }

        /// <summary>
        /// Adds the prefix local routs to each controller.
        /// Example: Culture = fi, Route = "moi"
        /// Create Route prefix: fi/moi   
        /// </summary>
        /// <param name="a_controller"></param>
        public void AddControllerRoutes(ControllerModel a_controller)
        {   
            // The controllerName (writing a_controller. every time is hard yo!)
            string controllerName = a_controller.ControllerName;

            // If the controller is the default controller then add the "/", "/culture" routes
            // If we don't do this / for example wouldn't work and would give 404. 
            // Instead /Default would be the only way to access the default controller.
            if (controllerName == LocalizationDataHandler.DefaultController)
            {
                // Set up the "/", "/culture1", "/culture2" for all supported cultures.
                // "/" is for the default culture.
                foreach(string culture in LocalizationDataHandler.SupportedCultures)
                {
                    string template = LocalizationDataHandler.DefaultCulture == culture ? "" : culture;

                    AttributeRouteModel defaultRoute = new AttributeRouteModel();
                    defaultRoute.Template = template;
                    AddAttributeRouteModel(a_controller.Selectors, defaultRoute);
                }
            }

            LocalizationDataHandler.AddControllerData(controllerName, DefaultCulture, controllerName);

            // Create the route for the controller,  since default controller should also be reachable by /default this is not done in the else statement
            // Which is not needed for the localized routing since linking to / is fine!            
            AttributeRouteModel controllerRoute = new AttributeRouteModel();
            controllerRoute.Template = a_controller.ControllerName;                    
            AddAttributeRouteModel(a_controller.Selectors, controllerRoute);

            AddControllerLocalizedRoutes(a_controller);
        }
        
        /// <summary>
        /// Add the localized routes for the controller model
        /// </summary>
        /// <param name="controllerModel"></param>
        public void AddControllerLocalizedRoutes(ControllerModel controllerModel)
        {
            // Get all the LocalizedRouteAttributes from the controller
            var controllerLocalizations = controllerModel.Attributes.OfType<LocalizedRouteAttribute>().ToList();            
            string controllerName = controllerModel.ControllerName;

            // So that any culture that doesn't have the controller added as a route will automatically get the default culture route,
            // Example if [LocalizedRoute("sv", ""] is not on the defaultcontroller it will be added so its found!
            HashSet<string> foundCultures = new HashSet<string>(LocalizationDataHandler.SupportedCultures);//.ToDictionary(x => x, x => x);
            foundCultures.Remove(LocalizationDataHandler.DefaultCulture);

            // Loop over all localized attributes
            foreach (LocalizedRouteAttribute attribute in controllerLocalizations)
            {
                string template = attribute.Culture;
                // If the attributeRoute isn't empty then we use the route name
                if (!String.IsNullOrEmpty(attribute.Route))
                {
                    // Add / if the route doesn't start with /
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
                LocalizationDataHandler.AddControllerData(controllerName, attribute.Culture, template);
                // Remove it from the dictionary having forgotten culture routes!
                // So eg:  /fi/koti   doesn't happen twice
                foundCultures.Remove(attribute.Culture);
            }

            // Add a route for the controllers that didn't have localization route attributes with their default name
            foreach (string culture in foundCultures)
            {
                string template = culture;
                if (!controllerName.Equals(LocalizationDataHandler.DefaultController, StringComparison.CurrentCultureIgnoreCase))
                {
                    template += "/" + controllerName;
                }

                AttributeRouteModel localRoute = new AttributeRouteModel();
                localRoute.Template = template;
                AddAttributeRouteModel(controllerModel.Selectors, localRoute);

                LocalizationDataHandler.AddControllerData(controllerName, culture, template);
            }
        }

        /// <summary>
        /// Adds the localized routes for a controller
        /// </summary>
        /// <param name="controllerModel"></param>
        public void AddActionRoutes(ControllerModel controllerModel)
        {
            // The controllerName (writing a_controler. everytime is hard yo!)
            string controllerName = controllerModel.ControllerName;
            // All the new localized actions
            List<ActionModel> newActions = new List<ActionModel>();
            // Loop through all the actions to add their routes and also get the localized actions
            foreach (ActionModel action in controllerModel.Actions)
            {                
                string actionName = action.ActionName;
                // If any parameters are needed such as /{index}
                string parameterTemplate = "";

                SelectorModel defaultSelectionModel = action.Selectors.FirstOrDefault(x => x.AttributeRouteModel != null);

                List<string> sortedRouteParameters = new List<string>();

                // If there is no[Route()] Attribute then create one for the route.
                if (defaultSelectionModel == null || defaultSelectionModel.AttributeRouteModel == null)
                {
                    AttributeRouteModel attributeRouteModel = new AttributeRouteModel();

                    if (action.Parameters.Count > 0)
                    {
                        foreach (ParameterModel parameter in action.Parameters)
                        {
                            sortedRouteParameters.Add(parameter.ParameterName.ToLower());
                        }
                    }

                    if (action.ActionName != LocalizationDataHandler.DefaultAction)
                    {
                        attributeRouteModel.Template = actionName;
                        // Add the action name as it is eg: about will be about!
                        LocalizationDataHandler.AddActionData(controllerName, actionName, DefaultCulture, actionName, actionName, sortedRouteParameters);
                    }
                    else
                    {
                        attributeRouteModel.Template = "";
                        // If action name is the default name then just add route as ""
                        // Final result for default controller & action will be "" + ""  => /
                        LocalizationDataHandler.AddActionData(controllerName, actionName, DefaultCulture, "", controllerName, sortedRouteParameters);
                    }

                    AddAttributeRouteModel(action.Selectors, attributeRouteModel);
                }
                // If a route already existed then check for parameter arguments to add to the cultural routes
                else
                {                    
                    string template = defaultSelectionModel.AttributeRouteModel.Template;

                    parameterTemplate = ParseParameterTemplate(template, sortedRouteParameters);

                    LocalizationDataHandler.AddActionData(controllerName, actionName, DefaultCulture, actionName, actionName, sortedRouteParameters);
                }

                var localizedActions = CreateLocalizedActionRoutes(controllerModel, action, parameterTemplate, sortedRouteParameters);
                newActions.AddRange(localizedActions);
            } // End foreach a_controller.Actions

            // Now add all the new actions to the controller
            foreach (ActionModel action in newActions)
            {
                controllerModel.Actions.Add(action);
            }            
        }

        public List<ActionModel> CreateLocalizedActionRoutes(ControllerModel controllerModel, ActionModel actionModel, string parameterTemplate, List<string> sortedRouteParameters)
        {
            string controllerName = controllerModel.ControllerName;
            string actionName = actionModel.ActionName;
            var actionLocalizationsAttributes = actionModel.Attributes.OfType<LocalizedRouteAttribute>().ToList();

            List<ActionModel> localizedActions = new List<ActionModel>();

            foreach (LocalizedRouteAttribute attribute in actionLocalizationsAttributes)
            {
                string route = attribute.Route + parameterTemplate;
                // This copies all existing Attributes on the ActionModel,  [Route] [HttpGet] e.t.c.
                // Sourcefile: https://github.com/aspnet/Mvc/blob/dev/src/Microsoft.AspNetCore.Mvc.Core/ApplicationModels/ActionModel.cs
                ActionModel newLocalizedActionModel = new ActionModel(actionModel);

                // Clear the Selectors or it will have shared selector data from default route.
                // This however clears the ActionConstraints like [HttpGet] and [HttpPost]
                newLocalizedActionModel.Selectors.Clear();
                AttributeRouteModel newLocalizedAttributeRouteModel = new AttributeRouteModel();
                //newLocalizedAttributeRouteModel.Template = attribute.Route;   
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
                if (actionName == LocalizationDataHandler.DefaultAction && !String.IsNullOrEmpty(linkName))
                {
                    linkName = GetLocalizedControllerName(controllerModel, attribute.Culture);
                }

                // Add the localized route for the action
                // Example of final route:  "fi/koti" + "/" + "ota_yhteyttä"
                LocalizationDataHandler.AddActionData(controllerName, actionName, attribute.Culture, attribute.Route, linkName, sortedRouteParameters);

            }

            return localizedActions;
        }

        /// <summary>
        /// Get the 
        /// </summary>
        /// <param name="controllerModel"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public string GetLocalizedControllerName(ControllerModel controllerModel, string culture)
        {
            var localizedRouteAttributes = controllerModel.Attributes.OfType<LocalizedRouteAttribute>().ToList();

            string name = controllerModel.ControllerName;

            foreach (LocalizedRouteAttribute attribute in localizedRouteAttributes)
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
                        name = LocalizedRouteAttribute.ConvertRouteToLink(attribute.Route, culture);
                    }

                    break;
                }
            }

            return name;
        }

        /// <summary>
        /// Parses the input template and returns a parsed template that only contains the {parameter} values.
        /// It also adds parameters it encounters to the sortedRouteParamaters list.
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
                // Incase of "/action/" 
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
        /// <param name="a_actionComponent"></param>
        /// <returns></returns>
        public string GetParameterName(string a_actionComponent)
        {
            // Example: { param:int = 1 }
            string name = a_actionComponent.Split(new char[] { '=', ':' }).First();
            // Remove whitespace since that's invalid!
            // Also remove the { character
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

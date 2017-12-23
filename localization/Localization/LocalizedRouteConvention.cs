using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.ApplicationModels;

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
        /// Adds the prefix local routs to each controller.
        /// Example: Culture = fi, Route = "moi"
        /// Create Route prefix: fi/moi   
        /// </summary>
        /// <param name="a_controller"></param>
        public void AddControllerRoutes(ControllerModel a_controller)
        {
            // Get all the LocalizedRouteAttributes from the controller
            var controllerLocalizations = a_controller.Attributes.OfType<LocalizedRouteAttribute>().ToList();
            // The controllerName (writing a_controler. everytime is hard yo!)
            string controllerName = a_controller.ControllerName;
            // If the controller is the default controller then add the "/" route by adding an empty ""
            if (controllerName == LocalizationDataHandler.DefaultController)
            {
                //CultureAttributeRouteModel defaultRoute = new CultureAttributeRouteModel(DefaultCulture);
                AttributeRouteModel defaultRoute = new AttributeRouteModel();
                defaultRoute.Template = "";
                a_controller.AttributeRoutes.Add(defaultRoute);

                // If it's the default controller then
                LocalizationDataHandler.AddControllerData(controllerName, DefaultCulture, "");
            }
            else
            {
                // Else add the controller name!
                LocalizationDataHandler.AddControllerData(controllerName, DefaultCulture, controllerName);
            }

            // Create the route for the controller,  since default controller should also be reachable by /default this is not done in the else statement
            // Which is not needed for the localized routing since linking to / is fine!
            //CultureAttributeRouteModel controllerRoute = new CultureAttributeRouteModel(DefaultCulture, a_controller.ControllerName); 
            AttributeRouteModel controllerRoute = new AttributeRouteModel();
            controllerRoute.Template = a_controller.ControllerName;
            a_controller.AttributeRoutes.Add(controllerRoute);

            // So that any culture that doesn't have the controller added as a route will automatically get the default culture route,
            // Example if [LocalizedRoute("sv", ""] is not on the defaultcontroller it will be added so its found!
            Dictionary<string, string> foundCultures = LocalizationDataHandler.SupportedCultures.ToDictionary(x => x, x => x);
            foundCultures.Remove(LocalizationDataHandler.DefaultCulture);
            
            // Loop over all localized attributes
            foreach (LocalizedRouteAttribute attribute in controllerLocalizations)
            {
                string template = attribute.Culture + "/" + attribute.Route;
                //CultureAttributeRouteModel localRoute = new CultureAttributeRouteModel(attribute.Culture, template);
                AttributeRouteModel localRoute = new AttributeRouteModel();
                localRoute.Template = template;
                a_controller.AttributeRoutes.Add(localRoute);

                // Add the route to the localizations dictionary
                LocalizationDataHandler.AddControllerData(controllerName, attribute.Culture, template);
                // Remove it from the dictionary having forgotten culture routes!
                // So eg:  /fi/koti   doesn't happen twice
                foundCultures.Remove(attribute.Culture);
            }
            
            // Add a route for the controllers that didn't have localization route attributes with their default name
            foreach (KeyValuePair<string, string> culture in foundCultures)
            {
                string tempName = controllerName;
                if (controllerName == LocalizationDataHandler.DefaultController)
                {
                    tempName = "";
                   
                }
                string template = culture.Value + "/" + tempName;


                //CultureAttributeRouteModel localRoute = new CultureAttributeRouteModel(culture.Value, template);
                AttributeRouteModel localRoute = new AttributeRouteModel();
                localRoute.Template = template;
                a_controller.AttributeRoutes.Add(localRoute);

                LocalizationDataHandler.AddControllerData(controllerName, culture.Value, template);
            }
        }  
        
        /// <summary>
        /// Adds the localized routes for a controller
        /// </summary>
        /// <param name="a_controller"></param>
        public void AddActionRoutes(ControllerModel a_controller)
        {
            // The controllerName (writing a_controler. everytime is hard yo!)
            string controllerName = a_controller.ControllerName;
            // All the new localized actions
            List<ActionModel> newActions = new List<ActionModel>();
            // Loop through all the actions to add their routes and also get the localized actions
            foreach (ActionModel action in a_controller.Actions)
            {
                string actionName = action.ActionName;
                // If any parameters are needed such as /{index}
                string parameterTemplate = "";                

                // If there is no [Route()] Attribute then create one for the route.
                if (action.AttributeRouteModel == null)
                {
                    //action.AttributeRouteModel = new CultureAttributeRouteModel(DefaultCulture);
                    action.AttributeRouteModel = new AttributeRouteModel();

                    if (action.ActionName != LocalizationDataHandler.DefaultAction)
                    {
                        action.AttributeRouteModel.Template = actionName;
                        // Add the action name as it is eg: about will be about!
                        LocalizationDataHandler.AddActionData(controllerName, actionName, DefaultCulture, actionName, actionName);
                    }
                    else
                    {
                        action.AttributeRouteModel.Template = "";
                        // If action name is the default name then just add route as ""
                        // Final result for default controller & action will be "" + ""  => /
                        LocalizationDataHandler.AddActionData(controllerName, actionName, DefaultCulture, "", controllerName);                        
                    }
                }
                // If a route already existed then check for parameter arguments to add to the cultural routes
                else
                {                    
                    // Check if the route has parameters
                    string[] actionComponents = action.AttributeRouteModel.Template.Split('/');

                    for (int i = 0; i < actionComponents.Length; i++)
                    {
                        // Check if first character starts with {
                        if (actionComponents[i][0] == '{')
                        {
                            parameterTemplate += "/" + actionComponents[i];
                        }                        
                    }                  
                }

                var actionLocalizationsAttributes = action.Attributes.OfType<LocalizedRouteAttribute>().ToList();

                foreach (LocalizedRouteAttribute attribute in actionLocalizationsAttributes)
                {
                    string route = attribute.Route += parameterTemplate;
                    ActionModel newLocalizedActions = new ActionModel(action);
                    //newLocalizedActions.AttributeRouteModel = new CultureAttributeRouteModel(attribute.Culture, attribute.Route);
                    newLocalizedActions.AttributeRouteModel = new AttributeRouteModel();
                    newLocalizedActions.AttributeRouteModel.Template = attribute.Route;
                    newActions.Add(newLocalizedActions);
                    // Add the localized route for the action
                    // Example of final route:  "fi/koti" + "/" + "ota_yhteyttä"
                    LocalizationDataHandler.AddActionData(controllerName, actionName, attribute.Culture, attribute.Route, attribute.Link);
                }
            }
            // Now add all the new actions to the controller
            foreach (ActionModel action in newActions)
            {
                a_controller.Actions.Add(action);
            }
        }      
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace localization.Localization
{
    public class LocalizationController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {            
            base.OnActionExecuting(context);
            // Get the action & controllerName            
            ControllerActionDescriptor actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;

            string controllerName = actionDescriptor?.ControllerName ?? "";
            string actionName = actionDescriptor?.ActionName ?? "";

            string culture = CultureInfo.CurrentCulture.Name;
            ViewData["culture"] = culture;

            // If the culture isn't default then ask database for the data needed to change.            
            Dictionary<string, string> pagedata = new Dictionary<string, string>()
            {
                { "header", "a header!" },
                { "body", "a body text!" }
            };  

            ViewData["pagedata"] = pagedata;   
        }        
    }
}

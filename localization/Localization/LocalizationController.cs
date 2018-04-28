using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace localization.Localization
{
    public class LocalizationController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {            
            base.OnActionExecuting(context);            
            
            string culture = CultureInfo.CurrentCulture.Name;
            ViewData["culture"] = culture;
            ViewData["controller"] = ControllerContext.ActionDescriptor.ControllerName;
            ViewData["action"] = ControllerContext.ActionDescriptor.ActionName;
        }  
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;

namespace localization.Localization
{
    public class LocalizationController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {            
            base.OnActionExecuting(context); 

            string culture = CultureInfo.CurrentCulture.Name;
            ViewData["culture"] = culture;
        }  
        
        
    }
}

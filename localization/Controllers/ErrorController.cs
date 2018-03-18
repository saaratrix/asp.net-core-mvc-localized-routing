using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using localization.Localization;
using Microsoft.AspNetCore.Mvc;

namespace localization.Controllers
{
    
    public class ErrorController : Controller
    {
        [Route("/Error/{0}")]
        public IActionResult Index(int error)
        {
            // Since we're not using LocalizationController this isn't automatically set
            string culture = CultureInfo.CurrentCulture.Name;

            return View();
        }
    }
}
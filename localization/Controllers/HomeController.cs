using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using localization.Models;
using localization.Localization;

namespace localization.Controllers
{
    /*
     * This HomeController is an example of using the [LocalizationRoute] attribute for different cultures and the link text.
    */

    // Routes for each culture:
    // Default: /Home           - / for the Index action
    // Finnish: /fi/koti        - /fi for the Index action.
    // Swedish: /sv/Hem         - /sv for the Index action.
    [LocalizationRoute("fi", "koti")]        
    [LocalizationRoute("sv", "Hem", "Hemma")] // The link text for <a>linktext</a> will be Hemma    
    public class HomeController : LocalizationController
    {
        public IActionResult Index()
        {
            return View();
        }

        // Routes for each culture:
        // Default: /Home/About
        // Finnish: /fi/koti/meistä
        // Swedish: /sv/Hem/om
        [LocalizationRoute("fi", "meistä")]
        [LocalizationRoute("sv", "om")]
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }
        
        // Routes for each culture:
        // Default: /Home/Contact
        // Finnish: /fi/koti/ota_yhteyttä
        // Swedish: /sv/Hem/kontakta-oss
        [LocalizationRoute("fi", "ota_yhteyttä")]                  // Automatically converts ota_yhteyttä to Ota Yhteyttä for the link text       
        [LocalizationRoute("sv", "kontakta-oss", "Kontakta Oss")]  // Explicitly tell the link text to be Kontakta Oss
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }
    }
}

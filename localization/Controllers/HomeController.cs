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
     * This HomeController is an example of using the [LocalizedRoute] attribute for different cultures and the link text.
    */

    // Routes for each culture:
    // Default: /Home           - / for the Index action
    // Finnish: /fi/koti        - /fi for the Index action.
    // Swedish: /sv/Hem         - /sv for the Index action.
    [LocalizedRoute("fi", "koti")]        
    [LocalizedRoute("sv", "Hem", "Hemma")] // The link text for <a>linktext</a> will be Hemma    
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
        [LocalizedRoute("fi", "meistä")]
        [LocalizedRoute("sv", "om")]
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }
        
        [LocalizedRoute("fi", "ota_yhteyttä")]                  // Automatically converts ota_yhteyttä to Ota Yhteyttä for the link text       
        [LocalizedRoute("sv", "kontakta-oss", "Kontakta Oss")]  // Explicitly tell the link text to be Kontakta Oss
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }
    }
}

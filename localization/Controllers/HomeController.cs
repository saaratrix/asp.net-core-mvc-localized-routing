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
    // This is not neccesary if Home is the defaultcontroller, automatically happens!    
    [LocalizedRoute("fi")]
    [LocalizedRoute("sv")]
    public class HomeController : LocalizationController
    {   
        public IActionResult Index()
        {
            return View();
        }

        [LocalizedRoute("fi", "miestä")]
        [LocalizedRoute("sv", "om")]
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        // Automatically makes ota_yhteyttä => Ota Yhteyttä
        [LocalizedRoute("fi", "ota_yhteyttä")]
        // Explicitly tell the link text to be Kontakta Oss
        [LocalizedRoute("sv", "kontakta-oss", "Kontakta Oss")]
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

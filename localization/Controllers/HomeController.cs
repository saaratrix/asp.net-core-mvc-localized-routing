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
    // Creates the route /fi/koti  for finnish culture
    // Creates the route /sv/Home for swedish culture since Route = ""
    [LocalizedRoute("fi", "koti")]
    [LocalizedRoute("sv")]
    public class HomeController : LocalizationController
    {
        public IActionResult Index()
        {
            return View();
        }

        [LocalizedRoute("fi", "meistä")]
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
    }
}

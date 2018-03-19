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
        [Route("Error")]
        public IActionResult Index()
        {
            return View();
        }

        // Need the [Route] attribute or the error is just a blank screen it doesn't work
        [Route("/Error/{0}")]
        public IActionResult Index(int error)
        {
            return View();
        }
    }
}
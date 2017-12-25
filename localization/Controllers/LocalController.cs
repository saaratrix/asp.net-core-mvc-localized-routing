using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using localization.Localization;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace localization.Controllers
{
    [LocalizedRoute("fi", "localFi")]
    // No need to name the route local, That's the name it automatically gets
    //[LocalizedRoute("sv", "local")]
    public class LocalController : LocalizationController
    {        
        public LocalController()
        {
             
        }      

        // GET: /<controller>/
        public IActionResult Index()
        {           
            return View();
        }

        // Add the route for default culture with parameters
        [Route("parameter/{index}/{test}")]
        // Final route is : /fi/local/param/{index}/{test}
        [LocalizedRoute("fi", "param")]
        // Sv is automatically set as parameter/{index}/{test}
        //[LocalizedRoute("sv", "parameter")]
        public IActionResult Parameter(int index, string test)
        {
            ViewData["index"] = index;
            ViewData["test"] = test;

            return View();
        }
    }
}

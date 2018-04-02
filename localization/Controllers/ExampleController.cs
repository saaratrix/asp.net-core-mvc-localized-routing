using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using localization.Localization;
using localization.Models.Local;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace localization.Controllers
{
    [LocalizedRoute("fi", "exampleFi")]
    // No need to name the route example, That's the name it automatically gets
    //[LocalizedRoute("sv", "example")]
    public class ExampleController : LocalizationController
    {        
        public ExampleController()
        {
             
        }      

        // GET: /<controller>/
        public IActionResult Index()
        {           
            return View();
        }

        // Add the route for default culture with parameters
        [Route("parameter/{index}/{test}")]
        //[HttpGet("parameter/{index}/{test}")]
        // Final route is : /fi/exampleFi/param/{index}/{test}
        [LocalizedRoute("fi", "param")]        
        // Sv is automatically set as parameter/{index}/{test}
        //[LocalizedRoute("sv", "parameter")]        
        public IActionResult Parameter(int index, string test)
        {
            ViewData["index"] = index;
            ViewData["test"] = test;
            ViewData["post"] = false;
            return View();
        }
        
        [HttpPost()]
        // Final route for /fi/ is : /fi/exampleFi/param/{index}/{test}
        [LocalizedRoute("fi", "param")]
        //[ValidateAntiForgeryToken]        
        public IActionResult Parameter(ParameterViewModel model)
        {
            ViewData["index"] = model.Index;
            ViewData["test"] = model.Test;
            ViewData["post"] = true;
            return View(model);
        }
    }
}

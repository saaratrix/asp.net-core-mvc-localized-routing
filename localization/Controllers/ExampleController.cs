using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using localization.Localization;
using localization.Models.Example;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860
namespace localization.Controllers
{
    // Creates the route /fi/exampleFi for finnish culture
    [LocalizedRoute("fi", "exampleFi")]
    // Since there is no [LocalizedRoute] for swedish culture it will be automatically added 
    // With the route /sv/Example 
    public class ExampleController : LocalizationController
    {        
        public ExampleController()
        {
             
        }      
                
        public IActionResult Index()
        {           
            return View();
        }       

        // Add the route for default culture with parameters
        [Route("parameter/{index}/{test}")]
        //[HttpGet("parameter/{index}/{test}")]
        // Final route is : /fi/exampleFi/param/{index}/{test}
        [LocalizedRoute("fi", "param")]        
        // Since there is no swedish [LocalizedRoute] it will automatically create one.
        // The swedish route will be /fi/Example/Paramaeter/{index}/{test}
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
        public IActionResult Parameter(ParameterViewModel model)
        {
            ViewData["index"] = model.Index;
            ViewData["test"] = model.Test;
            ViewData["post"] = true;
            return View(model);
        }

        [Route("getform/{index}/{test}")]
        public IActionResult GetForm(int index, string test)
        {
            ViewData["index"] = index;
            ViewData["test"] = test;

            return View();
        }
    }
}

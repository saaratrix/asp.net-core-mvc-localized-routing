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

namespace localization.Controllers
{
    /*
     * This Example controller demonstrates how to set up a new controller and a route with a form.
     * The form uses the following http methods:
     * GET with parameters
     * POST with a view model     
    */

    // Routes for each culture:
    // Default: /Example
    // Finnish: /fi/exampleFi
    // Swedish: /sv/Example     - Takes the name of controller since no [LocalizationRoute] for swedish culture   
    // The link text for <a> tags will be ExampleFi
    [LocalizationRoute("fi", "exampleFi")]    
    public class ExampleController : LocalizationController
    {        
        public ExampleController()
        {             
        }      
                
        public IActionResult Index()
        {           
            return View();
        }
        
        // Routes for each culture:
        // Default: /Example/Parameter/{index}/{test}
        // Finnish: /fi/exampleFi/param/{index}/{test}
        // Swedish: /sv/Example/Parameter/{index}/{test}        - Gets the Action name automatically because no [LocalizationRoute] attribute
        // [HttpGet("parameter/{index}/{test}")]                - [HttpGet] can be used instead of [Route]
        [Route("parameter/{index}/{test}")]        
        [LocalizationRoute("fi", "param")]
        public IActionResult Parameter(int index, string test)
        {
            ViewData["index"] = index;
            ViewData["test"] = test;
            ViewData["post"] = false;
            return View();
        }

        // Routes for each culture:
        // Default: /Example/Parameter
        // Finnish: /fi/exampleFi/param
        // Swedish: /sv/Example/Parameter
        [HttpPost()]        
        [LocalizationRoute("fi", "param")]        
        public IActionResult Parameter(ParameterViewModel model)
        {
            ViewData["index"] = model.Index;
            ViewData["test"] = model.Test;
            ViewData["post"] = true;
            return View(model);
        }
    }
}

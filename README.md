# ASP.NET Core 2.1 MVC localized routing

A route localization example for ASP.NET Core 2.1 MVC. 
The solution uses attributes on the controllers and actions to determine the localized routing for each different culture. 
A convention is added in Startup.cs to iterate over all the controllers and actions to set their routes based on the `[LocalizationRoute]` attributes. 

`services.AddMvc(options => { options.Conventions.Add(new LocalizationRouteConvention()); });`

Code used for the blogpost: http://saaratrix.blogspot.se/2017/12/localized-routing-aspnet-core-mvc-2.html

For the old solution for [ASP.NET Core 1.0 MVC release candidate 1](../../tree/core-1.0-rc-1)

Example usage for a controller:
```
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
            return View();
        }
    }
```




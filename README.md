# ASP.NET Core 2.1 MVC localized routing

A route localization example for ASP.NET Core 2.1 MVC. 
The solution uses attributes on the controllers and actions to determine the localized routes for each different culture. 
A convention is added in Startup.cs to iterate over all the controllers and actions to set their routes based on the `[LocalizationRoute]` attributes. 

**Note this solution most likely doesn't work with Areas if there's a controller with the same name. [#Issue 28](https://github.com/saaratrix/asp.net-core-mvc-localized-routing/issues/28)**

Code used for the blog post: https://saaratrix.blogspot.com/2017/12/localized-routing-aspnet-core-mvc-2.html

For the old solution for [ASP.NET Core 1.0 MVC release candidate 1](../../tree/core-1.0-rc-1)

Example usage for a controller:
```cs
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

And example of using the CultureActionLinkTagHelper in the views for the anchor element.
```
// Generated html: <a href="/Home/About">About</a>
<a asp-controller="home" asp-action="about" cms-culture="en">About</a>

// Generated html: <a href="/fi/koti/meistä">Meistä</a>
<a asp-controller="home" asp-action="about" cms-culture="fi">About</a>

// Generated html: <a href="/sv/Hem/om">Om</a>
<a asp-controller="home" asp-action="about" cms-culture="sv">About</a>

// Leaving cms-culture="" empty will use the current request culture.
// If user is at /fi/... then finnish culture is used then the generated html would be:
// <a href="/fi/koti/meistä">Meistä</a>
<a asp-controller="home" asp-action="about" cms-culture="">About</a>
```

To set up the localization you need to include it in Startup.cs or an equivalent file in your project.
The method everything is set up in is:  
`public void ConfigureServices(IServiceCollection services)` 
In this method we set settings such as what the supported cultures are.
You could set the DefaultCulture and SupportedCultures directly in the LocalizationRouteDataHandler.cs class as well.
Example usage with 3 cultures, en, fi, sv
```cs
public void ConfigureServices(IServiceCollection services)
{
        // ... Other code above here ...
        // Set up what culture to use
        LocalizationRouteDataHandler.DefaultCulture = "en";
        LocalizationRouteDataHandler.SupportedCultures = new Dictionary<string, string>()
        {
            { "en", "English" },
            { "fi", "Suomeksi" },
            { "sv", "Svenska" }
        };
        // Add the LocalizationRouteConvention to MVC Conventions.
        // Without this the localized routes won't work because this is what initializes all the routes!
        IMvcBuilder mvcBuilder = services.AddMvc(options =>
        {
            options.Conventions.Add(new LocalizationRouteConvention());
        });
        // Set up the request localization options which is what sets the culture for every http request.
        // Meaning if you visit /controller the culture is LocalizationRouteDataHandler.DefaultCulture.
        // And if you visit /fi/controller the culture is fi.
        services.Configure<RequestLocalizationOptions>(options =>
        {
            options.DefaultRequestCulture = new RequestCulture(LocalizationRouteDataHandler.DefaultCulture, LocalizationRouteDataHandler.DefaultCulture);
            foreach (var kvp in LocalizationRouteDataHandler.SupportedCultures)
            {
                CultureInfo culture = new CultureInfo(kvp.Key);
                options.SupportedCultures.Add(culture);
                options.SupportedUICultures.Add(culture);
            }
            options.RequestCultureProviders = new List<IRequestCultureProvider>()
            {
                new UrlCultureProvider(options.SupportedCultures)
            };
        });
        // Set up Resource file usage and IViewLocalizer
        services.AddLocalization(options => options.ResourcesPath = "Resources");
        // Views.Shared._Layout is for the /Views/Shared/_Layout.cshtml file
        mvcBuilder.AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
        // Add support for localizing strings in data annotations (e.g. validation messages) via the
        // IStringLocalizer abstractions.
        .AddDataAnnotationsLocalization();
        // ... Other code below here ...
}
```

LocalizationDataHandler.cs has the default values used by the localization app.
For example if your default controller isn't `Home` then change it in this file or in Startup.cs depending on your preference.
```cs
public static class LocalizationRouteDataHandler
{
    /// <summary>
    /// The default culture.
    /// </summary>
    public static string DefaultCulture { get; set; }
    /// <summary>
    /// The dictionary of all supported cultures.
    /// Key = Culture Name
    /// Value = Display Name
    /// Example: en, English
    /// </summary>
    public static Dictionary<string, string> SupportedCultures { get; set; }
    public static string DefaultController { get; set; } = "Home";
    public static string DefaultAction { get; set; } = "Index";
}
```



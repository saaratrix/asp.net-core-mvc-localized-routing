# ASP.NET Core 2.1 MVC localized routing

A route localization example for ASP.NET Core 2.1 MVC. 
The solution uses attributes on the controllers and actions to determine the localized routing for each different culture. 
A convention is added in Startup.cs to iterate over all the controllers and actions to set their routes based on the `[LocalizationRoute]` attributes. 

`services.AddMvc(options => { options.Conventions.Add(new LocalizationRouteConvention()); });`

Code used for the blogpost: http://saaratrix.blogspot.se/2017/12/localized-routing-aspnet-core-mvc-2.html

For the old solution for [ASP.NET Core 1.0 MVC release candidate 1](https://github.com/asp.net-core-mvc-localized-routing/tree/core-1.0-rc-1)


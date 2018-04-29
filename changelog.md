# 2.0.2

### Changes
* CultureActionLinkTagHelper:
    - Refactored the way it gets the href and link text.
        It might not work with raw `?querystring` or `#hashFragments`.
    - Added attribute `cms-keep-link` for anchor tags. Used like this: `cms-keep-link="true"` . It's used if the link text shouldn't be changed to the localized action name.
        For example `<a asp-controller="Home" asp-action="Index" cms-culture="@ViewData["culture"]" cms-keep-link="true" >/Brand</a>` Will make sure that `/Brand` is the link text instead of the action's name.
    - Added `KeepLinkDefault` as a static property to set default behavior for keeping links 
    - Fixed bug where actions could be double. 
        [#7](../../issues/7)
* LocalizationRouteDataHandlers
    - Changed SupportedCultures from type `List<string>` to `HashSet<string>`
    - Added GetOrderedParameters() that based on input routeValues returns tries to return the values in correct order.   
* LocalizationRouteAttribute
    - When it generates the LinkName it also breaks on `-` in addition to `_`. 
        This is controlled with a static property `RouteToLinkSplitLetters` 
    - Added `ConvertRouteToLink` that converts a route to a link name value.
* LocalizedRouteConvention
    - Removed DefaultCulture property. The convention uses `LocalizationRouteDataHandlers.DefaultCulture` instead.
    - Fixed bug where `/culture/` wouldn't work for default controller as a valid route. 
        [#4]((../../issues/4)) 
    - Fixed bug where Link Name for a culture and default action would be the default culture's action name.
        [#15](../../issues/15)

### Changes to examples and documentation
* Added global localized error handling through `ErrorController`
* Added `IViewLocalizer` injection in `_ViewImports.cshtml` so there's a `Localizer` available for every view.
* Added `_CultureSelector` partial view as an example of changing between cultures. 
* Added usages of `IViewLocalizer Localizer` for the ExampleController views.


* Tests:
    - Added unit tests for `LocalizationDataHandler.cs`, `LocalizedRouteAttribute.cs`, `LocalizedRouteConvention.cs`
    - Added integration tests for GET routes to HomeController and ExampleController.

### Breaking changes
* Renamed classes
    - `LocalizedRouteAttribute` to `LocalizationRouteAttribute` 
    - `LocalizedRouteConvention` to `LocalizationRouteConvention`
    - `LocalizedUrlResult` to `LocalizationUrlResult`
    - `LocalizationDataHandler` to `LocalizationRouteDataHandler`
    - `CultureControllerData` to `CultureControllerRouteData`
    - `CultureActionData` to `CultureActionRouteData`
    - `CultureUrlData.cs` to `CultureUrlRouteData`
* `LocalizationRouteDataHandler`:
    - Changed property `SupportedCultures` from `List<string>` to `Dictionary<string, string>`
    - Renamed `AddControllerData` to `AddControllerRouteData`
    - Renamed `AddActionData` to `AddActionRouteData`

### Issues closed
* \#3   - [Route([controller]/[action]) on a controller causes double actions](../../issues/3)
* \#4   - [Localized default controller with default action can't just be /{culture}/](../../issues/4) 
* \#5   - [Global errorhandling](../../issues/5)
* \#7   - [Actions are double](../../issues/7)
* \#9   - [Change SupportedCultures to use Dictionary instead of List](../../issues/9)
* \#15  - [Add the controller localized name to link text for default action for controllers](../../issues/15)

### Issues closed related to tests or documentation
* \#8   - [Unit tests for LocalizationDataHandler](../../issues/8)
* \#10  - [Integration GET tests](../../issues/10)
* \#11  - [Unit tests for LocalizedRouteConvention](../../issues/11)
* \#13  - [Rename LocalController to ExampleController](../../issues/13)
* \#14  - [Add example of IViewLocalizer to Example views](../../issues/14)
* \#17  - [Update code to follow Microsoft Naming Conventions ](../../issues/17)
* \#18  - [LocalizedRouteAttribute Tests](../../issues/18)
* \#19  - [Add a culture change links](../../issues/19)
* \#20  - [Unify naming, either Localization or Localized ](../../issues/20)

# 2.0.1

### Changes

* Added cms-culture TagHelper for `<form>` tag 
* Added more examples in AccountController with the Login & Register route
* Added a parameter post route in LocalController as an example and testing.
    Also rewrote the view for the parameter action to be able to test the post action.
* Changed name of `LocalizationDataHandler.GetCultureFromHref()` to `LocalizationDataHandler.GetCultureFromUrl()`
* Changed the brandUrl to either be `/{culture}` or `/` in `~/Views/Shared/_Layout.cshtml`

### Bugfixes: 
*   Solved bug where the anchor tag helper `cms-culture` could generate incorrect url.    
    For this example the controller has attribute `[LocalizedRoute("fi", "localFI")]` 
    and the action looks like this: 
    ```
    [Route("parameter/{index}/{test}")]
    [LocalizedRoute("fi", "param")]
    public IActionResult Parameter(int index, string test)
    ```    
    However for the anchor tag helper 
    
    `<a asp-controller="Local" asp-action="Parameter" asp-route-index="5" asp-route-test="fi" cms-culture="fi">`
    
    The bug generated incorrect url `/fi/localFI/parameter/5/fi` instead of `/fi/localFI/param/5/fi`   
	    
* Fixed bug reported through a comment on blog! <3
    Inside `LocalizedRouteConvention.cs` when the `newLocalizedActionModel.Selectors.Clear()` is called.
    The action constraints for `[HttpGet], [HttpPost]` are also cleared so they need to be readded
		

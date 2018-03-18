# 2.0.2

### Changes
* Added global localized errorhandling

### Bugfixes

### Issues closed

* #5 - [Global errorhandling](issues/5)

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
		

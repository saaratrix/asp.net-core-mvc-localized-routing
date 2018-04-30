This has information about what classes and methods are being tested.

# Unit tests
- LocalizationRouteDataHandler
    - GetUrl()
    - GetOrderedParameters()
    - GetCultureFromUrl()
- LocalizationRouteAttribute
    - Constructor()
    - ConvertRouteToLink()
- LocalizationRouteConvention
    - GetLocalizedControllerName()
    - ParseParameterTemplate()
    - GetParameterName()
            
# Integration tests
- GET tests for routes for english, finnish and swedish culture
    - Home, Index
    - Home, About
    - Home, Contact
    - Example, Index
    - Example, Parameter, 2 parameters
- GET tests to a 404 url
- GET tests to check the navigation menu has correct href and link texts
- POST request to `/Example/Parameter` form for english, finnish and swedish culture.
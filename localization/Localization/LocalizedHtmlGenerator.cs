using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace localization.Localization
{
    /*
     * For <a> AnchorTagHelper is used that calls either
     *  GenerateActionLink()
     *      urlHelper.Action(actionName, controllerName, routeValues, protocol, hostname, fragment)
     *          var virtualPathData = GetVirtualPathData(routeName: null, values: valuesDictionary);
     *          GenerateUrl(actionContext.Protocol, actionContext.Host, virtualPathData, actionContext.Fragment);
     *  GenerateRouteLink()
     *      urlHelper.RouteUrl(routeName, routeValues, protocol, hostName, fragment)
     *          var virtualPathData = GetVirtualPathData(routeContext.RouteName, valuesDictionary);
     *          return GenerateUrl(routeContext.Protocol, routeContext.Host, virtualPathData, routeContext.Fragment);
     *          
     *  
     *  GetVirtualPath(string routeName, RouteValueDictionary values)
     *      var context = new VirtualPathContext(HttpContext, AmbientValues, values, routeName);
            return Router.GetVirtualPath(context);

    For UrlHelper:
    protected IRouter Router => ActionContext.RouteData.Routers[0];  // Which is a RouteCollection class

    For RouteCollection
    // context.RouteName is null in my examples
    GetVirtualPath(VirtualPathContext context) 
        return NormalizeVirtualPath(GetVirtualPath(context, _routes));  // _routes[0] =  AttributeRoute which uses a TreeRouter

    GetVirtualPath(VirtualPathContext context, List<IRouter> routes) 
        TreeRouter.GetVirtualPath(context);

    // This is where it matches the parameters to each value.
    // RouteValues might be { action: parameter, controller: local, test: fi, index: 5 }
    // It then sorts it so it's in the correct order of {controller}/{action}/{index}/{test}
    TreeRouter:

        GetVirtualPath(VirtualPathContext context)
            // The decision tree will give us back all entries that match the provided route data in the correct
            // order. We just need to iterate them and use the first one that can generate a link.
            var matches = _linkGenerationTree.GetMatches(context);        
    */

    // Overriding functionality for the DefaultHtmlGenerator to get the localized route from LocalizationData
    // AnchorTagHelper: https://github.com/aspnet/Mvc/blob/dev/src/Microsoft.AspNetCore.Mvc.TagHelpers/AnchorTagHelper.cs
    // DefaultHtmlGenerator: https://github.com/aspnet/Mvc/blob/dev/src/Microsoft.AspNetCore.Mvc.ViewFeatures/ViewFeatures/DefaultHtmlGenerator.cs
    // Routing: https://github.com/aspnet/Mvc/tree/dev/src/Microsoft.AspNetCore.Mvc.Core/Routing
    // UrlHelper: https://github.com/aspnet/Mvc/blob/dev/src/Microsoft.AspNetCore.Mvc.Core/Routing/UrlHelper.cs
    // VirtualPathData: https://github.com/aspnet/Routing/blob/dev/src/Microsoft.AspNetCore.Routing.Abstractions/VirtualPathData.cs
    // RouteCollection: https://github.com/aspnet/Routing/blob/dev/src/Microsoft.AspNetCore.Routing/RouteCollection.cs
    // AttributeRoute: https://github.com/aspnet/Mvc/blob/dev/src/Microsoft.AspNetCore.Mvc.Core/Internal/AttributeRoute.cs
    // TreeRouter: https://github.com/aspnet/Routing/blob/dev/src/Microsoft.AspNetCore.Routing/Tree/TreeRouter.cs
    public class LocalizedHtmlGenerator : DefaultHtmlGenerator
    {
        public LocalizedHtmlGenerator(
            IAntiforgery antiforgery,
            IOptions<MvcViewOptions> optionsAccessor,
            IModelMetadataProvider metadataProvider,
            IUrlHelperFactory urlHelperFactory,
            HtmlEncoder htmlEncoder,
            ValidationHtmlAttributeProvider validationAttributeProvider)
            : base(antiforgery, optionsAccessor, metadataProvider, urlHelperFactory, htmlEncoder, validationAttributeProvider)
        {
            
        }

        
        //public override TagBuilder GenerateActionLink(ViewContext viewContext, string linkText, string actionName, string controllerName, string protocol, string hostname, string fragment, object routeValues, object htmlAttributes)
        //{

            // This calls the following functions:
            // var urlHelper = _urlHelperFactory.GetUrlHelper(viewContext);
            // var url = urlHelper.Action(actionName, controllerName, routeValues, protocol, hostname, fragment)
            // GenerateLink(linkText, url, htmlAttributes)
            //return base.GenerateActionLink(viewContext, linkText, actionName, controllerName, protocol, hostname, fragment, routeValues, htmlAttributes);
        //}

        // This is for asp-route
        //public override TagBuilder GenerateRouteLink(ViewContext viewContext, string linkText, string routeName, string protocol, string hostName, string fragment, object routeValues, object htmlAttributes)
        //{            
        //    return base.GenerateRouteLink(viewContext, linkText, routeName, protocol, hostName, fragment, routeValues, htmlAttributes);
        //}        
    }
}

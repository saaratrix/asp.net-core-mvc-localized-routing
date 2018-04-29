namespace localization.Localization.CultureRouteData
{
    /// <summary>
    /// The route data for each culture for an action in CultureActionData.
    /// </summary>
    public class CultureUrlRouteData
    {
        public readonly string Route;
        /// <summary>
        /// The innerText for an anchor tag when you use the anchor tag helper.
        /// <a>Link</a>
        /// </summary>
        public readonly string Link;     

        public CultureUrlRouteData(string route, string link)
        {
            Route = route;
            Link = link;
        }
    }
}

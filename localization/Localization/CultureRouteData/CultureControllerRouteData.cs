using System.Collections.Concurrent;

namespace localization.Localization.CultureRouteData
{
    /// <summary>
    /// The controller data for cultures stored in LocalizationRouteDataHandler.
    /// </summary>
    public class CultureControllerRouteData
    {
        /// <summary>
        /// Different names of the controller in different cultures.
        /// The name is used when constructing the localized Url.
        /// Example:
        /// en: Home
        /// fi: fi/koti
        /// </summary>
        public ConcurrentDictionary<string, string> Names { get; }

        /// <summary>
        /// The actions for the controller.
        /// </summary>
        public ConcurrentDictionary<string, CultureActionRouteData> Actions { get; }

        public CultureControllerRouteData()
        {
            Names = new ConcurrentDictionary<string, string>();
            Actions = new ConcurrentDictionary<string, CultureActionRouteData>();
        }
    }
}

using System.Collections.Concurrent;
using System.Collections.Generic;

namespace localization.Localization.CultureRouteData
{
    /// <summary>
    /// The action data for the cultures stored in a CultureControllerData object. 
    /// </summary>
    public class CultureActionRouteData
    {
        /// <summary>
        /// Different action names in different cultures.
        /// </summary>
        public ConcurrentDictionary<string, CultureUrlRouteData> UrlData { get; }

        /// <summary>
        /// The parameters by name sorted in order.
        /// Example Controller/Action/{first}/{second}
        /// [0]: first
        /// [1]: second
        /// </summary>
        public readonly string[] ParametersData;

        public CultureActionRouteData(List<string> parametersData)
        {
            UrlData = new ConcurrentDictionary<string, CultureUrlRouteData>();            
            // If the parameters data has any entries then convert it to a read only list.
            if (parametersData.Count > 0)
            {
                ParametersData = parametersData.ToArray();
            }
            else
            {
                ParametersData = null;
            }            
        }
    }
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace localization.Localization
{
    public class CultureActionData
    {
        /// <summary>
        /// Different names in different cultures
        /// </summary>
        public ConcurrentDictionary<string, CultureUrlData> UrlData { get; }

        /// <summary>
        /// The parameters by name sorted in order
        /// </summary>
        public readonly string[] ParametersData;

        public CultureActionData(List<string> a_parametersData)
        {
            UrlData = new ConcurrentDictionary<string, CultureUrlData>();
            // If the parameters data has any entries then convert it to a read only list
            if (a_parametersData.Count > 0)
            {
                ParametersData = a_parametersData.ToArray();
            }
            else
            {
                ParametersData = null;
            }            
        }
    }
}

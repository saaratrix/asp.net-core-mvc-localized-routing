using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

        public CultureActionData()
        {
            UrlData = new ConcurrentDictionary<string, CultureUrlData>();
        }
    }
}

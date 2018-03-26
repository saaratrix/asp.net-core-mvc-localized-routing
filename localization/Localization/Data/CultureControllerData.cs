using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace localization.Localization
{
    public class CultureControllerData
    {
        /// <summary>
        /// Different names of the controller in different cultures
        /// </summary>
        public ConcurrentDictionary<string, string> Names { get; }

        /// <summary>
        /// The actions in the default culture
        /// </summary>
        public ConcurrentDictionary<string, CultureActionData> Actions { get; }

        public CultureControllerData()
        {
            Names = new ConcurrentDictionary<string, string>();
            Actions = new ConcurrentDictionary<string, CultureActionData>();
        }
    }
}

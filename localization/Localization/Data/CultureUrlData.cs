using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace localization.Localization
{
    public class CultureUrlData
    {
        public string Route { get; set; }
        public string Link { get; set; }        

        public CultureUrlData(string route, string link)
        {
            Route = route;
            Link = link;
        }
    }
}

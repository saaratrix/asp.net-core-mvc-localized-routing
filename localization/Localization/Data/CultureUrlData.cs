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

        public CultureUrlData(string a_route, string a_link)
        {
            Route = a_route;
            Link = a_link;
        }
    }
}

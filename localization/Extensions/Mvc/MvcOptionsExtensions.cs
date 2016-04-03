using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using localization.Localization;

namespace localization.Extensions.Mvc
{
    public static class MvcOptionsExtensions
    {
        public static void AddLocalizedRoutes(this MvcOptions options)
        {
            options.Conventions.Insert(0, new LocalizedRouteConvention());
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace localization.Models.Local
{
    public class ParameterViewModel
    {
        public int Index { get; set; }

        [Display(Name = "Test parameter!")]
        public string Test { get; set; }
    }
}

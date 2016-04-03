using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace localization.Entities
{
    public partial class Page
    {
        [Key]
        public string Id { get; set; }
    }
}

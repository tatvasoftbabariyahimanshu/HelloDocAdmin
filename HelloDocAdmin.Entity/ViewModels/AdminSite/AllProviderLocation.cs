using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocAdmin.Entity.ViewModels.AdminSite
{
    public class AllProviderLocation
    {

        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string Name { get; set; }

        public string Address { get; set; }
    }
}

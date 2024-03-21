using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocAdmin.Entity.ViewModels.AdminSite
{
    public class VendorListView
    {
        public int? VendorID { get; set; }   
        public string VendorName { get; set;}
        public string Profession { get; set;}
        public string Email { get; set; }
        public string FaxNumber { get; set; }

        public string PhoneNumber { get; set; }
        public string BusinessContact { get; set; }
    }
}

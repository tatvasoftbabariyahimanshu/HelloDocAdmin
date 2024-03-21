using HelloDocAdmin.Entity.Models;
using HelloDocAdmin.Entity.ViewModels.AdminSite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocAdmin.Repositories.Interface
{
    public interface IVendorRepository
    {
        public List<VendorListView> getallvendor();
        public Healthprofessional gethelthprofessionaldetails(int vendorid);
        public bool addVendor(Healthprofessional model);
        public bool EditVendor(Healthprofessional model);
        public bool delete(int? vendorid);
    }
}

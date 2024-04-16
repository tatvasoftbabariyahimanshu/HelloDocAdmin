using HelloDocAdmin.Entity.Models;
using HelloDocAdmin.Entity.ViewModels.AdminSite;

namespace HelloDocAdmin.Repositories.Interface
{
    public interface IVendorRepository
    {
        public VendorData getallvendor(string? vendorname, int? helthprofessionaltype, int pagesize = 5, int currentpage = 1);
        public Healthprofessional gethelthprofessionaldetails(int vendorid);
        public int isBusinessNameExist(string businessName);
        public bool addVendor(Healthprofessional model);
        public bool EditVendor(Healthprofessional model);
        public bool delete(int? vendorid);
        public int isEmailExist(string Email);
    }
}

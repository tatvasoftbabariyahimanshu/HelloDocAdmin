using HelloDocAdmin.Entity.Models;
using HelloDocAdmin.Entity.ViewModels.AdminSite;

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

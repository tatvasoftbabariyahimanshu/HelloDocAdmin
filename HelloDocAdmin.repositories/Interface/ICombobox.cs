using HelloDocAdmin.Entity.Models;
using HelloDocAdmin.Entity.ViewModel;
using HelloDocAdmin.Entity.ViewModels;

namespace HelloDocAdmin.Repositories.Interface
{
    public interface ICombobox
    {
        Task<List<RegionComboBox>> RegionComboBox();
        Task<List<CaseReasonComboBox>> CaseReasonComboBox();
        public List<Physician> ProviderbyRegion(int? regionid);

        public Task<List<HealthprofessionalCombobox>> healthprofessionals();
        public List<HealthprofessionalCombobox> ProfessionalByType(int? HealthprofessionalID);
        public Task<List<HealthprofessionaltypeCombobox>> healthprofessionaltype();
        public Task<List<UserRoleCombobox>> UserRole();
        public Task<List<RoleComboBox>> RolelistAdmin();
        public Task<List<RoleComboBox>> RolelistProvider();
    }
}

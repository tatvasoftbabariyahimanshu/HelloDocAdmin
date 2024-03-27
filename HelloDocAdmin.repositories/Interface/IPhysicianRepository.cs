using HelloDocAdmin.Entity.ViewModels;
using HelloDocAdmin.Entity.ViewModels.AdminSite;

namespace HelloDocAdmin.Repositories.Interface
{
    public interface IPhysicianRepository
    {
        Task<List<PhysiciansViewModel>> PhysicianAll();
        Task<List<PhysiciansViewModel>> PhysicianByRegion(int? region);
        Task<bool> ChangeNotificationPhysician(Dictionary<int, bool> changedValuesDict);
        public Task<bool> EditAccountInfo(PhysiciansViewModel vm);
        public Task<bool> ResetPassword(int Physicianid, string Password);
        public Task<bool> PhysicianAddEdit(PhysiciansViewModel physiciandata, string AdminId);
        public Task<bool> EditAdminInfo(PhysiciansViewModel vm);
        public Task<bool> EditMailBilling(PhysiciansViewModel vm);
        public Task<PhysiciansViewModel> GetPhysicianById(int id);
        public Task<bool> EditProviderProfile(PhysiciansViewModel vm, string AdminId);
        public Task<bool> DeletePhysician(int PhysicianID, string AdminID);

        public Task<List<Schedule>> GetShift(int month);
        public Task<bool> CreateShift(Schedule v, string id);
        public Task<Schedule> GetShiftByShiftdetailId(int Shiftdetailid);
        public Task<bool> EditShift(Schedule s, string AdminID);
        public Task<bool> UpdateStatusShift(string s, string AdminID);
        public Task<bool> DeleteShift(string s, string AdminID);
    }
}

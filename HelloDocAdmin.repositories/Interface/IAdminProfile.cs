using HelloDocAdmin.Entity.ViewModels.AdminSite;

namespace HelloDocAdmin.Repositories.Interface
{
    public interface IAdminProfile
    {
        public int isEmailExist(string Email);
        public ViewAdminProfileModel GetDetailsForAdminProfile(string id);
        public bool Edit_Admin_Profile(ViewAdminProfileModel vm, string id);
        public bool Edit_Billing_Info(ViewAdminProfileModel vm, string id);
        public bool ChangePassword(string password, string id);
        public bool AddAdmin(ViewAdminProfileModel data, string id);
        public bool EditAdmin(ViewAdminProfileModel data, string id);
    }
}

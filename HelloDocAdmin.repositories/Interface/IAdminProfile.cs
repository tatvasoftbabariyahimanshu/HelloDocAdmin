using HelloDocAdmin.Entity.ViewModels.AdminSite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocAdmin.Repositories.Interface
{
    public interface IAdminProfile
    {

        public ViewAdminProfileModel GetDetailsForAdminProfile(string id);
        public bool Edit_Admin_Profile(ViewAdminProfileModel vm, string id);
        public bool Edit_Billing_Info(ViewAdminProfileModel vm , string id);
        public bool ChangePassword(string password, string id);
        public bool AddAdmin(ViewAdminProfileModel data, string id);
        public bool EditAdmin(ViewAdminProfileModel data, string id);
    }
}

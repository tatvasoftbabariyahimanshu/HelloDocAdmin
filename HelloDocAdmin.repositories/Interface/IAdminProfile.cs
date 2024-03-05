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

        public ViewAdminProfileModel GetDetailsForAdminProfile();
        public bool Edit_Admin_Profile(ViewAdminProfileModel vm);
        public bool Edit_Billing_Info(ViewAdminProfileModel vm);
        public bool ChangePassword(string password);
    }
}

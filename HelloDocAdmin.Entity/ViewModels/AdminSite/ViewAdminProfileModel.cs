using HelloDocAdmin.Entity.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocAdmin.Entity.ViewModels.AdminSite
{
    public class ViewAdminProfileModel
    {
        public string? AspnetUserID { get; set; }

        public string ASP_UserName { get; set; }
        public string? ASP_Password { get; set; }

        public short? ASP_Status { get; set; }
        public int? ASP_RoleID { get; set; }
        public string User_FirtName { get; set; }
        public string User_LastName{ get; set; }
        public string User_Email { get; set; }
        public string User_PhoneNumber{ get; set; }

        public List<int>? AdminReqionList { get; set; }
        public int RegionID { get; set; }   

        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }

        public string zip { get; set; }



    }
}

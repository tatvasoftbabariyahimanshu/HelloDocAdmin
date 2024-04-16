using System.ComponentModel.DataAnnotations;

namespace HelloDocAdmin.Entity.ViewModels.AdminSite
{
    public class ViewAdminProfileModel
    {
        public string? AspnetUserID { get; set; }

        public string ASP_UserName { get; set; }
        public string? ASP_Password { get; set; }

        public short ASP_Status { get; set; }
        public int ASP_RoleID { get; set; }
        [Display(Name = "First Name")]
        public string User_FirtName { get; set; }
        [Display(Name = "Last Name")]
        public string User_LastName { get; set; }
        [Display(Name = "Email")]
        [EmailAddress]
        public string User_Email { get; set; }
        [Display(Name = "Phone Number")]
        [Required(ErrorMessage = "Phone Number Required!")]
        [RegularExpression(@"^\d{10}$",
                   ErrorMessage = "Entered phone format is not valid.")]
        public string User_PhoneNumber { get; set; }


        public List<int>? AdminReqionList { get; set; }
        [Required(ErrorMessage = "Select Region!")]
        public int RegionID { get; set; }
        [Display(Name = "Address")]
        [Required(ErrorMessage = "Enter Address!")]
        public string Address1 { get; set; }
        [Display(Name = "Address ")]
        public string Address2 { get; set; }
        [Display(Name = "City ")]
        [Required(ErrorMessage = "Enter City!")]
        public string City { get; set; }
        [Display(Name = "zip")]
        [RegularExpression(@"^\d{6}$",
                   ErrorMessage = "Entered Valid Zip Code.")]
        public string? zip { get; set; }



    }
}

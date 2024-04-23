using System.ComponentModel.DataAnnotations;

namespace HelloDocAdmin.Entity.ViewModels.AdminSite
{
    public class ViewCloseCaseModel
    {
        public List<Documents> documentslist { get; set; } = null;
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
        public string Firstname { get; set; }
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
        public string Lastname { get; set; }
        public string ConfirmationNumber { get; set; }
        public int RequestID { get; set; }
        public int RequestWiseFileID { get; set; }
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]

        public string RC_FirstName { get; set; }
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
        public string RC_LastName { get; set; }
        public string RC_Email { get; set; }
        public DateTime RC_Dob { get; set; }
        public string RC_PhoneNumber { get; set; }

        public int RequestClientID { get; set; }



    }
}

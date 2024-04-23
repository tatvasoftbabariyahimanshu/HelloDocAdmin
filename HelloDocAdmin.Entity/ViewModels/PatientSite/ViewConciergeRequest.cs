using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace HelloDocAdmin.Entity.ViewModel.PatientSite
{
    public class ViewConciergeRequest
    {
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
        public string CON_FirstName { get; set; }
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
        public string CON_LastName { get; set; }

        public string CON_PhoneNumber { get; set; }
        [Required(ErrorMessage = "Email Is Required!")]
        [EmailAddress(ErrorMessage = "Please Enter Valid Email Address!")]
        public string CON_Email { get; set; }

        public string? CON_PropertyName { get; set; }
        public string? Id { get; set; } = null!;
        public string? Symptoms { get; set; }
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
        public string FirstName { get; set; }
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        [Required(ErrorMessage = "Email Is Required!")]
        [EmailAddress(ErrorMessage = "Please Enter Valid Email Address!")]
        public string Email { get; set; }

        public string PhoneNumber { get; set; }
        public string CON_Street { get; set; }
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Use letters only please")]
        public string CON_City { get; set; }
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
        public string CON_State { get; set; }
        [RegularExpression(@"^\d{6}$",
              ErrorMessage = "Entered Valid Zip Code.")]
        public string CON_ZipCode { get; set; }
        public string? RoomSite { get; set; }
        public IFormFile? UploadFile { get; set; }
        public string? UploadImage { get; set; }
    }
}

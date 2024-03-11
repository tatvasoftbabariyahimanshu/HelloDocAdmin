using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace HelloDocAdmin.Entity.ViewModel.PatientSite
{
    public class ViewPatientRequest
    {
        public string? Id { get; set; } = null!;
        public string? Symptoms { get; set; }

        [Required(ErrorMessage = "First Name Is Required!")]
        public string FirstName { get; set; }

   
        public string? Password { get; set; }
       
        
        public string LastName { get; set; }
       
        
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage ="Email Is Required!")]
        [EmailAddress(ErrorMessage ="Please Enter Valid Email Address!")]
        public string Email { get; set; }
        [Phone(ErrorMessage = "Please Enter Valid Phone Number!")]
        public string PhoneNumber { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string? RoomSite { get; set; }

       public int? Userid { get; set; }
        public IFormFile? UploadFile { get; set; } 
        public  string? UploadImage { get; set; }
    }
}

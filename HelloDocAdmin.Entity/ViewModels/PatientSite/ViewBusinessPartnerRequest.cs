using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace HELLO_DOC.Models
{
    public class ViewBusinessPartnerRequest
    {
        public string BUP_FirstName { get; set; }
        public string BUP_LastName { get; set; }
        public string BUP_PhoneNumber { get; set; }
        [Required(ErrorMessage = "Email Is Required!")]
        [EmailAddress(ErrorMessage = "Please Enter Valid Email Address!")]
        public string BUP_Email { get; set; }
        public string? BUP_PropertyName { get; set; }
        public string? BUP_CaseNumber { get; set; }
        public string? Id { get; set; } = null!;
        public string? Symptoms { get; set; }
        public string FirstName { get; set; }
   
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        [Required(ErrorMessage = "Email Is Required!")]
        [EmailAddress(ErrorMessage = "Please Enter Valid Email Address!")]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string? RoomSite { get; set; }
        public IFormFile? UploadFile { get; set; }
        public string? UploadImage { get; set; }
    }
}

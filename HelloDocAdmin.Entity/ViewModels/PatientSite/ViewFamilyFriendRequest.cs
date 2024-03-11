﻿using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace HelloDocAdmin.Entity.ViewModel.PatientSite
{
    public class ViewFamilyFriendRequest
    {
       
        public string FF_FirstName { get; set; }
        public string FF_LastName { get; set; }
        public string FF_PhoneNumber { get; set; }
        [Required(ErrorMessage = "Email Is Required!")]
        [EmailAddress(ErrorMessage = "Please Enter Valid Email Address!")]
        public string FF_Email { get; set; }
        public string? FF_RelationWithPatient { get; set; }
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


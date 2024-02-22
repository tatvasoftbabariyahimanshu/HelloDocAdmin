﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocAdmin.Entity.ViewModels.AdminSite
{
    public class ViewCaseModel
    {
        public int RequestID { get; set; }
        public string? PatientNotes { get; set; }
        public string? ConfirmationNumber { get; set; } 
        public string FirstName { get; set;}
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime  Dob { get; set; }
        [Required(ErrorMessage = "Email Is Required!")]
        [EmailAddress(ErrorMessage = "Please Enter Valid Email Address!")]
        public string Email { get; set; }   
        public string? Region { get; set; }
        public string? Address { get; set; }
        public string? Room { get; set; }
        public int RequestTypeID { get; set; }
    }
}

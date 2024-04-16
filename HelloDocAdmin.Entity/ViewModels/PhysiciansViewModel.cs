using Microsoft.AspNetCore.Http;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace HelloDocAdmin.Entity.ViewModels
{
    public class PhysiciansViewModel
    {
        public int? notificationid { get; set; }
        public BitArray? notification { get; set; }
        public string? role { get; set; }
        public int? Physicianid { get; set; }

        public string? Aspnetuserid { get; set; }
        [Required(ErrorMessage = "Enter UserName")]
        public string UserName { get; set; }
        public string? PassWord { get; set; }
        public string? Regionsid { get; set; }

        [Required(ErrorMessage = "Enter First Name")]
        public string Firstname { get; set; }
        [Required(ErrorMessage = "Enter Last Name")]
        public string Lastname { get; set; }
        [Required(ErrorMessage = "Enter Valid Email")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "Phone Number Required!")]
        [RegularExpression(@"^\d{10}$",
                  ErrorMessage = "Entered phone format is not valid.")]
        public string Mobile { get; set; }

        public string? State { get; set; }
        [Display(Name = "zip")]
        [RegularExpression(@"^\d{6}$",
                   ErrorMessage = "Entered Valid Zip Code.")]
        public string? Zipcode { get; set; }

        public string? Medicallicense { get; set; }

        public string? Photo { get; set; }



        [MaxFileSize(5 * 500 * 500)]

        [AllowedExtensions(new string[] { ".jpg", ".png" })]
        public IFormFile? PhotoFile { get; set; }





        public string? Adminnotes { get; set; }



        public bool Isagreementdoc { get; set; }

        public bool Isbackgrounddoc { get; set; }

        public bool Istrainingdoc { get; set; }

        public bool Isnondisclosuredoc { get; set; }
        public bool Islicensedoc { get; set; }

        [Required(ErrorMessage = "write your Address!!")]
        public string Address1 { get; set; }

        public string? Address2 { get; set; }
        [Required(ErrorMessage = "Enter City!!")]
        public string City { get; set; }
        [Required(ErrorMessage = "Select Region!!")]
        public int Regionid { get; set; }


        public string? Altphone { get; set; }

        public string? Createdby { get; set; } = null!;

        public DateTime? Createddate { get; set; }

        public string? Modifiedby { get; set; }

        public DateTime? Modifieddate { get; set; }

        public short? Status { get; set; }
        [Required(ErrorMessage = "Enter Business Name!!")]
        public string Businessname { get; set; }
        [Required(ErrorMessage = "Enter Bussiness Site!!")]
        public string Businesswebsite { get; set; }

        public BitArray? Isdeleted { get; set; }
        [Required(ErrorMessage = "select Physician Role...!!")]
        public int Roleid { get; set; }

        public string? Npinumber { get; set; }


        public string? Signature { get; set; }
        public IFormFile? SignatureFile { get; set; }

        public BitArray? Iscredentialdoc { get; set; }

        public BitArray? Istokengenerate { get; set; }

        public string? Syncemailaddress { get; set; }

        [AllowedExtensions(new string[] { ".pdf" })]
        public IFormFile? Agreementdoc { get; set; }
        [AllowedExtensions(new string[] { ".pdf" })]
        public IFormFile? NonDisclosuredoc { get; set; }
        [AllowedExtensions(new string[] { ".pdf" })]
        public IFormFile? Trainingdoc { get; set; }
        [AllowedExtensions(new string[] { ".pdf" })]
        public IFormFile? BackGrounddoc { get; set; }
        [AllowedExtensions(new string[] { ".pdf" })]
        public IFormFile? Licensedoc { get; set; }
        public List<Regions>? Regionids { get; set; } = null;
        public class Regions
        {
            public int? regionid { get; set; }
            public string? regionname { get; set; }

        }
        public class MaxFileSizeAttribute : ValidationAttribute
        {
            private readonly int _maxFileSize;
            public MaxFileSizeAttribute(int maxFileSize)
            {
                _maxFileSize = maxFileSize;
            }

            protected override ValidationResult IsValid(
            object value, ValidationContext validationContext)
            {
                var file = value as IFormFile;
                if (file != null)
                {
                    if (file.Length > _maxFileSize)
                    {
                        return new ValidationResult(GetErrorMessage());
                    }
                }

                return ValidationResult.Success;
            }

            public string GetErrorMessage()
            {
                return $"Maximum allowed file size is {_maxFileSize} bytes.";
            }
        }
        public class AllowedExtensionsAttribute : ValidationAttribute
        {
            private readonly string[] _extensions;
            public AllowedExtensionsAttribute(string[] extensions)
            {
                _extensions = extensions;
            }

            protected override ValidationResult IsValid(
            object value, ValidationContext validationContext)
            {
                var file = value as IFormFile;
                if (file != null)
                {
                    var extension = Path.GetExtension(file.FileName);
                    if (!_extensions.Contains(extension.ToLower()))
                    {
                        return new ValidationResult(GetErrorMessage());
                    }
                }

                return ValidationResult.Success;
            }

            public string GetErrorMessage()
            {
                return $"This photo extension is not allowed!";
            }
        }
    }
}

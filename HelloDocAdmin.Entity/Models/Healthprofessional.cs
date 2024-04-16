using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace HelloDocAdmin.Entity.Models;

public partial class Healthprofessional
{
    public int? Vendorid { get; set; }
    [Required(ErrorMessage = "Enter Vendor Name!!")]
    public string Vendorname { get; set; } = null!;

    [Required(ErrorMessage = "Select Profession!!")]
    public int Profession { get; set; }
    [Required(ErrorMessage = "Enter Fax Number!!")]
    public string Faxnumber { get; set; } = null!;

    public string? Address { get; set; }
    [Required(ErrorMessage = "Enter City!!")]
    public string City { get; set; }
    [Required(ErrorMessage = "Enter State!!")]
    public string State { get; set; }
    [Required(ErrorMessage = "Enter Zip Code!!")]
    public string Zip { get; set; }

    public int? Regionid { get; set; }

    public DateTime Createddate { get; set; }

    public DateTime? Modifieddate { get; set; }
    [Required(ErrorMessage = "Enter Phone Number!!")]
    public string? Phonenumber { get; set; }

    public BitArray? Isdeleted { get; set; }

    public string? Ip { get; set; }
    [EmailAddress(ErrorMessage = "Enter Valid Email Address")]
    [Required(ErrorMessage = "Enter Email!!")]
    public string Email { get; set; }

    public string? Businesscontact { get; set; }

    public virtual Healthprofessionaltype? ProfessionNavigation { get; set; }
}

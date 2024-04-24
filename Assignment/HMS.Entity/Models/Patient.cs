using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace HMS.Entity.Models;

public partial class Patient
{
    public int Id { get; set; }

    [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
    public string? FirstName { get; set; } = null!;

    [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
    public string? LastName { get; set; }

    public int? DoctorId { get; set; }
    [Range(1, 100)]

    public int? Age { get; set; }
    [EmailAddress(ErrorMessage = "Please Enter Valid Email Address!")]
    public string? Email { get; set; } = null!;

    [RegularExpression(@"^\d{10}$",
                   ErrorMessage = "Entered Valid mobile number.")]
    public string? PhoneNo { get; set; } = null!;

    public int Gender { get; set; }


    public string? Disease { get; set; } = null!;

    public string? Specialist { get; set; }
    public BitArray? IsDeleted { get; set; }

    public virtual Doctor Doctor { get; set; } = null!;
}

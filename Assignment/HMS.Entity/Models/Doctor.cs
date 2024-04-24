using System;
using System.Collections.Generic;

namespace HMS.Entity.Models;

public partial class Doctor
{
    public int DoctorId { get; set; }

    public string? Specialist { get; set; }

    public virtual ICollection<Patient> Patients { get; } = new List<Patient>();
}

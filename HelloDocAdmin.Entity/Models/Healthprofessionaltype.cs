using System;
using System.Collections;
using System.Collections.Generic;

namespace HelloDocAdmin.Entity.Models;

public partial class Healthprofessionaltype
{
    public int Healthprofessionalid { get; set; }

    public string Professionname { get; set; } = null!;

    public DateTime Createddate { get; set; }

    public BitArray? Isactive { get; set; }

    public BitArray? Isdeleted { get; set; }

    public virtual ICollection<Healthprofessional> Healthprofessionals { get; } = new List<Healthprofessional>();
}

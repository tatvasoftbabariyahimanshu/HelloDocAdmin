using System;
using System.Collections.Generic;

namespace HelloDocAdmin.Entity.Models;

public partial class Region
{
    public int Regionid { get; set; }

    public string Name { get; set; } = null!;

    public string? Abbreviation { get; set; }

    public virtual ICollection<Adminregion> Adminregions { get; } = new List<Adminregion>();

    public virtual ICollection<Concierge> Concierges { get; } = new List<Concierge>();

    public virtual ICollection<Physicianregion> Physicianregions { get; } = new List<Physicianregion>();

    public virtual ICollection<Physician> Physicians { get; } = new List<Physician>();

    public virtual ICollection<Requestclient> Requestclients { get; } = new List<Requestclient>();

    public virtual ICollection<Shiftdetailregion> Shiftdetailregions { get; } = new List<Shiftdetailregion>();
}

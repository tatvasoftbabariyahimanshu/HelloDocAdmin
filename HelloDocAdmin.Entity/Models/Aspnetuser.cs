using System;
using System.Collections.Generic;

namespace HelloDocAdmin.Entity.Models;

public partial class Aspnetuser
{
    public string Id { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string? Passwordhash { get; set; }

    public string? Email { get; set; }

    public string? Phonenumber { get; set; }

    public DateTime CreatedDate { get; set; }

    public string? Ip { get; set; }
    public string? pwdModified { get; set; }

    public DateTime? Modifieddate { get; set; }

    public virtual Aspnetuserrole? Aspnetuserrole { get; set; }

    public virtual ICollection<Business> BusinessCreatedbyNavigations { get; } = new List<Business>();

    public virtual ICollection<Business> BusinessModifiedbyNavigations { get; } = new List<Business>();

    public virtual ICollection<Physician> PhysicianAspnetusers { get; } = new List<Physician>();

    public virtual ICollection<Physician> PhysicianCreatedbyNavigations { get; } = new List<Physician>();

    public virtual ICollection<Physician> PhysicianModifiedbyNavigations { get; } = new List<Physician>();

    public virtual ICollection<Shiftdetail> Shiftdetails { get; } = new List<Shiftdetail>();

    public virtual ICollection<Shift> Shifts { get; } = new List<Shift>();

    public virtual ICollection<User> Users { get; } = new List<User>();
}

﻿using System;
using System.Collections.Generic;

namespace HelloDocAdmin.Entity.Models;

public partial class Concierge
{
    public int Conciergeid { get; set; }

    public string Conciergename { get; set; } = null!;

    public string? Address { get; set; }

    public string Street { get; set; } = null!;

    public string City { get; set; } = null!;

    public string State { get; set; } = null!;

    public string Zipcode { get; set; } = null!;

    public DateTime Createddate { get; set; }

    public int Regionid { get; set; }

    public string? Ip { get; set; }

    public virtual Region Region { get; set; } = null!;

    public virtual ICollection<Requestconcierge> Requestconcierges { get; } = new List<Requestconcierge>();
}

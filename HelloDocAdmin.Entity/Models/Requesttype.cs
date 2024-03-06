using System;
using System.Collections.Generic;

namespace HelloDocAdmin.Entity.Models;

public partial class Requesttype
{
    public int Requesttypeid { get; set; }

    public string Name { get; set; } = null!;
}

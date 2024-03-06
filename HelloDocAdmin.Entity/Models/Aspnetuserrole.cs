using System;
using System.Collections.Generic;

namespace HelloDocAdmin.Entity.Models;

public partial class Aspnetuserrole
{
    public string Userid { get; set; } = null!;

    public string Roleid { get; set; } = null!;

    public virtual Aspnetrole Role { get; set; } = null!;

    public virtual Aspnetuser User { get; set; } = null!;
}

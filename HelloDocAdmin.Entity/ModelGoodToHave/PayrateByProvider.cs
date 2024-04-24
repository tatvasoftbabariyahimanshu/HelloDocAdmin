using System;
using System.Collections.Generic;

namespace HelloDocAdmin.Entity.ModelGoodToHave;

public partial class PayrateByProvider
{
    public int PayrateId { get; set; }

    public int? CategoryId { get; set; }

    public int? PhysicianId { get; set; }

    public decimal? Payrate { get; set; }

    public string? CreatedBy { get; set; }

    public TimeOnly[]? CreatedDate { get; set; }

    public string? ModifiedBy { get; set; }

    public string? ModifiedDate { get; set; }

    public virtual PayrateCategory? Category { get; set; }
}

using System;
using System.Collections.Generic;

namespace HelloDocAdmin.Entity.ModelGoodToHave;

public partial class PayrateCategory
{
    public int PayrateCategoryId { get; set; }

    public string? CategoryName { get; set; }

    public virtual ICollection<PayrateByProvider> PayrateByProviders { get; } = new List<PayrateByProvider>();
}

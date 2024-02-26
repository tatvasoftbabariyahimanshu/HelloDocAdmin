using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocAdmin.Entity.ViewModels.AdminSite
{
    public class PartialCancelCase
    {
        public int RequestId { get; set; }
        public string CaseTag { get; set; }

        public string Note { get; set; }
    }
}

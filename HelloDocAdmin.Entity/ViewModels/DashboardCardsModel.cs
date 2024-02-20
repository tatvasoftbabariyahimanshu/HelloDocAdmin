using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocAdmin.Entity.ViewModels
{
    public  class DashboardCardsModel
    {
        public int? NewRequests { get; set; }
        public int? PandingRequests { get; set; }
        public int? ActiveRequests { get; set; }
        public int? ConcludeRequests { get; set; }
        public int? ToCloseRequests { get; set; }
        public int? UnpaidRequests { get; set; }
    }
}

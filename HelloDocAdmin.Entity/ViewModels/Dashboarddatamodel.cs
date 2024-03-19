using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocAdmin.Entity.ViewModels
{
    public class Dashboarddatamodel
    {
        public List<DashboardRequestModel> requestList { get; set; }
        
        public int TotalPage { get; set; }

        public int CurrentPage { get; set; }

        public int pageSize { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocAdmin.Entity.ViewModels.Authentication
{
    public class CookieModel
    {
        public string AspNetUserID { get; set; }
        public string role { get; set; }
        public string UserName { get; set; }
    }
}

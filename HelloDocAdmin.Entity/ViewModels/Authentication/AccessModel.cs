using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocAdmin.Entity.ViewModels.Authentication
{
    public class AccessModel
    {
        public string AspUserID { get; set; }
        public string AspUserName { get;}

        public string AspEmail { get; set;}

        public string AspUserRole { get; set;}
        public int AspRoleId { get; set; }
    }
}

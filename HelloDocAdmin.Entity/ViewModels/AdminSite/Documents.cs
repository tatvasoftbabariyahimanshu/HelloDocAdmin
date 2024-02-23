using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocAdmin.Entity.ViewModels.AdminSite
{
    public class Documents
    {
         public string? Uploader { get; set; }
        public string? Filename { get; set; }
        public DateTime Createddate { get; set; }
     
    }
}

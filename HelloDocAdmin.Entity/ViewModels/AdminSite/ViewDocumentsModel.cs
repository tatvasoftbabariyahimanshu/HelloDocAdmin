using HelloDocAdmin.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocAdmin.Entity.ViewModels.AdminSite
{
    public class ViewDocumentsModel
    {
       public List<Documents> documentslist { get; set; } = null;
      public string Firstanme { get; set; }
        public string Lastanme { get; set;}
        public string ConfirmationNumber { get; set; }   
        public int RequestID { get; set; }
        public int RequestWiseFileID { get; set; }
    }
}

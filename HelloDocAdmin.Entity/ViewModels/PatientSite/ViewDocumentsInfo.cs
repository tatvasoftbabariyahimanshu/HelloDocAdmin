using HelloDocAdmin.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocAdmin.Entity.ViewModels.PatientSite
{
    public class ViewDocumentsInfo
    {
        public List<Request> requests { get; set; }

        public List<Requestwisefile> requestswisefile { get; set; }


    }
}

using HelloDocAdmin.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocAdmin.PatientRepositories.Interface
{
    public interface IAspnetuserRepository
    {
        public Aspnetuser Aspnetuserbymail(string? Email);
    }
}

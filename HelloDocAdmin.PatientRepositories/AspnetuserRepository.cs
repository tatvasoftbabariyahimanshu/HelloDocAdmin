using HelloDocAdmin.Entity.Data;
using HelloDocAdmin.Entity.Models;
using HelloDocAdmin.Entity.ViewModels;
using HelloDocAdmin.PatientRepositories.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace HelloDocAdmin.PatientRepositories
{
    public class AspnetuserPropsitory : IAspnetuserRepository
    {
        private readonly ApplicationDbContext _context;

        public AspnetuserPropsitory(ApplicationDbContext context) { _context = context; }



        public Aspnetuser Aspnetuserbymail(string? Email)
        {
            var aspnetuser = _context.Aspnetusers.SingleOrDefault(x => x.Email == Email);
            return aspnetuser;
        }
    
    }
}

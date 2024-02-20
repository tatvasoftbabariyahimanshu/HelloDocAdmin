using HelloDocAdmin.Entity.Data;
using HelloDocAdmin.Entity.Models;
using HelloDocAdmin.Entity.ViewModels;
using HelloDocAdmin.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace HelloDocAdmin.Repositories
{
   
    public class DashboardRepository:IDashboardRepository
    {
        private readonly ApplicationDbContext _context;


        public DashboardRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public int GetRequestNumberByStatus(short status)
        {
            int n= _context.Requests.Where(E=>E.Status== status).Count();
            return n;
        }

        public List<DashboardRequestModel> GetRequests(short Status)
        {



            var allData = (from req in _context.Requests
                           join reqClient in _context.Requestclients
                           on req.Requestid equals reqClient.Requestid into reqClientGroup
                           from rc in reqClientGroup.DefaultIfEmpty()
                           join phys in _context.Physicians
                           on req.Physicianid equals phys.Physicianid into physGroup
                           from p in physGroup.DefaultIfEmpty()
                           where req.Status == Status
                           select new DashboardRequestModel
                           {
                               RequestTypeID = req.Requesttypeid,
                               Requestor = req.Firstname + " " + req.Lastname,
                               PatientName = rc.Firstname +" "+ rc.Lastname,
                               Dob = new DateOnly((int)rc.Intyear, DateTime.ParseExact(rc.Strmonth, "MMMM", new CultureInfo("en-US")).Month, (int)rc.Intdate),
                               RequestedDate = req.Createddate,
                               Email = rc.Email,
                               PhoneNumber = rc.Phonenumber,
                               Address = rc.Address + "," + rc.Street + "," + rc.City + "," + rc.State + "," + rc.Zipcode,
                               Notes = rc.Notes,
                               ProviderID = req.Physicianid,

                               RequestorPhoneNumber = req.Phonenumber



                           }).ToList();




            return (List<DashboardRequestModel>)allData;
        }


    }
}

using HelloDocAdmin.Entity.Data;
using HelloDocAdmin.Entity.Models;
using HelloDocAdmin.Entity.ViewModels;
using HelloDocAdmin.Entity.ViewModels.AdminSite;
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
        public ViewCaseModel GetRequestForViewCase(int id)
        {
            var  n = _context.Requests.FirstOrDefault(E => E.Requestid == id);

            var l = _context.Requestclients.FirstOrDefault(E => E.Requestid == id);

            var region = _context.Regions.FirstOrDefault(E => E.Regionid == l.Regionid);

            ViewCaseModel requestforviewcase = new ViewCaseModel
            {
                RequestID = id,
                Region=region.Name,
                FirstName = l.Firstname,
                LastName = l.Lastname,
                PhoneNumber=l.Phonenumber,
                PatientNotes = l.Notes,
                Email = l.Email,
                RequestTypeID=n.Requesttypeid,
                Address = l.Street+"," + l.City+"," + l.State,
                Room=l.Address,
                ConfirmationNumber=n.Confirmationnumber,
                Dob = new DateTime((int)l.Intyear, DateTime.ParseExact(l.Strmonth, "MMMM", new CultureInfo("en-US")).Month, (int)l.Intdate)
            };
            return requestforviewcase;
        }
        public bool EditCase(ViewCaseModel model)
        {
            try

            {
                int monthnum = model.Dob.Month;
                string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(monthnum);
                int date = model.Dob.Day;
                int year = model.Dob.Year;
                Requestclient client=_context.Requestclients.FirstOrDefault(E=>E.Requestid==model.RequestID);

               if (client != null)
                {

                    client.Firstname = model.FirstName;
                    client.Lastname = model.LastName;
                    client.Phonenumber = model.PhoneNumber;
                    client.Email=model.Email;
                    client.Intdate = date;
                    client.Intyear = year;
                    client.Strmonth = monthName;
                    

                  
                    _context.Requestclients.Update(client);
                     _context.SaveChangesAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<DashboardRequestModel> GetRequests(short Status)
        {



            List<DashboardRequestModel> allData = (from req in _context.Requests
                           join reqClient in _context.Requestclients
                           on req.Requestid equals reqClient.Requestid into reqClientGroup
                           from rc in reqClientGroup.DefaultIfEmpty()
                           join phys in _context.Physicians
                           on req.Physicianid equals phys.Physicianid into physGroup
                           from p in physGroup.DefaultIfEmpty()
                           join reg in _context.Regions
                          on rc.Regionid equals reg.Regionid into RegGroup
                           from rg in RegGroup.DefaultIfEmpty()
                           where req.Status == Status orderby req.Createddate descending
                           select new DashboardRequestModel
                           {
                               RequestID=req.Requestid,
                               RequestTypeID = req.Requesttypeid,
                               Requestor = req.Firstname + " " + req.Lastname,
                               PatientName = rc.Firstname + " " + rc.Lastname,
                               Dob = new DateOnly((int)rc.Intyear, DateTime.ParseExact(rc.Strmonth, "MMMM", new CultureInfo("en-US")).Month, (int)rc.Intdate),
                               RequestedDate = req.Createddate,
                               Email = rc.Email,

                               Region = rg.Name,
                               ProviderName = p.Firstname + " " + p.Lastname,
                               PhoneNumber = rc.Phonenumber,
                               Address = rc.Address + "," + rc.Street + "," + rc.City + "," + rc.State + "," + rc.Zipcode,
                               Notes = rc.Notes,
                               ProviderID = req.Physicianid,
                               RequestorPhoneNumber = req.Phonenumber
                           }).ToList();
            return allData;
        }
        public ViewNotesModel getNotesByID(int id)
        {
            var model = _context.Requestnotes.FirstOrDefault(E=>E.Requestid==id);
            ViewNotesModel allData = new ViewNotesModel {
             Requestid = id,
                Requestnotesid=model.Requestnotesid,
                Physiciannotes = model.Physiciannotes,
                Administrativenotes = model.Administrativenotes,
                Adminnotes= model.Adminnotes,
            };
            return allData;
        }

    }
}

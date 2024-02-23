using HelloDocAdmin.Entity.Data;
using HelloDocAdmin.Entity.Models;
using HelloDocAdmin.Entity.ViewModels;
using HelloDocAdmin.Entity.ViewModels.AdminSite;
using HelloDocAdmin.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
                Requestclient client = _context.Requestclients.FirstOrDefault(E => E.Requestid == model.RequestID);

                if (client != null)
                {

                    client.Firstname = model.FirstName;
                    client.Lastname = model.LastName;
                    client.Phonenumber = model.PhoneNumber;
                    client.Email = model.Email;
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
        public bool EditViewNotes(string? adminnotes,string? physiciannotes,int? RequestID)
        {
            try

            {
                Requestnote notes = _context.Requestnotes.FirstOrDefault(E => E.Requestid == RequestID);
                if (physiciannotes!=null)
                {
                    if (notes != null)
                    {

                        notes.Physiciannotes = physiciannotes;
                        notes.Modifieddate = DateTime.Now;

                        _context.Requestnotes.Update(notes);
                        _context.SaveChangesAsync();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (adminnotes!=null)
                {
                    if (notes != null)
                    {

                        notes.Adminnotes = adminnotes;
                        notes.Modifieddate = DateTime.Now;

                        _context.Requestnotes.Update(notes);
                        _context.SaveChangesAsync();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
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
            var requestlog = _context.Requeststatuslogs.Where(E => E.Requestid == id && (E.Transtophysician != null)|| (E.Transtoadmin != null));
            var model = _context.Requestnotes.FirstOrDefault(E=>E.Requestid==id);
            ViewNotesModel allData = new ViewNotesModel {
             Requestid = id,
                Requestnotesid=model.Requestnotesid,
                Physiciannotes = model.Physiciannotes,
                Administrativenotes = model.Administrativenotes,
                Adminnotes= model.Adminnotes,
            };
           List< TransfernotesModel> md=new List<TransfernotesModel>();
            foreach (var e in requestlog)
            {
              md.Add(new TransfernotesModel
                {
                    Requestid = e.Requestid,
                    Notes=e.Notes,
                    Physicianid = e.Physicianid,
                    Createddate = e.Createddate,
                    Requeststatuslogid=e.Requeststatuslogid,
                    Transtoadmin=e.Transtoadmin,
                    Transtophysicianid=e.Transtophysicianid
                });
            }
            allData.transfernotes = md;
            return allData;
        }
        public bool UploadDoc(int Requestid, IFormFile? UploadFile)
        {
            try
            {
                string UploadImage;
                if (UploadFile != null)
                {
                    string FilePath = "wwwroot\\Upload\\req_" + Requestid;
                    string path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    string fileNameWithPath = Path.Combine(path, UploadFile.FileName);
                    UploadImage = "~" + FilePath.Replace("wwwroot\\", "/") + "/" + UploadFile.FileName;
                    using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                    {
                        UploadFile.CopyTo(stream)
    ;
                    }
                    var requestwisefile = new Requestwisefile
                    {
                        Requestid = Requestid,
                        Filename = UploadFile.FileName,
                        Createddate = DateTime.Now,
                    };
                    _context.Requestwisefiles.Add(requestwisefile);
                    _context.SaveChanges();
                }

                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
           

           
        }

        public ViewDocumentsModel ViewDocument (int id)
        {
            ViewDocumentsModel alldata = new ViewDocumentsModel();

            var result = from requestWiseFile in _context.Requestwisefiles
                         join request in _context.Requests on requestWiseFile.Requestid equals request.Requestid
                         join physician in _context.Physicians on request.Physicianid equals physician.Physicianid into physicianGroup
                         from phys in physicianGroup.DefaultIfEmpty()
                         join admin in _context.Admins on requestWiseFile.Adminid equals admin.Adminid into adminGroup
                         from adm in adminGroup.DefaultIfEmpty()
                         where request.Requestid == id
                         select new 
                         {

                             Uploader = requestWiseFile.Physicianid != null ? phys.Firstname :
                             (requestWiseFile.Adminid != null ? adm.Firstname : request.Firstname),
                             requestWiseFile.Filename,
                             requestWiseFile.Createddate
                            
                         };
            List<Documents> doc=new List<Documents>();
            foreach (var item in result)
            {
                doc.Add(new Documents
                {
                    Createddate= item.Createddate,
                    Filename = item.Filename,
                    Uploader = item.Uploader,
                });
                    
            }
            alldata.documentslist=doc;
           var req = _context.Requests.FirstOrDefault(r => r.Requestid == id);

            alldata.Firstanme = req.Firstname;
            alldata.RequestID = req.Requestid;
            alldata.ConfirmationNumber = req.Confirmationnumber;
            alldata.Lastanme = req.Lastname;
            return alldata;

        }

    }
}


using HelloDocAdmin.Entity.Data;
using HelloDocAdmin.Entity.Models;
using HelloDocAdmin.Entity.ViewModels;
using HelloDocAdmin.Entity.ViewModels.AdminSite;
using HelloDocAdmin.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Collections;
using System.Globalization;

namespace HelloDocAdmin.Repositories
{

    public class DashboardRepository : IDashboardRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailConfiguration _email;

        public DashboardRepository(ApplicationDbContext context, EmailConfiguration email)
        {
            _context = context;
            _email = email;
        }

        public int GetRequestNumberByStatus(string status)
        {
            List<short> priceList = status.Split(',').Select(short.Parse).ToList();

            int n = _context.Requests.Count(E => priceList.Contains((short)E.Status));

            return n;
        }
        public ViewCaseModel GetRequestForViewCase(int id)
        {
            var n = _context.Requests.FirstOrDefault(E => E.Requestid == id);

            var l = _context.Requestclients.FirstOrDefault(E => E.Requestid == id);

            var region = _context.Regions.FirstOrDefault(E => E.Regionid == l.Regionid);

            ViewCaseModel requestforviewcase = new ViewCaseModel
            {
                RequestID = id,
                Region = region.Name,
                FirstName = l.Firstname,
                LastName = l.Lastname,
                PhoneNumber = l.Phonenumber,
                PatientNotes = l.Notes,
                Email = l.Email,
                physicianID = n.Physicianid,
                RequestTypeID = n.Requesttypeid,
                Address = l.Street + "," + l.City + "," + l.State,
                Room = l.Address,
                ConfirmationNumber = n.Confirmationnumber,
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
        public bool EditViewNotes(string? adminnotes, string? physiciannotes, int RequestID)
        {
            try

            {
                Requestnote notes = _context.Requestnotes.FirstOrDefault(E => E.Requestid == RequestID);

                if (notes != null)
                {
                    if (physiciannotes != null)
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
                    else if (adminnotes != null)
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

                else
                {
                    Requestnote rn = new Requestnote
                    {
                        Requestid = RequestID,
                        Adminnotes = adminnotes,
                        Physiciannotes = physiciannotes,
                        Createddate = DateTime.Now,
                        Createdby = "001e35a5 - cd12 - 4ec8 - a077 - 95db9d54da0f"


                    };
                    _context.Requestnotes.Add(rn);
                    _context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<DashboardRequestModel> GetRequests(string Status)
        {


            List<int> priceList = Status.Split(',').Select(int.Parse).ToList();

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
                                                   where priceList.Contains(req.Status)
                                                   orderby req.Createddate descending
                                                   select new DashboardRequestModel
                                                   {
                                                       RequestID = req.Requestid,
                                                       RequestTypeID = req.Requesttypeid,
                                                       Requestor = req.Firstname + " " + req.Lastname,
                                                       PatientName = rc.Firstname + " " + rc.Lastname,
                                                       Dob = new DateOnly((int)rc.Intyear, DateTime.ParseExact(rc.Strmonth, "MMMM", new CultureInfo("en-US")).Month, (int)rc.Intdate),
                                                       RequestedDate = req.Createddate,
                                                       Email = rc.Email,

                                                       Region = rg.Name,
                                                       ProviderName = p.Firstname + " " + p.Lastname,
                                                       PhoneNumber = rc.Phonenumber,
                                                       Address = rc.Address + ", " + rc.Street + ", " + rc.City + ", " + rc.State + ", " + rc.Zipcode,

                                                       ProviderID = req.Physicianid,
                                                       RequestorPhoneNumber = req.Phonenumber
                                                   }).ToList();



            return allData;
        }
        public async Task<Dashboarddatamodel> GetRequestsbyfilter(string Status, string search = "", int region = 0, int requesttype = 0, int currentpage = 1, int pagesize = 5)
        {
            Dashboarddatamodel dm = new Dashboarddatamodel();

            List<int> priceList = Status.Split(',').Select(int.Parse).ToList();

            IQueryable<DashboardRequestModel> allData = (from req in _context.Requests
                                                         join reqClient in _context.Requestclients
                                                         on req.Requestid equals reqClient.Requestid into reqClientGroup
                                                         from rc in reqClientGroup.DefaultIfEmpty()
                                                         join phys in _context.Physicians
                                                         on req.Physicianid equals phys.Physicianid into physGroup
                                                         from p in physGroup.DefaultIfEmpty()
                                                         join reg in _context.Regions
                                                        on rc.Regionid equals reg.Regionid into RegGroup
                                                         from rg in RegGroup.DefaultIfEmpty()
                                                         where priceList.Contains(req.Status)
                                                         orderby req.Createddate descending
                                                         select new DashboardRequestModel
                                                         {
                                                             RequestID = req.Requestid,
                                                             RequestTypeID = req.Requesttypeid,
                                                             Requestor = req.Firstname + " " + req.Lastname,
                                                             PatientName = rc.Firstname + " " + rc.Lastname,
                                                             Dob = new DateOnly((int)rc.Intyear, DateTime.ParseExact(rc.Strmonth, "MMMM", new CultureInfo("en-US")).Month, (int)rc.Intdate),
                                                             RequestedDate = req.Createddate,
                                                             Email = rc.Email,
                                                             RegionID = rc.Regionid,
                                                             Region = rg.Name,
                                                             ProviderName = p.Firstname + " " + p.Lastname,
                                                             PhoneNumber = rc.Phonenumber,
                                                             Address = rc.Address + ", " + rc.Street + ", " + rc.City + ", " + rc.State + ", " + rc.Zipcode,

                                                             ProviderID = req.Physicianid,
                                                             RequestorPhoneNumber = req.Phonenumber
                                                         });


            if (region != 0)
            {
                allData = allData.Where(r => r.RegionID == region);
            }
            if (requesttype != 0)
            {
                allData = allData.Where(r => r.RequestTypeID == requesttype);
            }
            if (!search.IsNullOrEmpty())

            {
                allData = allData.Where(r => r.PatientName.ToLower().Contains(search.ToLower()));
            }
            dm.TotalPage = (int)Math.Ceiling((double)allData.Count() / pagesize);
            allData = allData.OrderByDescending(x => x.RequestedDate).Skip((currentpage - 1) * pagesize).Take(pagesize);
            dm.requestList = allData.ToList();
            dm.pageSize = pagesize;
            int i = 0;
            foreach (var item in dm.requestList)
            {
                item.Notes = item.Notes ?? new List<string>(); // Initialize Notes if null
                var rsa = (from rs in _context.Requeststatuslogs
                           join py in _context.Physicians on rs.Physicianid equals py.Physicianid into pyGroup
                           from py in pyGroup.DefaultIfEmpty()
                           join p in _context.Physicians on rs.Transtophysicianid equals p.Physicianid into pGroup
                           from p in pGroup.DefaultIfEmpty()
                           join a in _context.Admins on rs.Adminid equals a.Adminid into aGroup
                           from a in aGroup.DefaultIfEmpty()
                           where rs.Requestid == item.RequestID && (rs.Transtoadmin != null || rs.Transtophysicianid != null || rs.Status == 2)
                           select rs.Notes).ToList();

                foreach (var slt in rsa)
                {
                    item.Notes.Add(slt);
                }

                dm.requestList[i].Notes = item.Notes;
                i++;
            }


            dm.CurrentPage = currentpage;





            return dm;
        }
        public ViewNotesModel getNotesByID(int id)
        {
            var rsa = (from rs in _context.Requeststatuslogs
                       join py in _context.Physicians on rs.Physicianid equals py.Physicianid into pyGroup
                       from py in pyGroup.DefaultIfEmpty()
                       join p in _context.Physicians on rs.Transtophysicianid equals p.Physicianid into pGroup
                       from p in pGroup.DefaultIfEmpty()
                       join a in _context.Admins on rs.Adminid equals a.Adminid into aGroup
                       from a in aGroup.DefaultIfEmpty()
                       where rs.Requestid == id && (rs.Transtoadmin != null || rs.Transtophysicianid != null || rs.Status == 2)
                       select new TransfernotesModel
                       {
                           TransPhysician = p.Firstname,
                           Admin = a.Firstname,
                           Physician = py.Firstname,
                           Requestid = rs.Requestid,
                           Notes = rs.Notes,
                           Status = rs.Status,
                           Physicianid = rs.Physicianid,
                           Createddate = rs.Createddate,
                           Requeststatuslogid = rs.Requeststatuslogid,
                           Transtoadmin = rs.Transtoadmin,
                           Transtophysicianid = rs.Transtophysicianid

                       }).ToList();

            var req = _context.Requests.FirstOrDefault(W => W.Requestid == id);
            var requestlog = _context.Requeststatuslogs.Where(E => E.Requestid == id && ((E.Transtophysician != null) || (E.Transtoadmin != null)));
            var cancelnotes = _context.Requeststatuslogs.Where(E => E.Requestid == id && ((E.Status == 3) || (E.Status == 7)));
            var model = _context.Requestnotes.FirstOrDefault(E => E.Requestid == id);
            ViewNotesModel allData = new ViewNotesModel();
            if (model == null)
            {
                allData.Requestid = id;

                allData.Physiciannotes = "-";
                allData.Administrativenotes = "-";
                allData.Adminnotes = "-";
            }
            else
            {
                allData.Status = req.Status;
                allData.Requestid = id;
                allData.Requestnotesid = model.Requestnotesid;
                allData.Physiciannotes = model.Physiciannotes;
                allData.Administrativenotes = model.Administrativenotes;
                allData.Adminnotes = model.Adminnotes;
            }

            List<TransfernotesModel> list = new List<TransfernotesModel>();
            foreach (var e in cancelnotes)
            {

                list.Add(new TransfernotesModel
                {

                    Requestid = e.Requestid,
                    Notes = e.Notes,
                    Status = e.Status,
                    Physicianid = e.Physicianid,
                    Createddate = e.Createddate,
                    Requeststatuslogid = e.Requeststatuslogid,
                    Transtoadmin = e.Transtoadmin,
                    Transtophysicianid = e.Transtophysicianid,


                });
            }
            allData.cancelnotes = list;

            List<TransfernotesModel> md = new List<TransfernotesModel>();
            foreach (var e in rsa)
            {
                md.Add(new TransfernotesModel
                {
                    TransPhysician = e.TransPhysician,
                    Admin = e.Admin,
                    Physician = e.Physician,
                    Requestid = e.Requestid,
                    Notes = e.Notes,
                    Status = e.Status,
                    Physicianid = e.Physicianid,
                    Createddate = e.Createddate,
                    Requeststatuslogid = e.Requeststatuslogid,
                    Transtoadmin = e.Transtoadmin,
                    Transtophysicianid = e.Transtophysicianid
                });
            }
            allData.transfernotes = md;
            return allData;
        }
        public bool SendLink(string firstname, string lastname, string email, string phonenumber)
        {


            if (_email.SendMail(email, "Add New Request", "HIMANSHU"))
            {
                Emaillog el = new Emaillog();
                el.Action = 3;

                el.Sentdate = DateTime.Now;
                el.Createdate = DateTime
                     .Now;
                el.Emailtemplate = "first";
                el.Senttries = 1;
                el.Subjectname = "Add New Request";

                el.Roleid = 2;
                el.Emailid = email;

                _context.Emaillogs.Add(el);
                _context.SaveChanges();
            }

            return true;
        }
        #region Assign_Provider
        public async Task<bool> AssignProvider(int RequestId, int ProviderId, string notes, string id)
        {
            var admindata = _context.Admins.FirstOrDefault(E => E.Aspnetuserid == id);
            var request = await _context.Requests.FirstOrDefaultAsync(req => req.Requestid == RequestId);
            request.Physicianid = ProviderId;

            request.Status = 2;
            _context.Requests.Update(request);
            _context.SaveChanges();

            Requeststatuslog rsl = new Requeststatuslog();
            rsl.Requestid = RequestId;
            rsl.Physicianid = ProviderId;
            rsl.Notes = notes;

            rsl.Createddate = DateTime.Now;
            rsl.Status = 2;
            rsl.Adminid = admindata.Adminid;
            _context.Requeststatuslogs.Update(rsl);
            _context.SaveChanges();

            return true;


        }
        #endregion
        public bool UploadDoc(int Requestid, IFormFile? UploadFile)
        {
            BitArray bt = new BitArray(1);
            bt.Set(0, false);
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
                        Isdeleted = bt,
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

        public ViewDocumentsModel ViewDocument(int id)
        {

            BitArray bt = new BitArray(1);
            bt.Set(0, false);
            ViewDocumentsModel alldata = new ViewDocumentsModel();

            var result = from requestWiseFile in _context.Requestwisefiles
                         join request in _context.Requests on requestWiseFile.Requestid equals request.Requestid
                         join physician in _context.Physicians on request.Physicianid equals physician.Physicianid into physicianGroup
                         from phys in physicianGroup.DefaultIfEmpty()
                         join admin in _context.Admins on requestWiseFile.Adminid equals admin.Adminid into adminGroup
                         from adm in adminGroup.DefaultIfEmpty()
                         where request.Requestid == id && requestWiseFile.Isdeleted == bt
                         select new
                         {

                             Uploader = requestWiseFile.Physicianid != null ? phys.Firstname :
                             (requestWiseFile.Adminid != null ? adm.Firstname : request.Firstname),
                             requestWiseFile.Filename,
                             requestWiseFile.Createddate,
                             requestWiseFile.Requestwisefileid

                         };
            List<Documents> doc = new List<Documents>();
            foreach (var item in result)
            {
                doc.Add(new Documents
                {
                    Createddate = item.Createddate,
                    Filename = item.Filename,
                    Uploader = item.Uploader,
                    RequestWiseFileID = item.Requestwisefileid

                });

            }
            alldata.documentslist = doc;
            var req = _context.Requests.FirstOrDefault(r => r.Requestid == id);

            alldata.Firstanme = req.Firstname;
            alldata.RequestID = req.Requestid;
            alldata.ConfirmationNumber = req.Confirmationnumber;
            alldata.Lastanme = req.Lastname;
            return alldata;

        }


        public bool CancelCase(int RequestID, string Note, string CaseTag, string id)
        {
            try
            {
                var admindata = _context.Admins.FirstOrDefault(E => E.Aspnetuserid == id);
                var requestData = _context.Requests.FirstOrDefault(e => e.Requestid == RequestID);
                if (requestData != null)
                {
                    requestData.Casetag = CaseTag;
                    requestData.Status = 3;
                    requestData.Modifieddate = DateTime.Now;

                    _context.Requests.Update(requestData);
                    _context.SaveChanges();

                    Requeststatuslog rsl = new Requeststatuslog
                    {
                        Requestid = RequestID,
                        Notes = Note,
                        Status = 3,
                        Adminid = admindata.Adminid,
                        Createddate = DateTime.Now
                    };
                    _context.Requeststatuslogs.Add(rsl);
                    _context.SaveChanges();
                    return true;
                }
                else { return false; }
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public bool BlockCase(int RequestID, string Note)
        {

            try
            {
                var requestData = _context.Requests.FirstOrDefault(e => e.Requestid == RequestID);
                if (requestData != null)
                {

                    requestData.Status = 11;
                    _context.Requests.Update(requestData);
                    _context.SaveChanges();
                    Blockrequest blc = new Blockrequest
                    {
                        Requestid = requestData.Requestid,
                        Phonenumber = requestData.Phonenumber,
                        Email = requestData.Email,
                        Reason = Note,
                        Createddate = DateTime.Now,
                        Modifieddate = DateTime.Now


                    };
                    _context.Blockrequests.Add(blc);
                    _context.SaveChanges();
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

    }
}

using HelloDocAdmin.Entity.Data;
using HelloDocAdmin.Entity.Models;
using HelloDocAdmin.Entity.ViewModel.PatientSite;
using HelloDocAdmin.Entity.ViewModels.AdminSite;
using HelloDocAdmin.Entity.ViewModels.PatientSite;
using HelloDocAdmin.Repositories.PatientInterface;
using Microsoft.AspNetCore.Http;
using System.Collections;
using System.Globalization;
using System.Net;
using System.Net.Mail;

namespace HelloDocAdmin.Repositories
{
    public class PatientDashboardRepository : IPatientDashboardRepository
    {

        private readonly ApplicationDbContext _context;
        private readonly EmailConfiguration _email;
        public PatientDashboardRepository(ApplicationDbContext context, EmailConfiguration email)
        {
            _context = context;
            _email = email;
        }
        #region DashboardData
        public ViewDashboardDataModel DashboardData(string UserID, int pagesize = 10, int currentpage = 1)
        {
            BitArray bt = new BitArray(1);
            bt.Set(0, false);
            var UserIDForRequest = _context.Users.FirstOrDefault(r => r.Aspnetuserid == UserID);
            ViewDashboardDataModel model = new ViewDashboardDataModel();
            if (UserIDForRequest != null)
            {
                int date = UserIDForRequest.Intdate;
                int year = UserIDForRequest.Intyear;
                string trimmedMonth = UserIDForRequest.Strmonth.Trim();
                int month = DateTime.ParseExact(trimmedMonth, "MMMM", new CultureInfo("en-US")).Month;
                DateTime birthDate = new DateTime(year, month, date);
                model.Userid = UserIDForRequest.Userid;
                model.Firstname = UserIDForRequest.Firstname;
                model.Lastname = UserIDForRequest.Lastname;
                model.Mobile = UserIDForRequest.Mobile;
                model.Email = UserIDForRequest.Email;
                model.Street = UserIDForRequest.Street;
                model.State = UserIDForRequest.State;
                model.City = UserIDForRequest.City;
                model.Zipcode = UserIDForRequest.Zipcode;

                model.BirthDate = birthDate;

            }

            IQueryable<Request> data = (from Req in _context.Requests

                                        where Req.Userid == UserIDForRequest.Userid
                                        select new Request()
                                               );

            model.TotalPage = (int)Math.Ceiling((double)data.Count() / pagesize);
            data = data.Skip((currentpage - 1) * pagesize).Take(pagesize);



            List<Request> Request = _context.Requests.Where(r => r.Userid == UserIDForRequest.Userid).ToList();
            model.pageSize = pagesize;
            model.CurrentPage = currentpage;
            model.requests = Request;
            Dictionary<int, int> list = new Dictionary<int, int>();
            foreach (Request request in Request)
            {
                var doc = _context.Requestwisefiles.Where(r => r.Requestid == request.Requestid).FirstOrDefault();

                if (doc != null)
                {
                    int numberOfDocs = _context.Requestwisefiles.Where(u => u.Requestid == request.Requestid && u.Isdeleted == bt).Count();
                    list.Add(request.Requestid, numberOfDocs);
                }
            }
            model.ids = list;
            return model;
        }
        #endregion
        #region DocumentsPage
        public ViewDocumentsModel ViewDocument(int RequestID)
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
                         where request.Requestid == RequestID && requestWiseFile.Isdeleted == bt
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
            var req = _context.Requests.FirstOrDefault(r => r.Requestid == RequestID);
            alldata.Firstanme = req.Firstname;
            alldata.RequestID = req.Requestid;
            alldata.ConfirmationNumber = req.Confirmationnumber;
            alldata.Lastanme = req.Lastname;
            return alldata;
        }

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
                        UploadFile.CopyTo(stream);
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


        #endregion
        public bool SendAllMailDoc(string path, int RequestID)
        {

            List<int> pathList = path.Split(',').Select(int.Parse).ToList();


            string senderEmail = "practicetatvasoft@outlook.com";
            string senderPassword = "Tatvasoft@123";

            SmtpClient client = new SmtpClient("smtp.office365.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(senderEmail, senderPassword),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false
            };


            string? email = _context.Requestclients.FirstOrDefault(x => x.Requestid == RequestID).Email;
            var requestWiseFile = from requestFiles in _context.Requestwisefiles
                                  where pathList.Contains(requestFiles.Requestwisefileid)
                                  select new Requestwisefile
                                  {
                                      Requestwisefileid = requestFiles.Requestwisefileid,
                                      Filename = requestFiles.Filename,
                                      Requestid = requestFiles.Requestid,
                                  };
            string message = $@"<html>
                                <body>  
                                <h1>All Documents</h1>
                                </body>
                                </html>";
            if (email != null)
            {
                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail, "hallodoc@gmail.com"),
                    Subject = "Documents",
                    Body = message,
                    IsBodyHtml = true
                };
                foreach (var item in requestWiseFile)
                {
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Upload/req_" + item.Requestid + "/" + item.Filename);
                    Attachment attachment = new Attachment(filePath);
                    mailMessage.Attachments.Add(attachment);
                }
                mailMessage.To.Add(email)
;
                client.Send(mailMessage);

                Emaillog el = new Emaillog();
                el.Action = 2;

                el.Sentdate = DateTime.Now;
                el.Createdate = DateTime
                     .Now;
                el.Emailtemplate = "first";
                el.Senttries = 1;
                el.Subjectname = "New Patient Account Creation";
                el.Requestid = RequestID;
                el.Roleid = 4;
                el.Emailid = email;
                _context.Emaillogs.Add(el);
                _context.SaveChanges();

                foreach (var attachment in mailMessage.Attachments)
                {
                    attachment.Dispose();
                }
                return true;
            }
            return false;
        }
        public bool DeleteDoc(int RequestWiseFileID)
        {
            var data = _context.Requestwisefiles.FirstOrDefault(e => e.Requestwisefileid == RequestWiseFileID);
            if (data != null)
            {
                BitArray bt = new BitArray(1);
                bt.Set(0, true);
                data.Isdeleted = bt;
                _context.Requestwisefiles.Update(data);
                _context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }

        }



        #region GetDataForME
        public ViewPatientRequest GetDataForME(string id)
        {
            ViewPatientRequest viewdata = new ViewPatientRequest();
            var UserIDForRequest = _context.Users.Where(r => r.Aspnetuserid == id).FirstOrDefault();
            int date = UserIDForRequest.Intdate;
            int year = UserIDForRequest.Intyear;
            string trimmedMonth = UserIDForRequest.Strmonth.Trim();
            int month = DateTime.ParseExact(trimmedMonth, "MMMM", new CultureInfo("en-US")).Month;
            DateTime birthDate = new DateTime(year, month, date);
            viewdata.FirstName = UserIDForRequest.Firstname;
            viewdata.LastName = UserIDForRequest.Lastname;
            viewdata.PhoneNumber = UserIDForRequest.Mobile;
            viewdata.Email = UserIDForRequest.Email;
            viewdata.Street = UserIDForRequest.Street;
            viewdata.State = UserIDForRequest.State;
            viewdata.City = UserIDForRequest.City;
            viewdata.ZipCode = UserIDForRequest.Zipcode;
            viewdata.BirthDate = birthDate;

            return viewdata;
        }
        #endregion

        #region GetDataForMESomeElse
        public ViewFamilyFriendRequest GetDataForSomeOneElse(string id)
        {
            ViewFamilyFriendRequest viewdata = new ViewFamilyFriendRequest();
            var UserIDForRequest = _context.Users.Where(r => r.Aspnetuserid == id).FirstOrDefault();
            viewdata.FF_FirstName = UserIDForRequest.Firstname;
            viewdata.FF_LastName = UserIDForRequest.Lastname;
            viewdata.FF_PhoneNumber = UserIDForRequest.Mobile;
            viewdata.FF_Email = UserIDForRequest.Email;

            return viewdata;
        }
        #endregion


        #region CreateNewRequestForME
        public bool CreateNewRequestForME(ViewPatientRequest viewdata)
        {
            try
            {
                BitArray bt = new BitArray(1);
                bt.Set(0, false);
                var region = _context.Regions.FirstOrDefault(u => u.Name == viewdata.State.Trim().ToLower().Replace(" ", ""));
                int requests = _context.Requests.Where(u => u.Createddate == DateTime.Now.Date).Count();
                string ConfirmationNumber = string.Concat(region.Abbreviation, viewdata.FirstName.Substring(0, 2).ToUpper(), viewdata.LastName.Substring(0, 2).ToUpper(), viewdata.LastName.Substring(0, 2).ToUpper(), requests.ToString("D" + 4));

                var isexist = _context.Users.FirstOrDefault(x => x.Email == viewdata.Email);
                Guid id = Guid.NewGuid();
                var Request = new Request
                {
                    Requesttypeid = 1,
                    Status = 1,
                    Firstname = viewdata.FirstName,
                    Lastname = viewdata.LastName,
                    Email = viewdata.Email,
                    Userid = isexist.Userid,
                    Createduserid = isexist.Userid,
                    Phonenumber = viewdata.PhoneNumber,
                    Confirmationnumber = ConfirmationNumber,
                    Createddate = DateTime.Now,
                    Isurgentemailsent = new BitArray(1),
                };
                _context.Requests.Add(Request);
                _context.SaveChanges();

                if (viewdata.UploadFile != null)
                {
                    string FilePath = "wwwroot\\Upload";
                    string path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    string fileNameWithPath = Path.Combine(path, viewdata.UploadFile.FileName);
                    viewdata.UploadImage = "~" + FilePath.Replace("wwwroot\\", "/") + "/" + viewdata.UploadFile.FileName;
                    using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                    {
                        viewdata.UploadFile.CopyTo(stream)
    ;
                    }
                    var requestwisefile = new Requestwisefile
                    {
                        Requestid = Request.Requestid,
                        Filename = viewdata.UploadFile.FileName,
                        Createddate = DateTime.Now,
                        Isdeleted = bt
                    };
                    _context.Requestwisefiles.Add(requestwisefile);
                    _context.SaveChanges();
                }
                int monthnum = viewdata.BirthDate.Month;
                string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(monthnum);
                int date = viewdata.BirthDate.Day;
                int year = viewdata.BirthDate.Year;
                var requestclient = new Requestclient
                {
                    Requestid = Request.Requestid,
                    Notes = viewdata.Symptoms,
                    Firstname = viewdata.FirstName,
                    Lastname = viewdata.LastName,
                    Phonenumber = viewdata.PhoneNumber,
                    Email = viewdata.Email,
                    State = viewdata.State,
                    City = viewdata.City,
                    Street = viewdata.Street,
                    Zipcode = viewdata.ZipCode,
                    Regionid = region.Regionid,
                    Intdate = date,
                    Intyear = year,
                    Strmonth = monthName,

                };
                _context.Requestclients.Add(requestclient);
                _context.SaveChanges();
                Requeststatuslog rsl = new Requeststatuslog();
                rsl.Status = 1;
                rsl.Createddate = DateTime.Now;
                rsl.Notes = viewdata.Symptoms;
                rsl.Requestid = Request.Requestid;
                _context.Requeststatuslogs.Add(rsl);
                _context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        #endregion
        #region CreateNewRequestForME
        public bool CreateNewRequestForSomeOneElse(ViewFamilyFriendRequest viewdata)
        {

            try
            {
                BitArray bt = new BitArray(1);
                bt.Set(0, false);
                var isexist = _context.Users.FirstOrDefault(x => x.Email == viewdata.FF_Email);
                var region = _context.Regions.FirstOrDefault(u => u.Name == viewdata.State.Trim().ToLower().Replace(" ", ""));
                int requests = _context.Requests.Where(u => u.Createddate == DateTime.Now.Date).Count();
                string ConfirmationNumber = string.Concat(region.Abbreviation, viewdata.FirstName.Substring(0, 2).ToUpper(), viewdata.LastName.Substring(0, 2).ToUpper(), viewdata.LastName.Substring(0, 2).ToUpper(), requests.ToString("D" + 4));
                var Request = new Request
                {
                    Requesttypeid = 2,
                    Status = 1,
                    Firstname = viewdata.FF_FirstName,
                    Lastname = viewdata.FF_LastName,
                    Email = viewdata.FF_Email,
                    Relationname = viewdata.FF_RelationWithPatient,
                    Phonenumber = viewdata.FF_PhoneNumber,
                    Createddate = DateTime.Now,
                    Createduserid = isexist.Userid,
                    Confirmationnumber = ConfirmationNumber,

                    Isurgentemailsent = new BitArray(1)

                };
                _context.Requests.Add(Request);
                _context.SaveChanges();

                int monthnum = viewdata.BirthDate.Month;
                string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(monthnum);
                int date = viewdata.BirthDate.Day;
                int year = viewdata.BirthDate.Year;

                var Requestclient = new Requestclient
                {
                    Request = Request,
                    Requestid = Request.Requestid,
                    Notes = viewdata.Symptoms,
                    Firstname = viewdata.FirstName,
                    Lastname = viewdata.LastName,
                    Phonenumber = viewdata.PhoneNumber,
                    Email = viewdata.Email,
                    State = viewdata.State,
                    City = viewdata.City,
                    Address = viewdata.RoomSite,
                    Street = viewdata.Street,
                    Zipcode = viewdata.ZipCode,
                    Regionid = region.Regionid,

                    Intdate = date,
                    Intyear = year,
                    Strmonth = monthName

                };
                _context.Requestclients.Add(Requestclient);
                _context.SaveChanges();
                if (viewdata.UploadFile != null)
                {
                    string FilePath = "wwwroot\\Upload\\req_" + Request.Requestid;
                    string path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    string fileNameWithPath = Path.Combine(path, viewdata.UploadFile.FileName);
                    viewdata.UploadImage = "~" + FilePath.Replace("wwwroot\\", "/") + "/" + viewdata.UploadFile.FileName;
                    using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                    {
                        viewdata.UploadFile.CopyTo(stream)
    ;
                    }
                    var requestwisefile = new Requestwisefile
                    {
                        Requestid = Request.Requestid,
                        Filename = viewdata.UploadImage,
                        Createddate = DateTime.Now,
                        Isdeleted = bt
                    };
                    _context.Requestwisefiles.Add(requestwisefile);
                    _context.SaveChanges();
                }

                ENC encyptdecypt = new ENC();

                string encyptemail = encyptdecypt.EnryptString(viewdata.Email);
                string encyptdatetime = encyptdecypt.EncryptDate(DateTime.Now);

                string link = $"https://localhost:44341/Login/NewRegsiter?mail={encyptemail}&datetime={encyptdatetime}";
                string emailContent = @"
        <!DOCTYPE html>
        <html lang=""en"">
        <head>
         <meta charset=""UTF-8"">
         <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
         <title>Patient Registration</title>
        </head>
        <body>
         <div style=""background-color: #f5f5f5; padding: 20px;"">
         <h2>Welcome to Our Healthcare Platform!</h2>
        <p>Dear Patient ,</p>
        <p>Your request for a patient account has been successfully created. To complete your registration, please follow the steps below:</p>
        <ol>
            <li>Click the following link to register:</li>
             <p><a href=""https://localhost:44341/Login/NewRegister?mail=" + encyptemail + "&datetime=" + encyptdatetime + @""">Patient Registration</a></p>
            <li>Follow the on-screen instructions to complete the registration process.</li>
        </ol>
        <p>If you have any questions or need further assistance, please don't hesitate to contact us.</p>
        <p>Thank you,</p>
        <p>The Healthcare Team</p>
        </div>
        </body>
        </html>
        ";


                if (_email.SendMail(viewdata.Email, "New Patient Account Creation", emailContent))
                {
                    Emaillog el = new Emaillog();
                    el.Action = 2;
                    el.Confirmationnumber = ConfirmationNumber;
                    el.Sentdate = DateTime.Now;
                    el.Createdate = DateTime
                         .Now;
                    el.Emailtemplate = "first";
                    el.Senttries = 1;
                    el.Subjectname = "New Patient Account Creation";
                    el.Requestid = Request.Requestid;
                    el.Roleid = 4;
                    el.Emailid = Request.Email;
                    _context.Emaillogs.Add(el);
                    _context.SaveChanges();
                    Requeststatuslog rsl = new Requeststatuslog();
                    rsl.Status = 1;
                    rsl.Createddate = DateTime.Now;
                    rsl.Notes = viewdata.Symptoms;
                    rsl.Requestid = Request.Requestid;
                    _context.Requeststatuslogs.Add(rsl);
                    _context.SaveChanges();

                    return true;


                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }



        }
        #endregion

        #region EditProfile
        public bool ProfileEdit(ViewDashboardDataModel u, string id)
        {
            try
            {
                User userToUpdate = _context.Users.FirstOrDefault(e => e.Userid == u.Userid);
                int monthnum = u.BirthDate.Month;
                string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(monthnum);
                int date = u.BirthDate.Day;
                int year = u.BirthDate.Year;
                if (userToUpdate != null)
                {
                    userToUpdate.Firstname = u.Firstname;
                    userToUpdate.Lastname = u.Lastname;
                    userToUpdate.Mobile = u.Mobile;
                    userToUpdate.State = u.State;
                    userToUpdate.Street = u.Street;
                    userToUpdate.City = u.City;
                    userToUpdate.Zipcode = u.Zipcode;
                    userToUpdate.Intdate = date;
                    userToUpdate.Intyear = year;
                    userToUpdate.Strmonth = monthName;

                    userToUpdate.Modifiedby = id;
                    userToUpdate.Modifieddate = DateTime.Now;

                    _context.Users.Update(userToUpdate);
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
        #endregion
    }
}

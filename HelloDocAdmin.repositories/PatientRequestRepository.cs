using HelloDocAdmin.Entity.Data;
using HelloDocAdmin.Entity.Models;
using HelloDocAdmin.Entity.ViewModel.PatientSite;
using HelloDocAdmin.Repositories.PatientInterface;
using Microsoft.AspNetCore.Identity;
using System.Collections;
using System.Globalization;

namespace HelloDocAdmin.Repositories
{
    public class PatientRequestRepository : IPatientRequestRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailConfiguration _email;

        public PatientRequestRepository(ApplicationDbContext context, EmailConfiguration email)
        {
            _context = context;
            _email = email;
        }
        public bool CheckUserExist(string? Email)
        {
            var aspnetuser = _context.Aspnetusers.SingleOrDefault(x => x.Email == Email);
            if (aspnetuser != null)
            {
                return true;
            }
            return false;
        }
        public bool UserIsBlocked(string? Email)
        {
            var aspnetuser = _context.Blockrequests.SingleOrDefault(x => x.Email == Email);
            if (aspnetuser != null)
            {
                return true;
            }
            return false;
        }


        #region Patient Request Create
        public bool CreatePatientRequest(ViewPatientRequest viewdata)
        {
            BitArray bt = new BitArray(1);
            bt.Set(0, false);
            try
            {

                var region = _context.Regions.FirstOrDefault(u => u.Name == viewdata.State.Trim().ToLower().Replace(" ", ""));
                int requests = _context.Requests.Where(u => u.Createddate == DateTime.Now.Date).Count();
                string ConfirmationNumber = string.Concat(region.Abbreviation, viewdata.FirstName.Substring(0, 2).ToUpper(), viewdata.LastName.Substring(0, 2).ToUpper(), viewdata.LastName.Substring(0, 2).ToUpper(), requests.ToString("D" + 4));

                var isexist = _context.Users.FirstOrDefault(x => x.Email == viewdata.Email);

                if (isexist == null)
                {
                    Guid id = Guid.NewGuid();
                    var hasher = new PasswordHasher<string>();

                    string hashedPassword = hasher.HashPassword(null, viewdata.Password);
                    var Aspnetuser = new Aspnetuser
                    {
                        Id = id.ToString(),
                        Username = viewdata.FirstName + " " + viewdata.LastName,
                        Email = viewdata.Email,
                        Passwordhash = hashedPassword,
                        Phonenumber = viewdata.PhoneNumber,

                        CreatedDate = DateTime.Now,
                    };
                    _context.Aspnetusers.Add(Aspnetuser);
                    _context.SaveChanges();

                    int monthnum = viewdata.BirthDate.Month;
                    string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(monthnum);
                    int date = viewdata.BirthDate.Day;
                    int year = viewdata.BirthDate.Year;

                    var User = new User
                    {
                        Aspnetuserid = Aspnetuser.Id,
                        Firstname = viewdata.FirstName,
                        Lastname = viewdata.LastName,
                        Email = viewdata.Email,
                        Createdby = Aspnetuser.Id,
                        Createddate = DateTime.Now,
                        Mobile = viewdata.PhoneNumber,
                        Street = viewdata.Street,
                        State = viewdata.State,
                        City = viewdata.City,
                        Zipcode = viewdata.ZipCode,
                        Intdate = date,
                        Intyear = year,
                        Strmonth = monthName
                    };
                    _context.Users.Add(User);
                    _context.SaveChanges();

                    var Request = new Request
                    {
                        Requesttypeid = 1,
                        Status = 1,
                        Firstname = viewdata.FirstName,
                        Lastname = viewdata.LastName,
                        Email = viewdata.Email,
                        Userid = User.Userid,
                        Phonenumber = viewdata.PhoneNumber,
                        Confirmationnumber = ConfirmationNumber,
                        Createddate = DateTime.Now,
                        Isurgentemailsent = new BitArray(1),
                    };
                    _context.Requests.Add(Request);
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
                            Filename = viewdata.UploadFile.FileName,
                            Createddate = DateTime.Now,
                            Isdeleted = bt
                        };
                        _context.Requestwisefiles.Add(requestwisefile);
                        _context.SaveChanges();
                    }



                    var requestclient = new Requestclient
                    {
                        Requestid = Request.Requestid,
                        Notes = viewdata.Symptoms,
                        Firstname = viewdata.FirstName,
                        Lastname = viewdata.LastName,
                        Phonenumber = viewdata.PhoneNumber,
                        Email = viewdata.Email,
                        Address = viewdata.Street,
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



                    return true;
                }

                else
                {

                    var Request = new Request
                    {
                        Requesttypeid = 1,
                        Status = 1,
                        Firstname = viewdata.FirstName,
                        Lastname = viewdata.LastName,
                        Email = viewdata.Email,
                        Userid = isexist.Userid,

                        Phonenumber = viewdata.PhoneNumber,
                        Confirmationnumber = ConfirmationNumber,
                        Createddate = DateTime.Now,
                        Isurgentemailsent = new BitArray(1),
                    };
                    _context.Requests.Add(Request);
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
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        #endregion

        #region Create Family Friend Request
        public bool CreateFamilyFriend(ViewFamilyFriendRequest viewdata)
        {
            BitArray bt = new BitArray(1);
            bt.Set(0, false);
            try
            {
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
                    //string FilePath = "wwwroot\\Upload\\req_" + Requestid;
                    //string path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);
                    //if (!Directory.Exists(path))
                    //    Directory.CreateDirectory(path);
                    //string fileNameWithPath = Path.Combine(path, UploadFile.FileName);
                    //UploadImage = "~" + FilePath.Replace("wwwroot\\", "/") + "/" + UploadFile.FileName;

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

                string link = $"https://localhost:44376/Login/RegisterNew?email={encyptemail}&datetime={encyptdatetime}";
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
             <p><a href=""https://localhost:44376/Login/RegisterNew?email=" + encyptemail + "&datetime=" + encyptdatetime + @""">Patient Registration</a></p>
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
        #region Create Concierge Request
        public bool CreateConcierge(ViewConciergeRequest viewdata)
        {
            try
            {
                var region = _context.Regions.FirstOrDefault(u => u.Name == viewdata.CON_State.Trim().ToLower().Replace(" ", ""));
                int requests = _context.Requests.Where(u => u.Createddate == DateTime.Now.Date).Count();
                string ConfirmationNumber = string.Concat(region.Abbreviation, viewdata.FirstName.Substring(0, 2).ToUpper(), viewdata.LastName.Substring(0, 2).ToUpper(), viewdata.LastName.Substring(0, 2).ToUpper(), requests.ToString("D" + 4));




                var Requestconcierge = new Requestconcierge();
                Random _random = new Random();


                var Concierge = new Concierge
                {
                    Conciergename = viewdata.CON_FirstName + viewdata.CON_LastName,
                    Street = viewdata.CON_Street,
                    City = viewdata.CON_City,
                    State = viewdata.CON_State,
                    Zipcode = viewdata.CON_ZipCode,
                    Regionid = region.Regionid,
                    Createddate = DateTime.Now,
                    Address = viewdata.CON_PropertyName
                };
                _context.Concierges.Add(Concierge);
                _context.SaveChanges();


                int id1 = Concierge.Conciergeid;

                var Request = new Request
                {
                    Requesttypeid = 3,
                    Status = 1,
                    Firstname = viewdata.FirstName,
                    Lastname = viewdata.LastName,
                    Email = viewdata.Email,
                    Phonenumber = viewdata.PhoneNumber,
                    Createddate = DateTime.Now,
                    Confirmationnumber = ConfirmationNumber,
                    Isurgentemailsent = new BitArray(1)

                };
                _context.Requests.Add(Request);
                _context.SaveChanges();
                int id2 = Request.Requestid;
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
                    State = viewdata.CON_State,
                    Street = viewdata.CON_Street,
                    City = viewdata.CON_City,
                    Zipcode = viewdata.CON_ZipCode,
                    Location = viewdata.RoomSite,
                    Regionid = region.Regionid,
                    Address = viewdata.RoomSite,
                    Intdate = date,
                    Intyear = year,
                    Strmonth = monthName

                };
                _context.Requestclients.Add(Requestclient);
                _context.SaveChanges();

                Requestconcierge.Requestid = id2;
                Requestconcierge.Conciergeid = id1;

                _context.Requestconcierges.Add(Requestconcierge);
                _context.SaveChanges();
                ENC encyptdecypt = new ENC();

                string encyptemail = encyptdecypt.EnryptString(viewdata.Email);
                string encyptdatetime = encyptdecypt.EncryptDate(DateTime.Now);

                string link = $"https://localhost:44376/Login/RegisterNew?email={encyptemail}&datetime={encyptdatetime}";
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
             <p><a href=""https://localhost:44376/Login/RegisterNew?email=" + encyptemail + "&datetime=" + encyptdatetime + @""">Patient Registration</a></p>
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



        #region BusinessPartner Request
        public bool BusinessPartnerRequest(ViewBusinessPartnerRequest viewdata)
        {
            try
            {
                var region = _context.Regions.FirstOrDefault(u => u.Name == viewdata.State.Trim().ToLower().Replace(" ", ""));
                int requests = _context.Requests.Where(u => u.Createddate == DateTime.Now.Date).Count();
                string ConfirmationNumber = string.Concat(region.Abbreviation, viewdata.FirstName.Substring(0, 2).ToUpper(), viewdata.LastName.Substring(0, 2).ToUpper(), viewdata.LastName.Substring(0, 2).ToUpper(), requests.ToString("D" + 4));




                var Requestconcierge = new Requestconcierge();
                Random _random = new Random();



                var Business = new Business
                {
                    Name = viewdata.BUP_FirstName + viewdata.BUP_LastName,
                    City = viewdata.City,
                    Zipcode = viewdata.ZipCode,
                    Address1 = viewdata.BUP_PropertyName,
                    Status = 1,
                    Regionid = region.Regionid,
                    Phonenumber = viewdata.BUP_PhoneNumber,
                    Createddate = DateTime.Now
                };
                _context.Businesses.Add(Business);
                _context.SaveChanges();

                int id1 = Business.Businessid;

                var Request = new Request
                {
                    Requesttypeid = 4,
                    Status = 1,
                    Firstname = viewdata.FirstName,
                    Lastname = viewdata.LastName,
                    Email = viewdata.Email,
                    Phonenumber = viewdata.PhoneNumber,
                    Isurgentemailsent = new BitArray(1),
                    Createddate = DateTime.Now,
                    Confirmationnumber = ConfirmationNumber
                };
                _context.Requests.Add(Request);
                _context.SaveChanges();

                int id2 = Request.Requestid;
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
                    Zipcode = viewdata.ZipCode,
                    Street = viewdata.Street,
                    Regionid = region.Regionid,
                    Intdate = date,
                    Intyear = year,
                    Strmonth = monthName

                };
                _context.Requestclients.Add(Requestclient);
                _context.SaveChanges();
                var Requestbusiness = new Requestbusiness
                {
                    Requestid = id2,
                    Businessid = id1
                };
                _context.Requestbusinesses.Add(Requestbusiness);
                _context.SaveChanges();
                ENC encyptdecypt = new ENC();

                string encyptemail = encyptdecypt.EnryptString(viewdata.Email);
                string encyptdatetime = encyptdecypt.EncryptDate(DateTime.Now);

                string link = $"https://localhost:44376/Login/RegisterNew?email={encyptemail}&datetime={encyptdatetime}";
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
             <p><a href=""https://localhost:44376/Login/RegisterNew?email=" + encyptemail + "&datetime=" + encyptdatetime + @""">Patient Registration</a></p>
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



        #region Region Check
        public bool CkeckRegion(string? State)
        {
            var region = _context.Regions.FirstOrDefault(u => u.Name == State.Trim().ToLower().Replace(" ", ""));
            if (region != null)
            {
                return true;
            }
            return false;

            #endregion
        }




    }
}

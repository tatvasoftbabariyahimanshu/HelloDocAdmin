using HELLO_DOC.Models;
using HelloDocAdmin.Entity.Data;
using HelloDocAdmin.Entity.Models;
using HelloDocAdmin.Entity.ViewModels;
using HelloDocAdmin.Entity.ViewModels.PatientSite;
using HelloDocAdmin.PatientRepositories.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Drawing;
using System.Globalization;
using System.Net;
using System.Web.Mvc;

namespace HelloDocAdmin.PatientRepositories
{
    public class PatientRepository:IPatientRepository
    {
        private readonly ApplicationDbContext _context;

        public PatientRepository(ApplicationDbContext context) {  _context = context; }
       
        public async Task<bool> Create(ViewPatientRequest viewdata)
        {
           
                var region = _context.Regions.FirstOrDefault(u => u.Name == viewdata.State.Trim().ToLower().Replace(" ", ""));
            //if (region == null)
            //{
            //    ModelState.AddModelError("State", "Currently we are not serving in this region");
            //    return View("../Request/PatientRequestForm", viewdata);
            //}
            //
            try
            {

                int requests = _context.Requests.Where(u => u.Createddate == DateTime.Now.Date).Count();
                string ConfirmationNumber = string.Concat(region.Abbreviation, viewdata.FirstName.Substring(0, 2).ToUpper(), viewdata.LastName.Substring(0, 2).ToUpper(), viewdata.LastName.Substring(0, 2).ToUpper(), requests.ToString("D" + 4));
                //string hostName = Dns.GetHostName();
                //string IP = Dns.GetHostByName(hostName).AddressList[0].ToString();
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
                    await _context.SaveChangesAsync();

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
                    await _context.SaveChangesAsync();

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
                    await _context.SaveChangesAsync();

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
                        };
                        _context.Requestwisefiles.Add(requestwisefile);
                        _context.SaveChanges();
                    }

                    var Requestclient = new Requestclient
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
                        Zipcode = viewdata.ZipCode,
                        Intdate = date,
                        Intyear = year,
                        Strmonth = monthName



                    };
                    _context.Requestclients.Add(Requestclient);
                    await _context.SaveChangesAsync();




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

                        Createddate = DateTime.Now,
                        Isurgentemailsent = new BitArray(1),
                    };
                    _context.Requests.Add(Request);
                    await _context.SaveChangesAsync();

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
                        };
                        _context.Requestwisefiles.Add(requestwisefile);
                        _context.SaveChanges();
                    }

                    var Requestclient = new Requestclient
                    {
                        Requestid = Request.Requestid,
                        Notes = viewdata.Symptoms,
                        Firstname = viewdata.FirstName,
                        Lastname = viewdata.LastName,
                        Phonenumber = viewdata.PhoneNumber,
                        Email = viewdata.Email,
                        State = viewdata.State,
                        City = viewdata.City,
                        Zipcode = viewdata.ZipCode,


                    };
                    _context.Requestclients.Add(Requestclient);
                    await _context.SaveChangesAsync();





                }
            }catch(Exception e)
            {
                return false;
            }
            return true;
            
           
        }
    }
}

using AspNetCoreHero.ToastNotification.Abstractions;
using DocumentFormat.OpenXml.InkML;
using HelloDocAdmin.Entity.Data;
using HelloDocAdmin.Entity.Models;
using HelloDocAdmin.Entity.ViewModels.Authentication;
using HelloDocAdmin.Repositories;
using HelloDocAdmin.Repositories.Interface;
using HelloDocAdmin.Repositories.PatientInterface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Net;
using Microsoft.AspNetCore.Http.Extensions;

namespace HelloDocAdmin.Controllers.AdminSite
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailConfiguration _email;
        private readonly INotyfService _notyf;
        private readonly ILoginRepository _loginRepository;
        private readonly IJWTService _jwtservice;
        private readonly IPatientRequestRepository _patientRequestRepository;
         public LoginController(ApplicationDbContext context, EmailConfiguration email, INotyfService notyf,ILoginRepository loginRepository, IJWTService jwtservice,IPatientRequestRepository patientRequestRepository)
        {
            _context = context;
            _email = email;
            _notyf = notyf;
            _loginRepository = loginRepository;
            _jwtservice = jwtservice;
            _patientRequestRepository = patientRequestRepository;
        }
        public IActionResult Index()
        {
            return View("../AdminSite/Login/Index");
        }
        public async Task<IActionResult> CheckAccessLogin(LoginViewModel vm)
        {

            if (ModelState.IsValid)
            {
                UserInfo admin = await _loginRepository.CheckAccessLogin(vm);
                if (admin != null)
                {

                    var jwttoken = _jwtservice.GenerateJWTAuthetication(admin);
                    Response.Cookies.Append("jwt", jwttoken);
              
                    Response.Cookies.Append("Status","1");
                    Response.Cookies.Append("StatusText", "New");



                    _notyf.Success("Login is successfully..");
                    if(admin.Role=="Admin")
                    return RedirectToAction("Index", "Dashboard");
                    else if (admin.Role == "Patient")
                    return RedirectToAction("Index", "PatientDashboard");
                    else
                        return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    _notyf.Error("Invalid Email or Password..");
                    return View("../AdminSite/Login/Index", vm);
                }
            }
            else
            {
                return View("../AdminSite/Login/Index", vm);
            }



        }

        public async Task<IActionResult> ResetEmail(ForgotPassword fp)
        {
            if (ModelState.IsValid)
            {
                if (_patientRequestRepository.CheckUserExist(fp.Email))
                {
                    bool data = _loginRepository.sendmailforresetpass(fp.Email);
                    if (data)
                    {
                        _notyf.Information("Check your Email For Reset password link");
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        _notyf.Error("Reset Link was not sent");
                        return View("../AdminSite/Login/ForgotPassword", fp);
                    }
                }
                else
                {
                    _notyf.Error("Email not Registered");
                    return View("../AdminSite/Login/ForgotPassword", fp);
                }
            }
            else
            {
                return View("../AdminSite/Login/ForgotPassword",fp);
            }
           
        }
        public IActionResult SavePassword(ChangePassModel cpm)
        {
            if(ModelState.IsValid)
            {
                
                ENC eNC = new ENC();
            
               cpm.Email=eNC.DecryptString(cpm.Email);
                if(_loginRepository.savepass(cpm))
                {
                    _notyf.Success("Password Changes Successfully...");
                    return RedirectToAction("Index");
                }
                else
                {
                    _notyf.Error("Password Not Changed");
                    return View("../AdminSite/Login/Changepass", cpm);
                }
            }
            else
            {
                _notyf.Error("Add Valid Details");
                return View("../AdminSite/Login/Changepass", cpm);
            }
        }

            public async Task<IActionResult> AccessDenide()
        {
            return View("../AdminSite/Login/AccessDenide");
        }
        public async Task<IActionResult> ForgotPassword()
        {
            return View("../AdminSite/Login/ForgotPassword");
        }
        [HttpGet]
        public async Task<IActionResult> ChangePassword(string email, string datetime)
        {
            ENC sn=new ENC();
            var currentUrl =Request.GetDisplayUrl();
            Console.WriteLine(currentUrl);
            ChangePassModel cpm =new ChangePassModel();
            cpm.Email = sn.DecryptString(email);
            TimeSpan time = DateTime.Now - sn.DecryptDate(datetime);

            if(!_loginRepository.islinkexist(currentUrl))
            {
                if (time.TotalHours > 24)
                {
                    return View("../AdminSite/Login/LinkExpired");
                }
                else
                {
                    return View("../AdminSite/Login/Changepass", cpm);
                }
            }
            else
            {
                return View("../AdminSite/Login/LinkExpired");
            }
         
            
        }
        public async Task<IActionResult> NewRegsiter(string mail, string datetime)
        {
            ENC sn = new ENC();
            NewRegistration cpm=new NewRegistration
           {
               Email= sn.DecryptString(mail),
           };
            TimeSpan time = DateTime.Now - sn.DecryptDate(datetime);

            if (time.TotalHours > 24)
            {
                return View("../AdminSite/Login/LinkExpired");
            }

            else
            {

                return View("../AdminSite/Login/RegisterNew",cpm);
            }

            
        }
        public async Task<IActionResult> Logout()
        {
            Response.Cookies.Delete("jwt");
            Response.Cookies.Delete("UserID");
            Response.Cookies.Delete("UserName");
            return RedirectToAction("Index", "Dashboard");
        }
        public IActionResult SaveUser(NewRegistration cpm)
        {
            if (ModelState.IsValid)
            {
               if(_patientRequestRepository.CheckUserExist(cpm.Email))
                {

                    _notyf.Success("User Alredy Exist...");
                    return View("../AdminSite/Login/RegisterNew", cpm);
                }
              
                if (_loginRepository.saveuser(cpm))
                {
                    _notyf.Success("User Created  Successfully...");
                    return RedirectToAction("Index");
                }
                else
                {

                    _notyf.Error("User Not Created");
                    return View("../AdminSite/Login/RegisterNew", cpm);
                }
            }
            else
            {
                _notyf.Error("Add Valid Details");
                return View("../AdminSite/Login/RegisterNew", cpm);
            }
        }

    }
}

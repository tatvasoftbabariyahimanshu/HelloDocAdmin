using AspNetCoreHero.ToastNotification.Abstractions;
using HelloDocAdmin.Entity.Data;
using HelloDocAdmin.Entity.Models;
using HelloDocAdmin.Entity.ViewModels.Authentication;
using HelloDocAdmin.Repositories.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Web.WebPages.Html;

namespace HelloDocAdmin.Controllers.AdminSite
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailConfiguration _email;
        private readonly INotyfService _notyf;
        private readonly ILoginRepository _loginRepository;
        private readonly IJWTService _jwtservice;

        public LoginController(ApplicationDbContext context, EmailConfiguration email, INotyfService notyf,ILoginRepository loginRepository, IJWTService jwtservice)
        {
            _context = context;
            _email = email;
            _notyf = notyf;
            _loginRepository = loginRepository;
            _jwtservice = jwtservice;
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

                    _notyf.Success("Login is successfully..");
                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    _notyf.Success("Invalid Email or Password..");
                    return View("../AdminSite/Login/Index", vm);
                }
            }
            else
            {
                return View("../AdminSite/Login/Index", vm);
            }



        }

        //public async Task<IActionResult> CheckAccessLogin(LoginViewModel vm)
        //{

        //    if (ModelState.IsValid)
        //    {
        //        var user = await _context.Aspnetusers.FirstOrDefaultAsync(m => m.Email == vm.Email);
        //        var admin = await _context.Admins.FirstOrDefaultAsync(m => m.Aspnetuserid == user.Id);
        //        var role = _context.Roles.FirstOrDefault(m => m.Roleid == admin.Roleid);
        //        var hasher = new PasswordHasher<string>();
        //        PasswordVerificationResult result = hasher.VerifyHashedPassword(null, user.Passwordhash, vm.Password);
        //        if (user != null)
        //        {
        //            if (result == PasswordVerificationResult.Success)
        //            {
        //                HttpContext.Session.SetString("UserName", user.Username.ToString());
        //                HttpContext.Session.SetString("UserID", user.Id.ToString());
        //                HttpContext.Session.SetString("UserRole", role.Name.ToString());

        //            }
        //            else
        //            {

        //                _notyf.Error("Invalid Emails Or Password");
        //                return View("../AdminSite/Login/Index", vm);

        //            }


        //            _notyf.Success("Login is successfully..");
        //            return RedirectToAction("Index", "Dashboard");
        //        }
        //        else
        //        {
        //            ViewData["error"] = "EmailID Or Password Is Invalid!!";
        //            return View("PatientLogin");
        //        }
        //    }
        //    else
        //    {
        //        return View("../AdminSite/Login/Index", vm);
        //    }



        //}


        public async Task<IActionResult> AccessDenide()
        {
            return View("../AdminSite/Login/AccessDenide");
        }
        public async Task<IActionResult> Logout()
        {
            Response.Cookies.Delete("jwt");
            return RedirectToAction("Index", "Dashboard");
        }
    }
}

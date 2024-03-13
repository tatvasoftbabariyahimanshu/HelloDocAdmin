using AspNetCoreHero.ToastNotification.Abstractions;
using HelloDocAdmin.Controllers.Authenticate;
using HelloDocAdmin.Entity.Data;
using HelloDocAdmin.Entity.Models;
using HelloDocAdmin.Entity.ViewModels;
using HelloDocAdmin.Repositories;
using HelloDocAdmin.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HelloDocAdmin.Controllers.AdminSite
{
    public class PhysicianController : Controller
    {
        private IDashboardRepository _dashboardrepo;
        private ICombobox _combobox;
        private readonly ILogger<DashboardController> _logger;
        private readonly IPhysicianRepository _phyrepo;
        private readonly EmailConfiguration _email;
        private readonly INotyfService _notyf;
        public PhysicianController(ILogger<DashboardController> logger,INotyfService notyf, IDashboardRepository dashboardRepository, ICombobox combobox, IPhysicianRepository phyrepo, EmailConfiguration email)
        {
            _logger = logger;
            _dashboardrepo = dashboardRepository;
            _combobox = combobox;
            _phyrepo = phyrepo;
            _email = email;
            _notyf = notyf;
        }

        public async Task<IActionResult> Index(int? region)
        {
            TempData["Status"] = TempData["Status"];
            ViewBag.RegionComboBox = await _combobox.RegionComboBox();
            var v = await _phyrepo.PhysicianAll();
            if (region == null)
            {
                v = await _phyrepo.PhysicianAll();

            }
            else
            {
                v = await _phyrepo.PhysicianByRegion(region);
                return Json(v);

            }
            return View("../AdminSite/Physician/Index", v);
        }
        #region SendMessage
        public async Task<IActionResult> SendMessage(string? id, string? email, int? way, string? msg)
        {
            bool s;
            if (way == 1)
            {
                s =  _email.SendMail(email, "Check massage", "Heyy " + msg);

            }
            else if (way == 2)
            {

                s =  _email.SendMail(email, "Check massage", "Heyy " + msg);
            }
            else
            {
                s =  _email.SendMail(email, "Check massage", "Heyy " + msg);

            }
            if (s)
            {
                _notyf.Success("Mail Sended Successfully");
            }

            return RedirectToAction("Index");
        }
        #endregion
        #region ChangeNotificationPhysician
        public async Task<IActionResult> ChangeNotificationPhysician(string changedValues)
        {
            Dictionary<int, bool> changedValuesDict = JsonConvert.DeserializeObject<Dictionary<int, bool>>(changedValues);

            if (await _phyrepo.ChangeNotificationPhysician(changedValuesDict))
            {
                _notyf.Success("Changes Done Successfully..");
            }
            else
            {
                _notyf.Error("Problem in Changes");
            }


            return RedirectToAction("Index");
        }
        #endregion
        #region PhysicianProfile
        public async Task<IActionResult> PhysicianProfile(int? id)
        {
            //TempData["Status"] = TempData["Status"];
            ViewBag.RegionComboBox = await _combobox.RegionComboBox();
            ViewBag.userrolecombobox = await _combobox.UserRole();
            if (id == null)
            {
                ViewData["PhysicianAccount"] = "Add";
                return View("../AdminSite/Physician/PhysicianAddEdit");
            }
            else
            {
                ViewData["PhysicianAccount"] = "Edit";
                PhysiciansViewModel v =  await _phyrepo.GetPhysicianById((int)id);
                return View("../AdminSite/Physician/PhysicianAddEdit",v);


            }
        
        }
        #endregion
        #region PhysicianAddEdit
        [HttpPost]
        public async Task<IActionResult> PhysicianAddEdit(PhysiciansViewModel physicians)
        {
            //TempData["Status"] = TempData["Status"];
            ViewBag.RegionComboBox = await _combobox.RegionComboBox();
            ViewBag.userrolecombobox = await _combobox.UserRole();
            // bool b = physicians.Isagreementdoc[0];

            if (ModelState.IsValid)
            {
               bool data= await _phyrepo.PhysicianAddEdit(physicians, CV.LoggedUserID());
                if(data)
                {
                    _notyf.Success("Physician Added Successfully...");
                    return RedirectToAction("Index");
                }
                else
                {
                    _notyf.Error("Physician Not Added...");
                    return View("../AdminSite/Physician/PhysicianAddEdit", physicians);
                }

            }
            else
            {
                _notyf.Error("Physician Data not Valid...");
                return View("../AdminSite/Physician/PhysicianAddEdit", physicians);
            }

         
        }
        #endregion


        #region Edit info
        public async Task<IActionResult> EditAccountInfo(PhysiciansViewModel physicians)
        {
            if (ModelState.IsValid)
            {
                bool data = await _phyrepo.EditAccountInfo(physicians);
                if (data)
                {
                    _notyf.Success("Account Info Updated Successfully...");
                    return RedirectToAction("PhysicianProfile", new { id=physicians.Physicianid });
                }
                else
                {
                    _notyf.Error("some problem");
                    return View("../AdminSite/Physician/PhysicianAddEdit", physicians);
                }
            }
            else
            {
                _notyf.Error("Enter Valid data");
                return View("../AdminSite/Physician/PhysicianAddEdit", physicians);
            }
           
        }
        public async Task<IActionResult> ResetPassword(int Physicianid,string Password)
        {
           
                bool data = await _phyrepo.ResetPassword(Physicianid, Password);
                if (data)
                {
                    _notyf.Success("Password Change");
                    return RedirectToAction("PhysicianProfile", new { id = Physicianid });
                }
                else
                {
                    _notyf.Error("some problem");
                return RedirectToAction("PhysicianProfile", new { id = Physicianid });
            }
            

        }
        #endregion

    }
}

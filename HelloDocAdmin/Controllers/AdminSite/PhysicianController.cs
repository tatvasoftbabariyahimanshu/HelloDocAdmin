using AspNetCoreHero.ToastNotification.Abstractions;
using HelloDocAdmin.Controllers.Authenticate;
using HelloDocAdmin.Entity.Data;
using HelloDocAdmin.Entity.Models;
using HelloDocAdmin.Entity.ViewModels;

using HelloDocAdmin.Entity.ViewModels.AdminSite;
using HelloDocAdmin.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HelloDocAdmin.Controllers.AdminSite
{
    [CustomAuthorization("Admin,Physician")]
    public class PhysicianController : Controller
    {
        private IDashboardRepository _dashboardrepo;
        private ICombobox _combobox;
        private readonly ILogger<DashboardController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IPhysicianRepository _phyrepo;
        private readonly EmailConfiguration _email;
        private readonly INotyfService _notyf;
        public PhysicianController(ApplicationDbContext _apdb, ILogger<DashboardController> logger, INotyfService notyf, IDashboardRepository dashboardRepository, ICombobox combobox, IPhysicianRepository phyrepo, EmailConfiguration email)
        {
            _logger = logger;
            _dashboardrepo = dashboardRepository;
            _combobox = combobox;
            _phyrepo = phyrepo;
            _email = email;
            _notyf = notyf;
            _context = _apdb;
        }
        #region Info Provider
        public async Task<IActionResult> Index(int? region)
        {
            TempData["Status"] = TempData["Status"];
            ViewBag.RegionComboBox = await _combobox.RegionComboBox();
            var v = await _phyrepo.PhysicianAll();
            if (region == null || region == 0)
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
        public IActionResult Scheduling()
        {
            ViewBag.RegionComboBox = _combobox.RegionComboBox();
            return View("../AdminSite/Physician/Scheduling");
        }
        #region SendMessage
        public async Task<IActionResult> SendMessage(int id, string? email, int? way, string? msg)
        {
            bool s;
            if (way == 2)
            {
                s = _email.SendMail(email, "Check massage", "Heyy " + msg);

            }
            else if (way == 1)
            {

                _email.msgbody = "Admin wants to contact you!!!";
                _email.tophone = "8849999677";
                s = _email.sendsms();
                if (s)
                {
                    var data = _context.Physicians.FirstOrDefault(e => e.Physicianid == id);
                    Smslog el = new Smslog();
                    el.Action = 7;

                    el.Sentdate = DateTime.Now;
                    el.Createdate = DateTime
                         .Now;
                    el.Smstemplate = "first";
                    el.Mobilenumber = data.Mobile;

                    el.Senttries = 1;


                    el.Roleid = 2;


                    _context.Smslogs.Add(el);
                    _context.SaveChanges();
                }
            }
            else
            {
                s = _email.SendMail(email, "Check massage", "Heyy " + msg);

            }
            if (s)
            {
                _notyf.Success("Mail/Massage Sended Successfully");
            }
            else
            {
                _notyf.Error("Mail/Massage Not Sended Successfully");
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
            ViewBag.userrolecombobox = await _combobox.RolelistProvider();
            if (id == null)
            {
                ViewData["PhysicianAccount"] = "Add";
                return View("../AdminSite/Physician/PhysicianAddEdit");
            }
            else
            {
                ViewData["PhysicianAccount"] = "Edit";
                PhysiciansViewModel v = await _phyrepo.GetPhysicianById((int)id);
                return View("../AdminSite/Physician/PhysicianAddEdit", v);


            }

        }
        #endregion
        #region PhysicianAddEdit
        [HttpPost]
        public async Task<IActionResult> PhysicianAddEdit(PhysiciansViewModel physicians)
        {
            //TempData["Status"] = TempData["Status"];
            ViewBag.RegionComboBox = await _combobox.RegionComboBox();
            ViewBag.userrolecombobox = await _combobox.RolelistProvider();
            // bool b = physicians.Isagreementdoc[0];

            if (ModelState.IsValid)
            {
                bool data = await _phyrepo.PhysicianAddEdit(physicians, CV.LoggedUserID());
                if (data)
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
            ViewBag.RegionComboBox = await _combobox.RegionComboBox();
            ViewBag.userrolecombobox = await _combobox.RolelistProvider();

            bool data = await _phyrepo.EditAccountInfo(physicians);
            if (data)
            {
                _notyf.Success("Account Info Updated Successfully...");
                return RedirectToAction("PhysicianProfile", new { id = physicians.Physicianid });
            }
            else
            {
                _notyf.Error("some problem");
                return View("../AdminSite/Physician/PhysicianAddEdit", physicians);
            }


        }
        public async Task<IActionResult> ResetPassword(int Physicianid, string Password)
        {
            ViewBag.RegionComboBox = await _combobox.RegionComboBox();
            ViewBag.userrolecombobox = await _combobox.RolelistProvider();
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

        public async Task<IActionResult> EditAdminInfo(PhysiciansViewModel physicians)
        {
            ViewBag.RegionComboBox = await _combobox.RegionComboBox();
            ViewBag.userrolecombobox = await _combobox.RolelistProvider();

            bool data = await _phyrepo.EditAdminInfo(physicians);
            if (data)
            {
                _notyf.Success("admin Info Updated Successfully...");
                return RedirectToAction("PhysicianProfile", new { id = physicians.Physicianid });
            }
            else
            {
                _notyf.Error("some problem");
                return View("../AdminSite/Physician/PhysicianAddEdit", physicians);
            }



        }
        public async Task<IActionResult> EditMailBilling(PhysiciansViewModel physicians)
        {
            ViewBag.RegionComboBox = await _combobox.RegionComboBox();
            ViewBag.userrolecombobox = await _combobox.RolelistProvider();

            bool data = await _phyrepo.EditMailBilling(physicians);
            if (data)
            {
                _notyf.Success("mail and billing Info Updated Successfully...");
                return RedirectToAction("PhysicianProfile", new { id = physicians.Physicianid });
            }
            else
            {
                _notyf.Error("some problem");
                return View("../AdminSite/Physician/PhysicianAddEdit", physicians);
            }



        }
        public async Task<IActionResult> EditProviderProfile(PhysiciansViewModel physicians)
        {
            ViewBag.RegionComboBox = await _combobox.RegionComboBox();
            ViewBag.userrolecombobox = await _combobox.RolelistProvider();

            bool data = await _phyrepo.EditProviderProfile(physicians, CV.LoggedUserID());
            if (data)
            {
                _notyf.Success("mail and billing Info Updated Successfully...");
                return RedirectToAction("PhysicianProfile", new { id = physicians.Physicianid });
            }
            else
            {
                _notyf.Error("some problem");
                return View("../AdminSite/Physician/PhysicianAddEdit", physicians);
            }



        }
        public async Task<IActionResult> DeletePhysician(int PhysicianID)
        {
            ViewBag.RegionComboBox = await _combobox.RegionComboBox();
            ViewBag.userrolecombobox = await _combobox.RolelistProvider();

            bool data = await _phyrepo.DeletePhysician(PhysicianID, CV.LoggedUserID());
            if (data)
            {
                _notyf.Success("Deleted Successfully...");
                return RedirectToAction("Index");
            }
            else
            {
                _notyf.Error("not Deleted problem");
                return RedirectToAction("PhysicianProfile", new { id = PhysicianID });
            }



        }
        #endregion
        #endregion

        [CustomAuthorization("Admin")]

        #region Scheduling
        #region _CreateShift
        public async Task<IActionResult> _CreateShift()
        {
            ViewBag.RegionComboBox = await _combobox.RegionComboBox();
            return PartialView("../AdminSite/Physician/_CreateShift");
        }

        #endregion
        #region _EditShift
        public async Task<IActionResult> _EditShift(int id)
        {
            ViewBag.RegionComboBox = await _combobox.RegionComboBox();


            Schedule schedule = await _phyrepo.GetShiftByShiftdetailId(id);


            return PartialView("../AdminSite/Physician/_EditShift", schedule);
        }
        public async Task<IActionResult> ProviderOncall(int? regionId)
        {
            TempData["Status"] = TempData["Status"];
            ViewBag.RegionComboBox = await _combobox.RegionComboBox();
            List<Physicians> v = await _phyrepo.PhysicianOnCall(regionId);
            if (regionId != null)
            {
                return Json(v);
            }
            return View("../AdminSite/Physician/ProviderOnCall", v);



        }

        #endregion
        #region _CreateShiftPost
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _CreateShiftPost(Schedule v)
        {
            if (await _phyrepo.CreateShift(v, CV.LoggedUserID()))
            {
                _notyf.Success("Shift Created Successfully...");
            }

            return RedirectToAction("Scheduling");
        }
        #endregion


        #endregion
        [CustomAuthorization("Admin,Physician")]
        public async Task<IActionResult> GetShiftForMonth(int? month)
        {
            var v = await _phyrepo.GetShift((int)month);
            return Json(v);
        }

        #region _EditShiftPost
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _EditShiftPost(Schedule v, string submittt)
        {
            if (submittt == "Return" && await _phyrepo.UpdateStatusShift("" + v.Shiftid, CV.LoggedUserID()))
            {
                _notyf.Success("Shift Updated successfully..");
            }
            else
            {

                if (await _phyrepo.EditShift(v, CV.LoggedUserID()))
                {
                    _notyf.Success("Shift Updated successfully..");
                }
                else
                {
                    _notyf.Error("Shift Not  Updated ");
                }
            }

            return RedirectToAction("Index");
        }
        #endregion
        #region _DeleteShiftPost

        public async Task<IActionResult> _DeleteShiftPost(int id)
        {
            if (await _phyrepo.DeleteShift("" + id, CV.LoggedUserID()))
            {
                _notyf.Success("Shift deleted successfully..");
            }
            else
            {
                _notyf.Error("Shift Not  Deleted ");
            }

            return RedirectToAction("Index");
        }
        #endregion
        #region PhysicianAll
        public async Task<IActionResult> PhysicianAll(int? region)
        {


            var v = await _phyrepo.PhysicianAll1();

            //if (region != null)
            //{
            //    v = await _schedulingRepository.PhysicianByRegion(region);

            //}

            return Json(v);
        }
        #endregion
        #region Provider_on_call
        public async Task<IActionResult> RequestedShift(int? regionId)
        {
            ViewBag.RegionComboBox = await _combobox.RegionComboBox();

            return View("../AdminSite/Physician/RequestedShift");
        }
        public async Task<IActionResult> RequestedShiftData(int Region, int pagesize = 5, int currentpage = 1)
        {
            ViewBag.RegionComboBox = await _combobox.RegionComboBox();
            RequestedShift data = await _phyrepo.RequestedShiftData(Region, pagesize, currentpage);

            return PartialView("../AdminSite/Physician/_RequestedshiftsList", data);
        }
        #endregion
        public async Task<IActionResult> ApproveAll(string selectedids)
        {
            if (selectedids == null)
            {
                _notyf.Information("Select Checkbox!!!");
                return RedirectToAction("RequestedShift");
            }
            else
            {
                if (_phyrepo.ApproveShiftAll(selectedids, CV.LoggedUserID()))
                {
                    _notyf.Success("Shift Approved successfully..");
                }
                else
                {
                    _notyf.Error("Shift Not  Approved ");
                }

                return RedirectToAction("RequestedShift");
            }
        }
        public async Task<IActionResult> DeleteAll(string selectedids)
        {
            if (selectedids == null)
            {
                _notyf.Information("Select Checkbox!!!");
                return RedirectToAction("RequestedShift");
            }
            else
            {
                if (_phyrepo.DeleteShiftAll(selectedids, CV.LoggedUserID()))
                {
                    _notyf.Success("Shift deleted successfully..");
                }
                else
                {
                    _notyf.Error("Shift Not  Deleted ");
                }
            }


            return RedirectToAction("RequestedShift");
        }
    }
}

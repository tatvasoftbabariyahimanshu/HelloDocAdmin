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

        #region Index
        [CustomAuthorization("Admin", "Physician")]
        public async Task<IActionResult> Index()
        {

            ViewBag.RegionComboBox = await _combobox.RegionComboBox();

            return View("../AdminSite/Physician/Index");
        }
        #endregion

        #region PhysicianList
        [CustomAuthorization("Admin,Physician", "Physician")]
        public async Task<IActionResult> PhysicianList(string? ProviderName, int? RegionID, int pagesize = 5, int currentpage = 1)
        {
            PhysiciansData sr = await _phyrepo.PhysicianAll(ProviderName, RegionID, pagesize, currentpage);
            return PartialView("../AdminSite/Physician/_physicianList", sr);
        }

        #endregion

        #region Scheduling
        [CustomAuthorization("Admin,Physician", "Scheduling")]
        public async Task<IActionResult> Scheduling()
        {
            ViewBag.RegionComboBox = await _combobox.RegionComboBox();
            return View("../AdminSite/Physician/Scheduling");
        }
        #endregion

        #region SendMessage
        [CustomAuthorization("Admin", "Physician")]
        public async Task<IActionResult> SendMessage(int id, string? email, int? way, string? msg)
        {
            bool s;
            if (way == 2)
            {
                s = _email.SendMail(email, "Check massage", "Heyy " + msg);

            }
            else if (way == 1)
            {

                _email.msgbody = msg;
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
        [CustomAuthorization("Admin", "Physician")]
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
        [CustomAuthorization("Admin,Physician", "Physician")]
        public async Task<IActionResult> PhysicianProfile(int? id)
        {

            ViewBag.RegionComboBox = await _combobox.RegionComboBox();
            ViewBag.userrolecombobox = await _combobox.RolelistProvider();
            if (id == null)
            {
                ViewData["PhysicianAccount"] = "Add";
                return View("../AdminSite/Physician/PhysicianAdd");
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
        [CustomAuthorization("Admin", "Physician")]
        public async Task<IActionResult> PhysicianAddEdit(PhysiciansViewModel physicians)
        {



            ViewBag.RegionComboBox = await _combobox.RegionComboBox();
            ViewBag.userrolecombobox = await _combobox.RolelistProvider();




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
                    ModelState.AddModelError("Address1", "Please Check Your Address");
                    return View("../AdminSite/Physician/PhysicianAdd", physicians);
                }

            }
            else
            {
                _notyf.Error("Physician Data not Valid...");
                return View("../AdminSite/Physician/PhysicianAdd", physicians);
            }


        }
        #endregion


        #region Edit info
        [CustomAuthorization("Admin", "Physician")]
        public async Task<IActionResult> EditAccountInfo(PhysiciansViewModel physicians)
        {
            ViewBag.RegionComboBox = await _combobox.RegionComboBox();
            ViewBag.userrolecombobox = await _combobox.RolelistProvider();

            bool data = await _phyrepo.EditAccountInfo(physicians);
            if (data)
            {
                _notyf.Success("Account Info Updated Successfully...");
                if (CV.LoggedUserRole() == "Admin")
                {
                    return RedirectToAction("PhysicianProfile", new { id = physicians.Physicianid });
                }
                else
                {
                    return RedirectToAction("GetPhysicianProfile");
                }
            }
            else
            {
                _notyf.Error("some problem");
                return View("../AdminSite/Physician/PhysicianAddEdit", physicians);
            }


        }
        [CustomAuthorization("Admin,Physician", "Physician")]
        public async Task<IActionResult> ResetPassword(int Physicianid, string Password)
        {
            ViewBag.RegionComboBox = await _combobox.RegionComboBox();
            ViewBag.userrolecombobox = await _combobox.RolelistProvider();
            bool data = await _phyrepo.ResetPassword(Physicianid, Password);
            if (data)
            {
                _notyf.Success("Password Change");
                if (CV.LoggedUserRole() == "Admin")
                {
                    return RedirectToAction("PhysicianProfile", new { id = Physicianid });
                }
                else
                {
                    return RedirectToAction("GetPhysicianProfile");
                }

            }
            else
            {
                _notyf.Error("some problem");
                if (CV.LoggedUserRole() == "Admin")
                {
                    return RedirectToAction("PhysicianProfile", new { id = Physicianid });
                }
                else
                {
                    return RedirectToAction("GetPhysicianProfile");
                }
            }


        }
        [CustomAuthorization("Admin", "Physician")]
        public async Task<IActionResult> EditAdminInfo(PhysiciansViewModel physicians)
        {
            ViewBag.RegionComboBox = await _combobox.RegionComboBox();
            ViewBag.userrolecombobox = await _combobox.RolelistProvider();

            bool data = await _phyrepo.EditAdminInfo(physicians);
            if (data)
            {
                _notyf.Success("admin Info Updated Successfully...");
                if (CV.LoggedUserRole() == "Admin")
                {
                    return RedirectToAction("PhysicianProfile", new { id = physicians.Physicianid });
                }
                else
                {
                    return RedirectToAction("GetPhysicianProfile");
                }
            }
            else
            {
                _notyf.Error("some problem");

                return View("../AdminSite/Physician/PhysicianAddEdit", physicians);


            }



        }
        [CustomAuthorization("Admin", "Physician")]
        public async Task<IActionResult> EditMailBilling(PhysiciansViewModel physicians)
        {
            ViewBag.RegionComboBox = await _combobox.RegionComboBox();
            ViewBag.userrolecombobox = await _combobox.RolelistProvider();

            bool data = await _phyrepo.EditMailBilling(physicians);
            if (data)
            {
                _notyf.Success("mail and billing Info Updated Successfully...");
                if (CV.LoggedUserRole() == "Admin")
                {
                    return RedirectToAction("PhysicianProfile", new { id = physicians.Physicianid });
                }
                else
                {
                    return RedirectToAction("GetPhysicianProfile");
                }
            }
            else
            {
                _notyf.Error("some problem");
                return View("../AdminSite/Physician/PhysicianAddEdit", physicians);
            }



        }
        [CustomAuthorization("Admin", "Physician")]
        public async Task<IActionResult> EditProviderProfile(PhysiciansViewModel physicians)
        {
            ViewBag.RegionComboBox = await _combobox.RegionComboBox();
            ViewBag.userrolecombobox = await _combobox.RolelistProvider();

            bool data = await _phyrepo.EditProviderProfile(physicians, CV.LoggedUserID());
            if (data)
            {
                _notyf.Success("Provider Info Updated Successfully...");
                if (CV.LoggedUserRole() == "Admin")
                {
                    return RedirectToAction("PhysicianProfile", new { id = physicians.Physicianid });
                }
                else
                {
                    return RedirectToAction("GetPhysicianProfile");
                }
            }
            else
            {
                _notyf.Error("some problem");
                return View("../AdminSite/Physician/PhysicianAddEdit", physicians);
            }



        }
        [CustomAuthorization("Admin", "Physician")]
        public async Task<IActionResult> EditProviderOnbording(Physicians physicians)
        {
            bool data = await _phyrepo.EditProviderOnbording(physicians, CV.LoggedUserID());
            if (data)
            {
                _notyf.Success("Updated Onboarding Successfully...");
                return RedirectToAction("PhysicianProfile", new { id = physicians.Physicianid });
            }
            else
            {
                TempData["Status"] = "some problem";
                return RedirectToAction("PhysicianProfile", new { id = physicians.Physicianid });
            }
        }
        [CustomAuthorization("Admin", "Physician")]
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



        #region Scheduling
        #region _CreateShift
        [CustomAuthorization("Admin,Physician", "Scheduling")]
        public async Task<IActionResult> _CreateShift()
        {
            ViewBag.RegionComboBox = await _combobox.RegionComboBox();
            return PartialView("../AdminSite/Physician/_CreateShift");
        }

        #endregion

        #region _EditShift
        [CustomAuthorization("Admin,Physician", "Scheduling")]
        public async Task<IActionResult> _EditShift(int id)
        {
            ViewBag.RegionComboBox = await _combobox.RegionComboBox();


            Schedule schedule = await _phyrepo.GetShiftByShiftdetailId(id);


            return PartialView("../AdminSite/Physician/_EditShift", schedule);
        }
        [CustomAuthorization("Admin", "Scheduling")]
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
        [CustomAuthorization("Admin,Physician", "Scheduling")]
        public async Task<IActionResult> _CreateShiftPost(Schedule v)
        {
            if (await _phyrepo.CreateShift(v, CV.LoggedUserID()))
            {
                _notyf.Success("Shift Created Successfully...");
            }

            return RedirectToAction("Scheduling");
        }
        #endregion

        #region GetShiftForMonth
        [CustomAuthorization("Admin,Physician", "Scheduling")]
        public async Task<IActionResult> GetShiftForMonth(int? month, int? regionId)
        {
            var v = await _phyrepo.GetShift((int)month, CV.LoggedUserID(), (int)regionId);
            return Json(v);
        }
        #endregion

        #region _EditShiftPost
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorization("Admin,Physician", "Scheduling")]
        public async Task<IActionResult> _EditShiftPost(Schedule v, string submittt)
        {
            if (v.ShiftDate < DateTime.Now)
            {
                _notyf.Warning("You can Not Update Shift of Past Time!!");
                return RedirectToAction("Scheduling");
            }

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

            return RedirectToAction("Scheduling");
        }
        #endregion
        #region _DeleteShiftPost
        [CustomAuthorization("Admin,Physician", "Scheduling")]
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
        [CustomAuthorization("Admin,Physician", "Scheduling")]
        public async Task<IActionResult> PhysicianAll(int? regionId)
        {


            var v = await _phyrepo.PhysicianAll1();

            if (regionId != null)
            {
                v = await _phyrepo.PhysicianByRegion1(regionId);

            }

            return Json(v);
        }
        #endregion
        #region Provider_on_call
        [CustomAuthorization("Admin,Physician", "Scheduling")]
        public async Task<IActionResult> RequestedShift(int? regionId)
        {
            ViewBag.RegionComboBox = await _combobox.RegionComboBox();

            return View("../AdminSite/Physician/RequestedShift");
        }
        [CustomAuthorization("Admin,Physician", "Scheduling")]
        public async Task<IActionResult> RequestedShiftData(int Region, int pagesize = 5, int currentpage = 1)
        {
            ViewBag.RegionComboBox = await _combobox.RegionComboBox();
            RequestedShift data = await _phyrepo.RequestedShiftData(Region, pagesize, currentpage);

            return PartialView("../AdminSite/Physician/_RequestedshiftsList", data);
        }
        #endregion

        #region ApproveAll
        [CustomAuthorization("Admin", "Scheduling")]
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
        #endregion

        #region DeleteAll
        [CustomAuthorization("Admin", "Scheduling")]
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
        #endregion
        #region PhysicianProfile
        [CustomAuthorization("Admin,Physician", "Scheduling")]
        public async Task<IActionResult> GetPhysicianProfile()
        {

            ViewBag.RegionComboBox = await _combobox.RegionComboBox();
            ViewBag.userrolecombobox = await _combobox.RolelistProvider();

            ViewData["PhysicianAccount"] = "Edit";

            PhysiciansViewModel v = await _phyrepo.GetPhysicianByuserId(CV.LoggedUserID());
            return View("../AdminSite/Physician/PhysicianAddEdit", v);




        }
        #endregion
        #endregion




    }
}

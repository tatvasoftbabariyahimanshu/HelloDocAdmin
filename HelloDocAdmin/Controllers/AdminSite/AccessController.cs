using AspNetCoreHero.ToastNotification.Abstractions;
using HelloDocAdmin.Controllers.Authenticate;
using HelloDocAdmin.Entity.Data;
using HelloDocAdmin.Entity.Models;
using HelloDocAdmin.Entity.ViewModels;
using HelloDocAdmin.Entity.ViewModels.AdminSite;
using HelloDocAdmin.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;


namespace HelloDocAdmin.Controllers.AdminSite
{
    [CustomAuthorization("Admin")]
    public class AccessController : Controller
    {
        private IActionRepository _actionrepo;
        private IAccessRepository _accesrepo;
        private IDashboardRepository _dashboardrepo;
        private ICombobox _combobox;
        private readonly ILogger<DashboardController> _logger;
        private readonly INotyfService _notyf;
        private readonly EmailConfiguration _email;
        private readonly IPhysicianRepository _phyrepo;
        private readonly IAdminProfile _admin;
        public AccessController(ILogger<DashboardController> logger, IPhysicianRepository physician, IAdminProfile adminProfile, IDashboardRepository dashboardRepository, ICombobox combobox, IActionRepository actionrepo, INotyfService notyf, EmailConfiguration email, IAccessRepository accesrepo)
        {
            _logger = logger;
            _combobox = combobox;
            _dashboardrepo = dashboardRepository;
            _actionrepo = actionrepo;
            _notyf = notyf;
            _email = email;
            _accesrepo = accesrepo;
            _phyrepo = physician;
            _admin = adminProfile;
        }

        public IActionResult Index()
        {
            List<Role> v = _accesrepo.GetRoleAccessDetails();
            return View("../AdminSite/Access/Index", v);
        }
        public IActionResult CreateAccess(int id)
        {
            if (id != 0)
            {
                ViewData["Page"] = "Edit";
                RolesModel v = _accesrepo.GetRoleByMenus((int)id);
                return View("../AdminSite/Access/CreateAccess", v);
            }
            ViewData["Page"] = "Create";
            return View("../AdminSite/Access/CreateAccess");
        }

        public async Task<IActionResult> GetMenusByAccount(short Accounttype, int roleid)
        {
            List<Menu> v = await _accesrepo.GetMenusByAccount(Accounttype);

            if (roleid != null)
            {
                List<RolesModel.Menu> vm = new List<RolesModel.Menu>();
                List<int> rm = await _accesrepo.CheckMenuByRole(roleid);
                foreach (var item in v)
                {
                    RolesModel.Menu menu = new RolesModel.Menu();
                    menu.Name = item.Name;
                    menu.Menuid = item.Menuid;

                    if (rm.Contains(item.Menuid))
                    {
                        menu.checekd = "checked";
                        vm.Add(menu);
                    }
                    else
                    {
                        vm.Add(menu);
                    }
                }
                return Json(vm);
            }

            return Json(v);
        }


        #region PostRoleMenu
        [HttpPost]
        public async Task<IActionResult> PostRoleMenu(RolesModel role, string Menusid)
        {
            bool data = false;
            if (Menusid == null)
            {
                _notyf.Warning("Select Menus!!!");
                return View("../AdminSite/Access/CreateAccess", role);
            }
            if (ModelState.IsValid)
            {
                if (role.Roleid == null)
                {
                    data = await _accesrepo.PostRoleMenu(role, Menusid, CV.LoggedUserID());

                }
                else
                {
                    data = await _accesrepo.PutRoleMenu(role, Menusid, CV.LoggedUserID());
                }

                if (data)
                {

                    _notyf.Success("Role Added/Updated Successfully");
                }
                else
                {
                    _notyf.Error("Role not added/Updated");
                }
                return RedirectToAction("Index");
            }
            else
            {
                _notyf.Error("Enter valid details");
                return View("../AdminSite/Access/CreateAccess", role);
            }


        }
        #endregion

        #region User_Access
        public async Task<IActionResult> UserAccess()
        {


            List<ViewUserAccess> v = await _accesrepo.GetAllUserDetails();
            return View("../AdminSite/Access/UserAccess", v);
        }
        #endregion
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
                bool data = await _phyrepo.PhysicianAddEdit(physicians, CV.LoggedUserID());
                if (data)
                {
                    _notyf.Success("Physician Added Successfully...");
                    return RedirectToAction("UserAccess");
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
        #region adminaddedit

        public async Task<IActionResult> AdminProfile(string? id)
        {
            ViewBag.userrolecombobox = await _combobox.RolelistAdmin();
            ViewBag.RegionComboBox = await _combobox.RegionComboBox();
            if (id != null)
            {
                ViewAdminProfileModel vm = _admin.GetDetailsForAdminProfile(id);
                return View("../AdminSite/Access/AdminAddEdit", vm);
            }
            return View("../AdminSite/Access/AdminAddEdit");
        }

        public async Task<IActionResult> AdminAddEdit(ViewAdminProfileModel model)
        {
            ViewBag.userrolecombobox = await _combobox.RolelistAdmin();
            ViewBag.RegionComboBox = await _combobox.RegionComboBox();
            if (ModelState.IsValid)
            {
                model.AdminReqionList = Request.Form["SelectedRegions"].Select(int.Parse).ToList();


                if (model.AspnetUserID == null)
                {
                    bool data = _admin.AddAdmin(model, CV.LoggedUserID());
                    if (data)
                    {
                        _notyf.Success("Admin Added sucessfully");
                        return RedirectToAction("UserAccess");
                    }
                    else
                    {
                        _notyf.Error("Admin Not Added ");
                        return View("../AdminSite/Access/AdminAddEdit", model);
                    }
                }
                else
                {
                    bool data = _admin.EditAdmin(model, CV.LoggedUserID());
                    if (data)
                    {
                        _notyf.Success("Admin Edited sucessfully");
                        return RedirectToAction("UserAccess");
                    }
                    else
                    {
                        _notyf.Error("Admin Not Adited ");
                        return View("../AdminSite/Access/AdminAddEdit", model);
                    }
                }



            }
            else
            {
                _notyf.Error("Enter valid data ");
                return View("../AdminSite/Access/AdminAddEdit", model);
            }



        }


        public async Task<IActionResult> DeleteAccess(int id)
        {
            if (_accesrepo.DeleteAccess(id))
            {
                _notyf.Success("Role Deletde SuccessFully");
            }
            else
            {
                _notyf.Error("Role not Deleted SuccessFully");
            }
            return RedirectToAction("Index");
        }
        #endregion





    }
}

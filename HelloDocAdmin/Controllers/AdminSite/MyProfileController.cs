using AspNetCoreHero.ToastNotification.Abstractions;
using HelloDocAdmin.Controllers.Authenticate;
using HelloDocAdmin.Entity.ViewModels.AdminSite;
using HelloDocAdmin.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;

namespace HelloDocAdmin.Controllers.AdminSite
{
    [CustomAuthorization("Admin")]
    public class MyProfileController : Controller
    {
        private IDashboardRepository _dashboardrepo;
        private ICombobox _combobox;
        private IAdminProfile _adminProfile;
        private readonly ILogger<DashboardController> _logger;
        private readonly INotyfService _notyf;
        public MyProfileController(ILogger<DashboardController> logger, IDashboardRepository dashboardRepository, ICombobox combobox, IAdminProfile adminprofile, INotyfService notyf)
        {
            _logger = logger;
            _dashboardrepo = dashboardRepository;
            _combobox = combobox;
            _adminProfile = adminprofile;
            _notyf = notyf;
        }

        public async Task<IActionResult> IndexAsync()
        {
            ViewBag.userrolecombobox = await _combobox.UserRole();
            ViewBag.RegionComboBox = await _combobox.RegionComboBox();
            ViewAdminProfileModel vm = _adminProfile.GetDetailsForAdminProfile(CV.LoggedUserID());
            return View("../AdminSite/MyProfile/Index", vm);
        }
        [HttpPost]
        public IActionResult SaveAdministrationinfo(ViewAdminProfileModel vm)
        {


            vm.AdminReqionList = Request.Form["SelectedRegions"].Select(int.Parse).ToList();
            bool data = _adminProfile.Edit_Admin_Profile(vm, CV.LoggedUserID());
            if (data)
            {
                _notyf.Success("Administration Information Changed...");
            }
            else
            {
                _notyf.Error("Imformation not Changed properly...");
            }

            return RedirectToAction("Index");
        }
        public IActionResult EditBillingInfo(ViewAdminProfileModel vm)
        {



            bool data = _adminProfile.Edit_Billing_Info(vm, CV.LoggedUserID());
            if (data)
            {
                _notyf.Success("Billing Information Changed...");
            }
            else
            {
                _notyf.Error("Billing not Changed properly...");
            }

            return RedirectToAction("Index");
        }

        public IActionResult ResetPassAdmin(string password)
        {



            bool data = _adminProfile.ChangePassword(password, CV.LoggedUserID());
            if (data)
            {
                _notyf.Success("Password changed Successfully...");
            }
            else
            {
                _notyf.Error("Password not Changed...");
            }

            return RedirectToAction("Index");
        }
    }
}

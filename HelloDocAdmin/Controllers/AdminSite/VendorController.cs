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
    [CustomAuthorization("Admin", "Vendor")]
    public class VendorController : Controller
    {
        private IActionRepository _actionrepo;
        private IDashboardRepository _dashboardrepo;
        private ICombobox _combobox;
        private readonly ILogger<DashboardController> _logger;
        private readonly INotyfService _notyf;
        private readonly EmailConfiguration _email;
        private readonly IVendorRepository _vendorrepo;
        public VendorController(ILogger<DashboardController> logger, IVendorRepository vendorrepo, IDashboardRepository dashboardRepository, ICombobox combobox, IActionRepository actionrepo, INotyfService notyf, EmailConfiguration email)
        {
            _logger = logger;
            _combobox = combobox;
            _dashboardrepo = dashboardRepository;
            _actionrepo = actionrepo;
            _notyf = notyf;
            _email = email;
            _vendorrepo = vendorrepo;
        }
        public async Task<IActionResult> Index()
        {
            List<HealthprofessionaltypeCombobox> cs = await _combobox.healthprofessionaltype();
            ViewBag.Healthprofessionaltype = cs;
            return View("../AdminSite/Partner/Index");
        }
        public async Task<IActionResult> VendorList(string? vendorName, int? healthproffesionID, int pagesize = 5, int currentpage = 1)
        {
            VendorData sr = _vendorrepo.getallvendor(vendorName, healthproffesionID, pagesize, currentpage);
            return PartialView("../AdminSite/Partner/_VendorList", sr);
        }
        public async Task<IActionResult> VendorAddEdit(int vendorid)
        {
            List<HealthprofessionaltypeCombobox> cs = await _combobox.healthprofessionaltype();
            ViewBag.ProfessionType = cs;
            if (vendorid != null)
            {
                Healthprofessional hp = _vendorrepo.gethelthprofessionaldetails(vendorid);
                return View("../AdminSite/Partner/AddEditVendor", hp);
            }
            else
            {
                return View("../AdminSite/Partner/AddEditVendor");
            }

        }


        public async Task<IActionResult> DeleteVendor(int? vendorid)
        {
            if (_vendorrepo.delete(vendorid))
            {
                _notyf.Success("Vendor Deleted successfully");

            }
            else
            {
                _notyf.Error("Vendor Not Deleted ");
            }

            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Save(Healthprofessional model)
        {
            List<HealthprofessionaltypeCombobox> cs = await _combobox.healthprofessionaltype();
            ViewBag.ProfessionType = cs;
            if (ModelState.IsValid)
            {
                if (model.Vendorid == null)
                {
                    if (_vendorrepo.isBusinessNameExist(model.Vendorname) > 0)
                    {
                        ModelState.AddModelError("Vendorname", "Business name is Already Taken!! choose another one");
                        return View("../AdminSite/Partner/AddEditVendor", model);
                    }
                    if (_vendorrepo.isEmailExist(model.Email) > 0)
                    {
                        ModelState.AddModelError("Email", "Email is Already Taken!! choose another one");
                        return View("../AdminSite/Partner/AddEditVendor", model);
                    }

                    bool data = _vendorrepo.addVendor(model);
                    if (data)
                    {
                        _notyf.Success("Vendor added successfully");
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        _notyf.Error("Vendor Not added");
                        return View("../AdminSite/Partner/AddEditVendor", model);
                    }
                }
                else
                {
                    bool data = _vendorrepo.EditVendor(model);
                    if (_vendorrepo.isBusinessNameExist(model.Vendorname) > 1)
                    {
                        ModelState.AddModelError("Vendorname", "Business name is Already Taken!! choose another one");
                        return View("../AdminSite/Partner/AddEditVendor", model);
                    }
                    if (_vendorrepo.isEmailExist(model.Email) > 1)
                    {
                        ModelState.AddModelError("Email", "Email is Already Taken!! choose another one");
                        return View("../AdminSite/Partner/AddEditVendor", model);
                    }
                    if (data)
                    {
                        _notyf.Success("Vendor Edited successfully");
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        _notyf.Error("Vendor Not Edited");
                        return View("../AdminSite/Partner/AddEditVendor", model);
                    }
                }



            }
            else
            {
                _notyf.Error("enter valid data");
                return View("../AdminSite/Partner/AddEditVendor", model);
            }
        }
    }
}

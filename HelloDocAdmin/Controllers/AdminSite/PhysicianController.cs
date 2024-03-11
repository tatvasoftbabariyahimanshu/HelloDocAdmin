using AspNetCoreHero.ToastNotification.Abstractions;
using HelloDocAdmin.Entity.Data;
using HelloDocAdmin.Repositories;
using HelloDocAdmin.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;

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
    }
}

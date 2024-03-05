using AspNetCoreHero.ToastNotification.Abstractions;
using HelloDocAdmin.Entity.Data;
using HelloDocAdmin.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;

namespace HelloDocAdmin.Controllers.AdminSite
{
    public class ProviderLocationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailConfiguration _email;
        private readonly INotyfService _notyf;
        private readonly IProviderLocation _iploc;
        public ProviderLocationController(ApplicationDbContext context, EmailConfiguration email, INotyfService notyf,IProviderLocation iploc)
        {
            _context = context;
            _email = email;
            _notyf = notyf;
            _iploc = iploc;
        }

        public IActionResult Index()
        {
            var list=_iploc.GetAllProviderAddress();
            return View("../AdminSite/ProviderLocation/Index",list);
        }
    }
}

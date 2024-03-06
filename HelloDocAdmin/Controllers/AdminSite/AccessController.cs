using Microsoft.AspNetCore.Mvc;

namespace HelloDocAdmin.Controllers.AdminSite
{
    [CustomAuthorization("Administrator")]
    public class AccessController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

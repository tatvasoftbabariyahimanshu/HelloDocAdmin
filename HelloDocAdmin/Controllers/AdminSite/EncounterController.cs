using Microsoft.AspNetCore.Mvc;

namespace HelloDocAdmin.Controllers.AdminSite
{
    public class EncounterController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

using Microsoft.AspNetCore.Mvc;

namespace HelloDocAdmin.Controllers.AdminSite
{
    public class RecordsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult SearchRecord()
        {
            return View("../AdminSite/Records/SearchRecords");
        }
    }
}

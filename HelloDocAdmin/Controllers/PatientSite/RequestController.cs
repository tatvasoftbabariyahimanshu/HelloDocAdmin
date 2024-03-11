using Microsoft.AspNetCore.Mvc;

namespace HelloDocAdmin.Controllers.PatientSite
{
    public class RequestController : Controller
    {
        public IActionResult Index()
        {
            return View("../PatientSite/Request/SubmitRequest");
        }
    }
}

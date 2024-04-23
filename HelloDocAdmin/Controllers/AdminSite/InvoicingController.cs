using HelloDocAdmin.Controllers.Authenticate;
using Microsoft.AspNetCore.Mvc;

namespace HelloDocAdmin.Controllers.AdminSite
{
    public class InvoicingController : Controller
    {
        [CustomAuthorization("Admin,Physician", "Invoicing")]
        public IActionResult Index()
        {
            return View("../Invoicing/PhysicianInvoicingIndex");
        }
    }
}

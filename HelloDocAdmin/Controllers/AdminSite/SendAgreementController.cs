using AspNetCoreHero.ToastNotification.Abstractions;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using HelloDocAdmin.Entity.Data;
using HelloDocAdmin.Entity.Models;
using Microsoft.AspNetCore.Mvc;

namespace HelloDocAdmin.Controllers.AdminSite
{
    public class SendAgreementController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailConfiguration _email;
        private readonly INotyfService _notyf;

        public SendAgreementController(ApplicationDbContext context, EmailConfiguration email,INotyfService notyf)
        {
            _context = context;
            _email = email;
            _notyf = notyf;

        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult SendAgreement(int requestid,string email)
        {
            var res=_context.Requests.FirstOrDefault(e=>e.Requestid == requestid);

            var agreementUrl = Url.Action("Agreement", "SendAgreement", new { area = "", RequestID = requestid }, Request.Scheme);

            if(_email.SendMail(email, "Agreement for your request", $"<a href='{agreementUrl}'>Agree/Disagree</a>"))
            {
                _notyf.Success("Agreement Sent to patient...");
                return RedirectToAction("Index", "Dashboard");
            }
            else
            {
                _notyf.Error("Check Valid Data for send Agreement...");
                return RedirectToAction("Index", "Dashboard");
            }
            


         
        }
        #region agreement

        public IActionResult agreement(int RequestID)
        {
            var request = _context.Requests.Find(RequestID);
            TempData["RequestID"]=RequestID;
            TempData["PatientName"] = request.Firstname + " " + request.Lastname;
            return View("Agreement");
        }
        public IActionResult accept(int RequestID)
        {
            var request=_context.Requests.Find(RequestID);
            if (request!=null)
            {
                request.Status = 4;
                _context.Requests.Update(request);
                _context.SaveChanges();

                Requeststatuslog rsl = new Requeststatuslog();
                rsl.Requestid = RequestID;

                rsl.Status = 4;

                rsl.Createddate = DateTime.Now;
              
                _context.Requeststatuslogs.Add(rsl);
                _context.SaveChanges();

            }
            return RedirectToAction("Index", "Dashboard");
        }
        public IActionResult Reject(int RequestID,string Notes)
        {
            var request = _context.Requests.Find(RequestID);
            if (request != null)
            {
                request.Status = 7;
                _context.Requests.Update(request);
                _context.SaveChanges();

                Requeststatuslog rsl = new Requeststatuslog();
                rsl.Requestid = RequestID;

                rsl.Status = 7;
                rsl.Notes= Notes;

                rsl.Createddate = DateTime.Now;

                _context.Requeststatuslogs.Add(rsl);
                _context.SaveChanges();

            }
            return RedirectToAction("Index", "Dashboard");
        }
        #endregion
    }
}

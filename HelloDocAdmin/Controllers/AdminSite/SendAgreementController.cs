using AspNetCoreHero.ToastNotification.Abstractions;
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

        public SendAgreementController(ApplicationDbContext context, EmailConfiguration email, INotyfService notyf)
        {
            _context = context;
            _email = email;
            _notyf = notyf;

        }
        public IActionResult Index()
        {
            return View();
        }

        #region agreement
        public IActionResult SendAgreement(int requestid, string email)
        {
            var res = _context.Requests.FirstOrDefault(e => e.Requestid == requestid);

            // Encode RequestID to Base64
            byte[] idBytes = BitConverter.GetBytes(requestid);
            string base64String = Convert.ToBase64String(idBytes);

            var agreementUrl = Url.Action("Agreement", "SendAgreement", new { area = "", RequestID = base64String }, Request.Scheme);

            if (_email.SendMail(email, "Agreement for your request", $"<a href='{agreementUrl}'>Agree/Disagree</a>"))
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

        public IActionResult Agreement(string RequestID)
        {
            try
            {
                // Decode Base64 back to byte array
                byte[] decodedBytes = Convert.FromBase64String(RequestID);

                if (decodedBytes.Length != 4) // Ensure the length of the byte array is exactly 4
                {
                    // Handle error, maybe return a BadRequest or log it
                    return BadRequest("Invalid RequestID");
                }

                int decodedRequestID = BitConverter.ToInt32(decodedBytes, 0);

                var request = _context.Requests.Find(decodedRequestID);
                TempData["RequestID"] = decodedRequestID;
                TempData["PatientName"] = request.Firstname + " " + request.Lastname;
                return View("Agreement");
            }
            catch (FormatException)
            {
                // Handle invalid Base64 string error
                return BadRequest("Invalid Base64 encoded RequestID");
            }
        }

        public IActionResult accept(int RequestID)
        {
            var request = _context.Requests.Find(RequestID);
            if (request != null)
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
        public IActionResult Reject(int RequestID, string Notes)
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
                rsl.Notes = Notes;

                rsl.Createddate = DateTime.Now;

                _context.Requeststatuslogs.Add(rsl);
                _context.SaveChanges();

            }
            return RedirectToAction("Index", "Dashboard");
        }
        #endregion
    }
}

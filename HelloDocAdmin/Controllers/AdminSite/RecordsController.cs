using AspNetCoreHero.ToastNotification.Abstractions;
using HelloDocAdmin.Controllers.Authenticate;
using HelloDocAdmin.Entity.Data;
using HelloDocAdmin.Entity.ViewModels.AdminSite;
using HelloDocAdmin.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;

namespace HelloDocAdmin.Controllers.AdminSite
{
    [CustomAuthorization("Admin", "SearchRecords")]
    [CustomAuthorization("Admin", "Records")]
    public class RecordsController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly INotyfService _notyf;
        private readonly ISearchRecords _searchrecords;


        public RecordsController(ISearchRecords searchRecords, INotyfService notyf)
        {

            _searchrecords = searchRecords;
            _notyf = notyf;

        }

        public IActionResult Index()
        {
            return View();
        }
        [CustomAuthorization("Admin", "SearchRecords")]
        public IActionResult SearchRecord()
        {
            return View("../AdminSite/Records/SearchRecord");
        }
        [CustomAuthorization("Admin", "SearchRecords")]
        public async Task<IActionResult> FilterRequest(short status, string patientname, int requesttype, DateTime startdate, DateTime enddate, string physicianname, string email, string phonenumber, int pagesize = 10, int currentpage = 1)
        {
            RequestRecords sr = await _searchrecords.GetRequestsbyfilterForRecords(status, patientname, requesttype, startdate, enddate, physicianname, email, phonenumber, currentpage, pagesize);
            return PartialView("../AdminSite/Records/_requestPartials", sr);
        }
        [CustomAuthorization("Admin", "PatientRecord")]
        public IActionResult PatientRecord()
        {
            return View("../AdminSite/Records/SearchPatient");
        }
        [CustomAuthorization("Admin", "PatientRecord")]
        public async Task<IActionResult> FilterPatient(string firstname, string lastname, string email, string phonenumber, int pagesize = 5, int currentpage = 1)
        {
            PatientHistory sr = await _searchrecords.Patienthistorybyfilter(firstname, lastname, email, phonenumber, currentpage, pagesize);
            return PartialView("../AdminSite/Records/_PatientsPartials", sr);
        }
        [CustomAuthorization("Admin", "PatientRecord")]
        public async Task<IActionResult> PatientRecords(int userid, int pagesize = 5, int currentpage = 1)
        {
            PatientRecordsView sr = await _searchrecords.PatientRecordsViewBy(userid, currentpage, pagesize);

            return View("../AdminSite/Records/PatientRecords", sr);
        }
        public async Task<IActionResult> PatientRecordsList(int userid, int pagesize = 5, int currentpage = 1)
        {
            PatientRecordsView sr = await _searchrecords.PatientRecordsViewBy(userid, currentpage, pagesize);
            return PartialView("../AdminSite/Records/_PatientRecordsList", sr);
        }
        [CustomAuthorization("Admin", "EmailLogs")]
        public IActionResult EmailLogs()
        {
            return View("../AdminSite/Records/EmailLogs");
        }
        [CustomAuthorization("Admin", "EmailLogs")]
        public async Task<IActionResult> EmailLogsData(int accounttype, string email, string ReciverName, DateTime CreatedDate, DateTime SendDate, int pagesize = 10, int currentpage = 1)
        {
            EmailRecords sr = await _searchrecords.EmailLogs(accounttype, email, ReciverName, CreatedDate, SendDate, pagesize, currentpage);
            return PartialView("../AdminSite/Records/_emaillogslist", sr);
        }
        [CustomAuthorization("Admin", "SMSLogs")]
        public IActionResult SMSLogs()
        {
            return View("../AdminSite/Records/SMSLogs");
        }
        [CustomAuthorization("Admin", "SMSLogs")]
        public async Task<IActionResult> SMSLogsData(int accounttype, string phonenumber, string ReciverName, DateTime CreatedDate, DateTime SendDate, int pagesize = 5, int currentpage = 1)
        {
            SMSLogs sr = await _searchrecords.SMSLogs(accounttype, phonenumber, ReciverName, CreatedDate, SendDate, pagesize, currentpage);
            return PartialView("../AdminSite/Records/_smslogslist", sr);
        }
        [CustomAuthorization("Admin", "BlockHistory")]
        public IActionResult BlockHistory()
        {
            return View("../AdminSite/Records/BlockHistory");
        }
        [CustomAuthorization("Admin", "BlockHistory")]
        public async Task<IActionResult> BlockHistoryData(string name, string email, string phonenumber, DateTime CreatedDate, int pagesize = 5, int currentpage = 1)
        {
            BlockRequest sr = await _searchrecords.BlockHistory(name, email, phonenumber, CreatedDate, pagesize, currentpage);
            return PartialView("../AdminSite/Records/_BlockHistoryData", sr);
        }
        [CustomAuthorization("Admin", "BlockHistory")]
        public async Task<IActionResult> UnBlock(int RequestId)
        {


            bool UnBlock = await _searchrecords.UnBlock(RequestId, CV.LoggedUserID());
            if (UnBlock)
            {
                _notyf.Success("UnBlock Request Successfully");

            }
            else
            {
                _notyf.Error("UnBlock Request Canceled");

            }

            return View("../AdminSite/Records/BlockHistory");

        }
        public async Task<IActionResult> Delete(int RequestId)
        {


            bool UnBlock = _searchrecords.Delete(RequestId, CV.LoggedUserID());
            if (UnBlock)
            {
                _notyf.Success(" Request Deleted Successfully");

            }
            else
            {
                _notyf.Error("Request Not Deleted ");

            }

            return RedirectToAction("SearchRecord");

        }

    }
}

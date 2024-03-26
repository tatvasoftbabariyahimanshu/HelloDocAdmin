using AspNetCoreHero.ToastNotification.Abstractions;
using HelloDocAdmin.Controllers.Authenticate;
using HelloDocAdmin.Entity.Data;
using HelloDocAdmin.Entity.ViewModels.AdminSite;
using HelloDocAdmin.Repositories;
using HelloDocAdmin.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;

namespace HelloDocAdmin.Controllers.AdminSite
{
    [CustomAuthorization("Admin")]
    public class RecordsController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly INotyfService _notyf;
        private readonly ISearchRecords _searchrecords;


        public RecordsController(ISearchRecords searchRecords, INotyfService notyf)
        {

            _searchrecords=searchRecords;
             _notyf = notyf;

        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult SearchRecord()
        {
            return View("../AdminSite/Records/SearchRecord");
        }
        public async Task<IActionResult> FilterRequest(short status,string patientname,int requesttype,DateTime startdate, DateTime enddate,string physicianname,string email,string phonenumber, int pagesize = 5, int currentpage = 1) {
            RequestRecords sr = await _searchrecords.GetRequestsbyfilterForRecords(status, patientname, requesttype, startdate, enddate, physicianname, email, phonenumber, currentpage, pagesize);
            return PartialView("../AdminSite/Records/_requestPartials" , sr);
        }



    }
}

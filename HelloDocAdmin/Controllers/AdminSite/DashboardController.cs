using HelloDocAdmin.Controllers.Authenticate;
using HelloDocAdmin.Entity.Models;
using HelloDocAdmin.Entity.ViewModels;
using HelloDocAdmin.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;


namespace HelloDocAdmin.Controllers.AdminSite
{
    [CustomAuthorization("Admin")]
    public class DashboardController : Controller
    {
        private IDashboardRepository _dashboardrepo;
        private ICombobox _combobox;
        private readonly ILogger<DashboardController> _logger;
        public DashboardController(ILogger<DashboardController> logger, IDashboardRepository dashboardRepository,ICombobox combobox)
        {
            _logger = logger;
            _dashboardrepo = dashboardRepository;
          _combobox= combobox;
        }

    
       


        public async Task<IActionResult> Index()
        {

     
            ViewBag.RegionComboBox = await _combobox.RegionComboBox();
            ViewBag.CaseReasonComboBox = await _combobox.CaseReasonComboBox();
            DashboardCardsModel model = new DashboardCardsModel();
            model.PandingRequests = _dashboardrepo.GetRequestNumberByStatus("2");
            model.NewRequests = _dashboardrepo.GetRequestNumberByStatus("1");
            model.ActiveRequests = _dashboardrepo.GetRequestNumberByStatus("4,5");
            model.ConcludeRequests = _dashboardrepo.GetRequestNumberByStatus("6");
            model.ToCloseRequests = _dashboardrepo.GetRequestNumberByStatus("3,7,8");
            model.UnpaidRequests = _dashboardrepo.GetRequestNumberByStatus("9");

            return View("../AdminSite/Dashboard/Index",model);
        }
        #region providerbyregion
        public  IActionResult ProviderbyRegion(int? Regionid)
        {
            var v =  _combobox.ProviderbyRegion(Regionid);
            return Json(v);
        }
        #endregion

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _SearchResult(string Status)
        {
             if(Status==null)
            {
                Status = CV.CurrentStatus();
            }

            Response.Cookies.Delete("Status");
            Response.Cookies.Append("Status", Status);
            List<DashboardRequestModel> contacts = _dashboardrepo.GetRequests(Status);
            TempData["CurrentStatusinlist"] = Status;

            switch (Status)
            {
                case "1":
                    TempData["CurrentStatus"] = "New";
                  

                    break;
                case "2":
                    TempData["CurrentStatus"] = "Panding";
                 
                    break;
                case "4,5":
                    TempData["CurrentStatus"] = "Active";
               
                    break;
                case "6":
                    TempData["CurrentStatus"] = "Conclude";
               
                    break;
                case "3,7,8":
                    TempData["CurrentStatus"] = "To Close";
               
                    break;
                case "9":
                    TempData["CurrentStatus"] = "Unpaid";
                  
                    break;
            }

            switch (Status)
            {
                case "1":
                  
                    return PartialView("../AdminSite/Dashboard/_newList", contacts);

                    break;
                case "2":

                  
                    return PartialView("../AdminSite/Dashboard/_pandingList", contacts);
                    break;
                case "4,5":
                  
                    return PartialView("../AdminSite/Dashboard/_activeList", contacts);
                    break;
                case "6":
              
                    return PartialView("../AdminSite/Dashboard/_concludeList", contacts);
                    break;
                case "3,7,8":
                   
                    return PartialView("../AdminSite/Dashboard/_toCloseList", contacts);
                    break;
                case "9":
          
                    return PartialView("../AdminSite/Dashboard/_toUnpaidList", contacts);
                    break;
            }
           

            return PartialView("../AdminSite/Dashboard/nodata", contacts);
        }

    }
}

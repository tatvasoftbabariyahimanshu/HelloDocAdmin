using HelloDocAdmin.Entity.Models;
using HelloDocAdmin.Entity.ViewModels;
using HelloDocAdmin.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;


namespace HelloDocAdmin.Controllers.AdminSite
{
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
            model.PandingRequests = _dashboardrepo.GetRequestNumberByStatus(2);
            model.NewRequests = _dashboardrepo.GetRequestNumberByStatus(1);
            model.ActiveRequests = _dashboardrepo.GetRequestNumberByStatus(3);
            model.ConcludeRequests = _dashboardrepo.GetRequestNumberByStatus(4);
            model.ToCloseRequests = _dashboardrepo.GetRequestNumberByStatus(5);
            model.UnpaidRequests = _dashboardrepo.GetRequestNumberByStatus(6);

            return View("../AdminSite/Dashboard/Index",model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _SearchResult(short Status)
        {
         
            List<DashboardRequestModel> contacts = _dashboardrepo.GetRequests(Status);
            switch (Status)
            {
                case 1:
                    TempData["CurrentStatus"] = "New";
                  

                    break;
                case 2:
                    TempData["CurrentStatus"] = "Panding";
                 
                    break;
                case 3:
                    TempData["CurrentStatus"] = "Active";
               
                    break;
                case 4:
                    TempData["CurrentStatus"] = "Conclude";
               
                    break;
                case 5:
                    TempData["CurrentStatus"] = "To Close";
               
                    break;
                case 6:
                    TempData["CurrentStatus"] = "Unpaid";
                  
                    break;
            }

            switch (Status)
            {
                case 1:
                    return PartialView("../AdminSite/Dashboard/_newList", contacts);

                    break;
                case 2:
                  
                    return PartialView("../AdminSite/Dashboard/_pandingList", contacts);
                    break;
                case 3:
                
                    return PartialView("../AdminSite/Dashboard/_activeList", contacts);
                    break;
                case 4:
                  
                    return PartialView("../AdminSite/Dashboard/_concludeList", contacts);
                    break;
                case 5:
                  
                    return PartialView("../AdminSite/Dashboard/_toCloseList", contacts);
                    break;
                case 6:
                 
                    return PartialView("../AdminSite/Dashboard/_toUnpaidList", contacts);
                    break;
            }
           

            return PartialView("../AdminSite/Dashboard/nodata", contacts);
        }

    }
}

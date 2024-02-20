using HelloDocAdmin.Entity.Models;
using HelloDocAdmin.Entity.ViewModels;
using HelloDocAdmin.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;


namespace HelloDocAdmin.Controllers
{
    public class DashboardController : Controller
    {
        private IDashboardRepository _dashboardrepo;
        private readonly ILogger<DashboardController> _logger;
        public DashboardController(ILogger<DashboardController> logger, IDashboardRepository dashboardRepository)
        {
            _logger = logger;
            _dashboardrepo = dashboardRepository;
        }
        public IActionResult Index()
        {

            DashboardCardsModel model = new DashboardCardsModel();
            model.PandingRequests = _dashboardrepo.GetRequestNumberByStatus(2);
            model.NewRequests = _dashboardrepo.GetRequestNumberByStatus(1);
            model.ActiveRequests = _dashboardrepo.GetRequestNumberByStatus(3);
            model.ConcludeRequests = _dashboardrepo.GetRequestNumberByStatus(4);
            model.ToCloseRequests= _dashboardrepo.GetRequestNumberByStatus(5);
            model.UnpaidRequests=_dashboardrepo.GetRequestNumberByStatus(6);

            return View(model);
        }
  
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _SearchResult(short Status)
        {
          switch(Status)
            {
                case 1:
                    ViewData["CurrentStatus"] = "New";
                    break;
                case 2:
                    ViewData["CurrentStatus"] = "Panding";
                    break;
                case 3:
                    ViewData["CurrentStatus"] = "Active";
                    break;
                case 4:
                    ViewData["CurrentStatus"] = "Conclude";
                    break;
                case 5:
                    ViewData["CurrentStatus"] = "To Close";
                    break;
                case 6:
                    ViewData["CurrentStatus"] = "Unpaid";
                    break;
            }
                 
            List<DashboardRequestModel> contacts =_dashboardrepo.GetRequests(Status);
           
            return PartialView("_LawerSection", contacts);
        }
   
    }
}

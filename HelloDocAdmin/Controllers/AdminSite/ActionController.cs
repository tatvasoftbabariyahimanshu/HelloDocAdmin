using HelloDocAdmin.Entity.ViewModels.AdminSite;
using HelloDocAdmin.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;

namespace HelloDocAdmin.Controllers.AdminSite
{
    public class ActionController : Controller
    {
        private IDashboardRepository _dashboardrepo;
        private readonly ILogger<DashboardController> _logger;
        public ActionController(ILogger<DashboardController> logger, IDashboardRepository dashboardRepository)
        {
            _logger = logger;
            _dashboardrepo = dashboardRepository;
        }
        public IActionResult ViewCase(int id)
        {
            ViewCaseModel sm=_dashboardrepo.GetRequestForViewCase(id);
           
            return View("../AdminSite/Action/ViewCase",sm);
        }
        public IActionResult ViewNotes(int id)
        {

            ViewNotesModel sm = _dashboardrepo.getNotesByID(id);
            return View("../AdminSite/Action/ViewNotes",sm);
        }


        public IActionResult EditCase(ViewCaseModel sm)
        {
            bool result=_dashboardrepo.EditCase(sm);

            if(result)
            {
                return RedirectToAction("ViewCase",new {id=sm.RequestID});
            }
            else
            {
                return View("../AdminSite/Action/ViewCase", sm);
            }
            
        }

    }
}

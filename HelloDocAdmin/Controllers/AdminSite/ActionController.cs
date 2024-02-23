using HelloDocAdmin.Entity.Models;
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
        public IActionResult ViewDocuments(int id)
        {

            ViewDocumentsModel sm = _dashboardrepo.ViewDocument(id);
            return View("../AdminSite/Action/ViewDocuments", sm);
        }
        public IActionResult UploadDocuments(int id, IFormFile? UploadFile)
        {

           bool sm = _dashboardrepo.UploadDoc(id, UploadFile);
           
            return RedirectToAction("ViewDocuments", new { id });
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
        [HttpPost]
        public IActionResult ChangeNotes(int? RequestID,string? adminnotes,string? physiciannotes)
        {
            if(adminnotes!=null || physiciannotes!=null)
            {
                bool result = _dashboardrepo.EditViewNotes(adminnotes, physiciannotes, RequestID);
                if (result)
                {
                    return RedirectToAction("ViewNotes", new { id = RequestID });
                }
                else
                {
                    return View("../AdminSite/Action/ViewNotes");
                }
            }
            else
            {
                TempData["Errormassage"] = "Please Select one of the note!!";
                return RedirectToAction("ViewNotes", new { id = RequestID });
            }
              

        

        }

    }
}

using AspNetCoreHero.ToastNotification.Abstractions;
using HelloDocAdmin.Entity.Models;
using HelloDocAdmin.Entity.ViewModels.AdminSite;
using HelloDocAdmin.Repositories;
using HelloDocAdmin.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;

namespace HelloDocAdmin.Controllers.AdminSite
{
    public class ActionController : Controller
    {
        private IActionRepository _actionrepo;
        private IDashboardRepository _dashboardrepo;
        private ICombobox _combobox;
        private readonly ILogger<DashboardController> _logger;
        private readonly INotyfService _notyf;
        public ActionController(ILogger<DashboardController> logger, IDashboardRepository dashboardRepository, ICombobox combobox,IActionRepository actionrepo, INotyfService notyf)
        {
            _logger = logger;
            _combobox = combobox;
            _dashboardrepo = dashboardRepository;
            _actionrepo = actionrepo;
            _notyf = notyf;
        }
        public async Task<IActionResult> ViewCase(int id)
        {
            ViewBag.RegionComboBox =await  _combobox.RegionComboBox();
            ViewBag.CaseReasonComboBox =await  _combobox.CaseReasonComboBox();
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
            if(sm)
            {

                _notyf.Success("Document Uploaded Successfully");
            }
            else
            {
                _notyf.Error("Document not Uploaded");

            }

            return RedirectToAction("ViewDocuments", new { id });
        }


        public IActionResult EditCase(ViewCaseModel sm)
        {
            bool result=_dashboardrepo.EditCase(sm);

            if(result)
            {
                _notyf.Success("Case Edited Successfully..");
                return RedirectToAction("ViewCase",new {id=sm.RequestID});
            }
            else
            {
                _notyf.Success("Case Not Edited...");
                return View("../AdminSite/Action/ViewCase", sm);
            }
            
        }
        public IActionResult SendLink(string firstname, string lastname, string email, string phonenumber)
        {
            if (_dashboardrepo.SendLink(firstname, lastname, email, phonenumber))
            {

                _notyf.Success("Email Sended to "+ firstname);
            }
            else
            {
                _notyf.Error("Email Not Sended");
            }
            return RedirectToAction("Index", "Dashboard");
        }
        #region AssignProvider
        public async Task<IActionResult> AssignProvider(int requestid, int ProviderId, string Notes)
        {
            if (await _dashboardrepo.AssignProvider(requestid, ProviderId, Notes))
            {
                _notyf.Success("Physician Assigned successfully...");
            }
            else
            {
                _notyf.Error("Physician Not Assigned...");
            }

            return RedirectToAction("Index", "Dashboard");
        }
        #endregion
        #region TransferProvider
        public async Task<IActionResult> TransferProvider(int requestid, int ProviderId, string Notes)
        {
            if (await _actionrepo.TransferProvider(requestid, ProviderId, Notes))
            {
                _notyf.Success("Physician Transfered successfully...");
            }
            else
            {
                _notyf.Error("Physician Not Transfered...");
            }

            return RedirectToAction("Index", "Dashboard");
        }
        #endregion

        [HttpPost]
        public IActionResult ChangeNotes(int RequestID,string? adminnotes,string? physiciannotes)
        {
            if(adminnotes!=null || physiciannotes!=null)
            {
                bool result = _dashboardrepo.EditViewNotes(adminnotes, physiciannotes, RequestID);
                if (result)
                {
                    _notyf.Success("Notes Updated successfully...");
                    return RedirectToAction("ViewNotes", new { id = RequestID });
                }
                else
                {
                    _notyf.Error("Notes Note Updated");
                    return View("../AdminSite/Action/ViewNotes");
                }
            }
            else
            {
                _notyf.Information("Please Select one of the note!!");
                TempData["Errormassage"] = "Please Select one of the note!!";
                return RedirectToAction("ViewNotes", new { id = RequestID });
            }
              

        

        }

        #region _CancelCase
        public IActionResult CancelCase(int RequestID,string Note,string CaseTag)
        {


            bool CancelCase=_dashboardrepo.CancelCase(RequestID, Note, CaseTag);
            if (CancelCase)
            {
                _notyf.Success("Case Canceled Successfully");

            }
            else
            {
                _notyf.Error("Case Not Canceled");

            }

            return RedirectToAction("Index", "Dashboard");

        }
        public IActionResult BlockCase(int RequestID, string Note)
        {


            bool BlockCase = _dashboardrepo.BlockCase(RequestID, Note);
             if (BlockCase)
            {
                _notyf.Success("Case Blocked Successfully");

            }
            else
            {
                _notyf.Error("Case Not Blocked");

            }

            return RedirectToAction("Index", "Dashboard");

        }
        #endregion
        #region Delete Doc
        public IActionResult DeleteDoc(int RequestWiseFileID , int RequestID)
        {


            bool data = _actionrepo.DeleteDoc(RequestWiseFileID);
           if(data)
            {
                _notyf.Success("Documet Deleted Successfully");
            }
            else
            {
                _notyf.Error("Documet Not Deleted");
            }
           
            return RedirectToAction("ViewDocuments", new { id = RequestID });

        }
        #endregion
    }
}

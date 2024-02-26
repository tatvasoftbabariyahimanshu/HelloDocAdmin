﻿using HelloDocAdmin.Entity.Models;
using HelloDocAdmin.Entity.ViewModels.AdminSite;
using HelloDocAdmin.Repositories;
using HelloDocAdmin.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;

namespace HelloDocAdmin.Controllers.AdminSite
{
    public class ActionController : Controller
    {
        private IDashboardRepository _dashboardrepo;
        private ICombobox _combobox;
        private readonly ILogger<DashboardController> _logger;
        public ActionController(ILogger<DashboardController> logger, IDashboardRepository dashboardRepository, ICombobox combobox)
        {
            _logger = logger;
            _combobox = combobox;
            _dashboardrepo = dashboardRepository;
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
        public IActionResult SendLink(string firstname, string lastname, string email, string phonenumber)
        {
            if (_dashboardrepo.SendLink(firstname, lastname, email, phonenumber))
            {

                TempData["Status"] = "Link Send In mail Successfully..!";
            }
            return RedirectToAction("Index", "Dashboard");
        }
        #region AssignProvider
        public async Task<IActionResult> AssignProvider(int requestid, int ProviderId, string Notes)
        {
            if (await _dashboardrepo.AssignProvider(requestid, ProviderId, Notes))
            {
                TempData["Status"] = "Assign Provider Successfully..!";
            }

            return RedirectToAction("Index", "Dashboard");
        }
        #endregion
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

        #region _CancelCase
        public IActionResult CancelCase(int RequestID,string Note,string CaseTag)
        {


            bool CancelCase=_dashboardrepo.CancelCase(RequestID, Note, CaseTag);

            return RedirectToAction("Index", "Dashboard");

        }
        public IActionResult BlockCase(int RequestID, string Note)
        {


            bool BlockCase = _dashboardrepo.BlockCase(RequestID, Note);

            return RedirectToAction("Index", "Dashboard");

        }
        #endregion
    }
}

using AspNetCore;
using AspNetCoreHero.ToastNotification.Abstractions;
using DocumentFormat.OpenXml.Drawing;
using HelloDocAdmin.Entity.Data;
using HelloDocAdmin.Entity.Models;
using HelloDocAdmin.Entity.ViewModels;
using HelloDocAdmin.Entity.ViewModels.AdminSite;
using HelloDocAdmin.Repositories;
using HelloDocAdmin.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;

namespace HelloDocAdmin.Controllers.AdminSite
{
    public class ActionController : Controller
    {
        private IActionRepository _actionrepo;
        private IDashboardRepository _dashboardrepo;
        private ICombobox _combobox;
        private readonly ILogger<DashboardController> _logger;
        private readonly INotyfService _notyf;
        private readonly EmailConfiguration _email;
        public ActionController(ILogger<DashboardController> logger, IDashboardRepository dashboardRepository, ICombobox combobox, IActionRepository actionrepo, INotyfService notyf, EmailConfiguration email)
        {
            _logger = logger;
            _combobox = combobox;
            _dashboardrepo = dashboardRepository;
            _actionrepo = actionrepo;
            _notyf = notyf;
            _email = email;
        }
        public async Task<IActionResult> ViewCase(int id)
        {
            ViewBag.RegionComboBox = await _combobox.RegionComboBox();
            ViewBag.CaseReasonComboBox = await _combobox.CaseReasonComboBox();
            ViewCaseModel sm = _dashboardrepo.GetRequestForViewCase(id);

            return View("../AdminSite/Action/ViewCase", sm);
        }
        public IActionResult ViewNotes(int id)
        {

            ViewNotesModel sm = _dashboardrepo.getNotesByID(id);
            return View("../AdminSite/Action/ViewNotes", sm);
        }
        public IActionResult ViewDocuments(int id)
        {
            if(TempData["Mail"]!=null)
            {
                _notyf.Success(TempData["Mail"].ToString());
            }
          
            ViewDocumentsModel sm = _dashboardrepo.ViewDocument(id);
            return View("../AdminSite/Action/ViewDocuments", sm);
        }
        public IActionResult UploadDocuments(int id, IFormFile? UploadFile)
        {

            bool sm = _dashboardrepo.UploadDoc(id, UploadFile);
            if (sm)
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
            bool result = _dashboardrepo.EditCase(sm);

            if (result)
            {
                _notyf.Success("Case Edited Successfully..");
                return RedirectToAction("ViewCase", new { id = sm.RequestID });
            }
            else
            {
                _notyf.Success("Case Not Edited...");
                return View("../AdminSite/Action/ViewCase", sm);
            }

        }

        public IActionResult EditForCloseCase(ViewCloseCaseModel sm)
        {
            bool result = _actionrepo.EditForCloseCase(sm);

            if (result)
            {
                _notyf.Success("Case Edited Successfully..");
                return RedirectToAction("CloseCase", new { sm.RequestID });
            }
            else
            {
                _notyf.Error("Case Not Edited...");
                return RedirectToAction("CloseCase", new { sm.RequestID });
            }

        }
        public IActionResult SendLink(string firstname, string lastname, string email, string phonenumber)
        {
            if (_dashboardrepo.SendLink(firstname, lastname, email, phonenumber))
            {

                _notyf.Success("Email Sended to " + firstname);
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
        public IActionResult ChangeNotes(int RequestID, string? adminnotes, string? physiciannotes)
        {
            if (adminnotes != null || physiciannotes != null)
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
        public IActionResult CancelCase(int RequestID, string Note, string CaseTag)
        {


            bool CancelCase = _dashboardrepo.CancelCase(RequestID, Note, CaseTag);
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

        public IActionResult DeleteallDoc(string path,int RequestID)
        {



            List<int> pathList = path.Split(',').Select(int.Parse).ToList();
            for(var i=0;i<pathList.Count;i++)
            {
              
                bool data = _actionrepo.DeleteDoc(pathList[i]);
           


                if (data)
                {
       
                    _notyf.Success("File deleted successfully.");
                }
                else
                {
                  
                    _notyf.Error("File does not exist.");
                    return RedirectToAction("ViewDocuments", new { id = RequestID });
                }
            }

      

                _notyf.Success("Documet Deleted Successfully");
           

            return RedirectToAction("ViewDocuments", new { id = RequestID });

        }
        public IActionResult SendMailDoc(string path, int RequestID)
        {



           

                bool data = _actionrepo.SendAllMailDoc(path,RequestID);



                if (data)
                {

                    _notyf.Success("mail Sended with Document successfully.");
                }
                else
                {

                    _notyf.Error("mail not sended.");
                    return RedirectToAction("ViewDocuments", new { id = RequestID });
                }
            



            _notyf.Success("Documet Deleted Successfully");


            return RedirectToAction("ViewDocuments", new { id = RequestID });

        }

        public IActionResult DeleteDoc(int RequestWiseFileID, int RequestID,string path)
        {


            
            bool data = _actionrepo.DeleteDoc(RequestWiseFileID);
            if (data)
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
        #region Delete Doc CloseCase
        public IActionResult DeleteDocCloseCase(int RequestWiseFileID, int RequestID)
        {


            bool data = _actionrepo.DeleteDoc(RequestWiseFileID);
            if (data)
            {
                _notyf.Success("Documet Deleted Successfully");
            }
            else
            {
                _notyf.Error("Documet Not Deleted");
            }

            return RedirectToAction("CloseCase", new { RequestID = RequestID });

        }
        #endregion

        #region order_action
        public async Task<IActionResult> Order(int id)
        {

            List<HealthprofessionaltypeCombobox> cs =await _combobox.healthprofessionaltype();
            ViewBag.ProfessionType = cs;
            ViewSendOrderModel data = new ViewSendOrderModel
            {
                RequestID = id
            };
            return View("../AdminSite/Action/SendOrder", data);
        }

        public Task<IActionResult> ProfessionalByType(int HealthprofessionalID)
        {

            var v = _combobox.ProfessionalByType(HealthprofessionalID);
            return Task.FromResult<IActionResult>(Json(v));
        }

        public Task<IActionResult> SelectProfessionalByID (int VendorID)
        {

            var v = _actionrepo.SelectProfessionlByID(VendorID);
            return Task.FromResult<IActionResult>(Json(v));
        }


        public IActionResult SendOrder(ViewSendOrderModel sm)
        {
            if(ModelState.IsValid)
            {
                bool data=_actionrepo.SendOrder(sm);
                if(data)
                {
                    _notyf.Success("Order Created  successfully...");
                    _notyf.Information("Mail is sended to Vendor successfully...");
                     return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    _notyf.Error("Order Not Created...");
                    return View("../AdminSite/Action/SendOrder", sm);
                }

            }
            else
            {
                return View("../AdminSite/Action/SendOrder", sm);
            }
        }



        #endregion



        #region Clear_case
          public IActionResult ClearCase(int RequestID)
        {
             bool sm=_actionrepo.ClearCase(RequestID);
            if(sm)
            {
                _notyf.Success("Case Cleared...");
                _notyf.Warning("You can not show Cleared Case ...");
            }
            else
            {
                _notyf.Error("there is some error in deletion...");
            }
            return RedirectToAction("Index", "Dashboard", new { Status="4,5" });
        }

        #endregion



        #region Close Case


        public IActionResult CloseCase(int RequestID)
        {

            ViewCloseCaseModel vc=_actionrepo.CloseCaseData(RequestID);
            return View("../AdminSite/Action/CloseCase",vc);
        }
        public IActionResult CloseCaseUnpaid(int RequestID)
        {
            bool sm = _actionrepo.CloseCase(RequestID);
            if (sm)
            {
                _notyf.Success("Case Closed...");
                     _notyf.Information("You can see Closed case in unpaid State...");

            }
            else
            {
                _notyf.Error("there is some error in deletion...");
            }
            return RedirectToAction("Index", "Dashboard", new { Status = "3,7,8" });
        }


        #endregion


    }
}

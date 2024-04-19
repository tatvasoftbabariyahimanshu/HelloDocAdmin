using AspNetCoreHero.ToastNotification.Abstractions;
using HelloDocAdmin.Controllers.Authenticate;
using HelloDocAdmin.Entity.Data;
using HelloDocAdmin.Entity.ViewModel.PatientSite;
using HelloDocAdmin.Entity.ViewModels;
using HelloDocAdmin.Entity.ViewModels.AdminSite;
using HelloDocAdmin.Repositories.Interface;
using HelloDocAdmin.Repositories.PatientInterface;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;

namespace HelloDocAdmin.Controllers.AdminSite
{
    [CustomAuthorization("Admin,Physician", "Dashboard")]
    public class ActionController : Controller
    {
        private IPatientRequestRepository _patientrequestrepo;
        private IActionRepository _actionrepo;
        private IDashboardRepository _dashboardrepo;
        private ICombobox _combobox;
        private readonly ILogger<DashboardController> _logger;
        private readonly INotyfService _notyf;
        private readonly EmailConfiguration _email;
        public ActionController(ILogger<DashboardController> logger, IPatientRequestRepository patientrequestrepo, IDashboardRepository dashboardRepository, ICombobox combobox, IActionRepository actionrepo, INotyfService notyf, EmailConfiguration email)
        {
            _logger = logger;
            _combobox = combobox;
            _dashboardrepo = dashboardRepository;
            _actionrepo = actionrepo;
            _notyf = notyf;
            _email = email;
            this._patientrequestrepo = patientrequestrepo;
        }

        #region ViewCase
        public async Task<IActionResult> ViewCase(int id)
        {
            ViewBag.RegionComboBox = await _combobox.RegionComboBox();
            ViewBag.CaseReasonComboBox = await _combobox.CaseReasonComboBox();
            ViewCaseModel sm = _dashboardrepo.GetRequestForViewCase(id);

            return View("../AdminSite/Action/ViewCase", sm);
        }
        #endregion

        #region ViewNotes
        public IActionResult ViewNotes(int id)
        {

            ViewNotesModel sm = _dashboardrepo.getNotesByID(id);
            return View("../AdminSite/Action/ViewNotes", sm);
        }
        #endregion

        #region ViewDocuments
        public IActionResult ViewDocuments(int id)
        {
            if (TempData["Mail"] != null)
            {
                _notyf.Success(TempData["Mail"].ToString());
            }

            ViewDocumentsModel sm = _dashboardrepo.ViewDocument(id);
            return View("../AdminSite/Action/ViewDocuments", sm);
        }
        #endregion

        #region UploadDocuments
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
        #endregion

        #region UploadDocumentsCC
        public IActionResult UploadDocumentsCC(int id, IFormFile? UploadFile)
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

            return RedirectToAction("ConcludeCare", new { requestID = id });
        }
        #endregion

        #region EditCase
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
        #endregion

        #region EditForCloseCase
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
        #endregion

        #region SendLink
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
        #endregion


        #region AssignProvider
        [CustomAuthorization("Admin", "Dashboard")]
        public async Task<IActionResult> AssignProvider(int requestid, int ProviderId, string Notes)
        {
            if (await _dashboardrepo.AssignProvider(requestid, ProviderId, Notes, CV.LoggedUserID()))
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
        [CustomAuthorization("Admin", "Dashboard")]
        public async Task<IActionResult> TransferProvider(int requestid, int ProviderId, string Notes)
        {
            if (await _actionrepo.TransferProvider(requestid, ProviderId, Notes, CV.LoggedUserID()))
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

        #region TransferProvider
        [CustomAuthorization("Physician", "Dashboard")]
        public async Task<IActionResult> TransferToAdmin(int requestid, string Notes)
        {
            if (await _actionrepo.TransferToAdmin(requestid, Notes, CV.LoggedUserID()))
            {
                _notyf.Success("Case is Transfered To Admin...");
            }
            else
            {
                _notyf.Error("Not Transfered...");
            }

            return RedirectToAction("Index", "Dashboard");
        }
        #endregion

        #region ChangeNotes
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
        #endregion

        #region _CancelCase
        [CustomAuthorization("Admin", "Dashboard")]
        public IActionResult CancelCase(int RequestId, string Note, string CaseTag)
        {


            bool CancelCase = _dashboardrepo.CancelCase(RequestId, Note, CaseTag, CV.LoggedUserID());
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
        [CustomAuthorization("Admin", "Dashboard")]
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

        public IActionResult DeleteallDoc(string path, int RequestID)
        {



            List<int> pathList = path.Split(',').Select(int.Parse).ToList();
            for (var i = 0; i < pathList.Count; i++)
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





            bool data = _actionrepo.SendAllMailDoc(path, RequestID);



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

        public IActionResult DeleteDoc(int RequestWiseFileID, int RequestID, string path)
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
        public IActionResult DeleteDocCC(int RequestWiseFileID, int RequestID, string path)
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

            return RedirectToAction("ConcludeCare", new { requestID = RequestID });

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
        [CustomAuthorization("Admin,Physician", "SendOrder")]
        public async Task<IActionResult> Order(int id)
        {

            List<HealthprofessionaltypeCombobox> cs = await _combobox.healthprofessionaltype();
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


        public Task<IActionResult> SelectProfessionalByID(int VendorID)
        {

            var v = _actionrepo.SelectProfessionlByID(VendorID);
            return Task.FromResult<IActionResult>(Json(v));
        }

        [CustomAuthorization("Admin,Physician", "SendOrder")]
        public IActionResult SendOrder(ViewSendOrderModel sm)
        {
            if (ModelState.IsValid)
            {
                bool data = _actionrepo.SendOrder(sm, CV.LoggedUserID());
                if (data)
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
        [CustomAuthorization("Admin", "Dashboard")]
        public IActionResult ClearCase(int RequestID)
        {
            bool sm = _actionrepo.ClearCase(RequestID, CV.LoggedUserID());
            if (sm)
            {
                _notyf.Success("Case Cleared...");
                _notyf.Warning("You can not show Cleared Case ...");
            }
            else
            {
                _notyf.Error("there is some error in deletion...");
            }
            return RedirectToAction("Index", "Dashboard", new { Status = "4,5" });
        }

        #endregion

        #region Close Case

        [CustomAuthorization("Admin", "Dashboard")]
        public IActionResult CloseCase(int RequestID)
        {

            ViewCloseCaseModel vc = _actionrepo.CloseCaseData(RequestID);
            return View("../AdminSite/Action/CloseCase", vc);
        }
        [CustomAuthorization("Admin", "Dashboard")]
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

        #region Accept Case


        [CustomAuthorization("Physician", "Dashboard")]
        public IActionResult AcceptRequest(int requestID)
        {
            bool sm = _actionrepo.AcceptCase(requestID);
            if (sm)
            {
                _notyf.Success("Request Accepted");


            }
            else
            {
                _notyf.Error("there is some error in Accepting...");
            }
            return RedirectToAction("Index", "Dashboard", new { Status = "2" });
        }


        #endregion

        #region Encounter

        #region ACTION-ENCOUNTER VIEW
        public async Task<IActionResult> EncounterView(int RequestID)
        {

            EncounterViewModel model = _actionrepo.GetEncounterDetailsByRequestID(RequestID);

            return View("../AdminSite/Action/Encounter", model);
        }
        #endregion
        public IActionResult EncounterEdit(EncounterViewModel model)
        {



            bool data = _actionrepo.EditEncounterDetails(model, CV.LoggedUserID());
            if (data)
            {


                _notyf.Success("Encounter Changes Saved..");
            }
            else
            {
                _notyf.Error("Encounter Changes Not Saved");
            }

            return RedirectToAction("EncounterView", new { RequestID = model.Requesid });

        }

        #region ACTION-FINALIZE
        public IActionResult Finalize(EncounterViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool data = _actionrepo.EditEncounterDetails(model, CV.LoggedUserID());
                if (data)
                {
                    bool final = _actionrepo.CaseFinalized(model, CV.LoggedUserID());
                    if (final)
                    {
                        _notyf.Success("Case Is Finalized");
                        return RedirectToAction("Index", "Dashboard", new { Status = "6" });
                    }
                    else
                    {
                        _notyf.Success("Case Is not Finalized");
                        return View("../AdminSite/Action/Encounter", model);
                    }

                }
                else
                {
                    _notyf.Error("Case Is not Finalized");
                    return View("../AdminSite/Action/Encounter", model);
                }
            }
            else
            {
                _notyf.Error("Enter Valid data");
                return View("../AdminSite/Action/Encounter", model);
            }


        }
        #endregion

        #endregion

        #region Create New Request

        public IActionResult CreateNewRequest()
        {
            return View("../AdminSite/Action/CreateNewRequest");
        }
        public IActionResult CreateNewRequestPost(ViewPatientRequest data)
        {
            if (ModelState.IsValid)
            {

                bool region = _patientrequestrepo.CkeckRegion(data.State);
                if (region)
                {
                    _notyf.Information("There are No services In this region");
                    ModelState.AddModelError("State", "There are No services In this region");
                    return View("../AdminSite/Action/CreateNewRequest", data);
                }
                bool apr = _actionrepo.CreateNewRequestPost(data, CV.LoggedUserID());
                if (apr)
                {
                    _notyf.Success("Request Created....!");
                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    _notyf.Error("Request Not Created....!");
                    return View("../AdminSite/Action/CreateNewRequest");
                }
            }
            else
            {
                _notyf.Error("Data Invalid....!");
                return View("../AdminSite/Action/CreateNewRequest");
            }

        }

        #endregion

        #region HouseCall
        [CustomAuthorization("Physician", "Dashboard")]
        public IActionResult HouseCall(int requestID)
        {

            bool data = _actionrepo.houseCall(requestID, CV.LoggedUserID());
            if (data)
            {
                _notyf.Success("Request Status Changed To House Call");
            }
            else
            {
                _notyf.Error("Request Status Not Changed To House Call");

            }

            return RedirectToAction("Index", "Dashboard");
        }
        #endregion

        #region Consult
        [CustomAuthorization("Physician", "Dashboard")]
        public IActionResult Consult(int requestID)
        {

            bool data = _actionrepo.Consult(requestID, CV.LoggedUserID());
            if (data)
            {
                _notyf.Success("Request Concluded...");
            }
            else
            {
                _notyf.Error("Request Not Concluded...");

            }

            return RedirectToAction("Index", "Dashboard");
        }
        #endregion

        #region ConcludeCare
        public IActionResult ConcludeCare(int requestID)
        {



            ViewDocumentsModel sm = _dashboardrepo.ViewDocument(requestID);
            return View("../AdminSite/Action/ConcludeCare", sm);


        }
        #endregion

        #region ConcludeCarePost
        public IActionResult ConcludeCarePost(int RequestID, string Notes)
        {
            if (_actionrepo.IsCaseFinialized(RequestID))
            {
                _notyf.Error("Case Not Finalized , please finalized ...");
                return RedirectToAction("ConcludeCare", new { requestID = RequestID });
            }
            else
            {
                bool data = _actionrepo.ConcludeCarePost(RequestID, Notes);
                if (data)
                {
                    _notyf.Success("Case Concluded...");
                }
                else
                {
                    _notyf.Error("Case Not Concluded...");

                }

                return RedirectToAction("Index", "Dashboard");
            }






        }
        #endregion

        #region GeneratePdf
        public IActionResult GeneratePdf(int RequestID)
        {

            EncounterViewModel model = _actionrepo.GetEncounterDetailsByRequestID(RequestID);


            return new ViewAsPdf("../Shared/EncounterPDF", model)
            {
                FileName = "EncounterReport.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageMargins = { Left = 20, Right = 20 }
            };
        }
        #endregion
    }
}

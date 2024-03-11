using AspNetCoreHero.ToastNotification.Abstractions;
using HelloDocAdmin.Entity.ViewModels.PatientSite;
using HelloDocAdmin.Repositories.PatientInterface;
using HelloDocAdmin.Controllers.Authenticate;
using Microsoft.AspNetCore.Mvc;
using HelloDocAdmin.Controllers.AdminSite;
using DocumentFormat.OpenXml.Office2010.Excel;
using HelloDocAdmin.Entity.ViewModels.AdminSite;
using HelloDocAdmin.Entity.Models;
using DocumentFormat.OpenXml.Wordprocessing;

namespace HelloDocAdmin.Controllers.PatientSite
{
    [CustomAuthorization("Patient")]
    public class PatientDashboardController : Controller
    {
        private IPatientDashboardRepository _patientDashrepo;
        private readonly INotyfService _notyf;
        public string DATASUCCESS=null;
        public PatientDashboardController(IPatientDashboardRepository patientDashrepo, INotyfService notyf)
        {

            this._patientDashrepo = patientDashrepo;
            _notyf = notyf;

        }
        #region Index
        public IActionResult Index()
        {

            ViewDashboardDataModel model = _patientDashrepo.DashboardData(Authenticate.CV.LoggedUserID());
            return View("../PatientSite/PatientDashboard/PatientDashboard",model);
        }
        #endregion


        #region  Documents
        public IActionResult Documents(int RequestID)
        {
            ViewDocumentsModel sm = _patientDashrepo.ViewDocument(RequestID);
            
            return View("../PatientSite/PatientDashboard/Documentsinfo", sm);
        }
        public IActionResult UploadDocuments(int RequestID, IFormFile? UploadFile)
        {

            bool sm = _patientDashrepo.UploadDoc(RequestID, UploadFile);
            if (sm)
            {
                _notyf.Success("Document Uploaded Successfully");
            }
            else
            {
                _notyf.Error("Document not Uploaded");
            }

            return RedirectToAction("Documents", new { RequestID });
        }
        public IActionResult DeleteallDoc(string path, int RequestID)
        {



            List<int> pathList = path.Split(',').Select(int.Parse).ToList();
            for (var i = 0; i < pathList.Count; i++)
            {

                bool data = _patientDashrepo.DeleteDoc(pathList[i]);



                if (data)
                {

                    _notyf.Success("File deleted successfully.");
                }
                else
                {

                    _notyf.Error("File does not exist.");
                    return RedirectToAction("Documents", new {  RequestID });
                }
            }



            _notyf.Success("Documet Deleted Successfully");
        


            return RedirectToAction("Documents", new {  RequestID });

        }
        public IActionResult DeleteDoc(int RequestWiseFileID, int RequestID)
        {



            bool data = _patientDashrepo.DeleteDoc(RequestWiseFileID);
            if (data)
            {


                _notyf.Success("Documet Deleted Successfully");
            }
            else
            {
                _notyf.Error("Documet Not Deleted");
            }

            return RedirectToAction("Documents", new {  RequestID });

        }
        public IActionResult SendMailDoc(string path, int RequestID)
        {





            bool data = _patientDashrepo.SendAllMailDoc(path, RequestID);



            if (data)
            {

                _notyf.Success("mail Sended with Document successfully.");
            }
            else
            {

                _notyf.Error("mail not sended.");
                return RedirectToAction("Documents", new {  RequestID });
            }




            _notyf.Success("Documet Deleted Successfully");


            return RedirectToAction("Documents", new { RequestID });

        }
        #endregion


        #region EditProfile
        public async Task<IActionResult> EditProfile(ViewDashboardDataModel model)
        {
            
            
            bool sm = _patientDashrepo.ProfileEdit(model, Authenticate.CV.LoggedUserID());
            if (sm)
            {
                _notyf.Success("Profile Edited  Successfully");
            }
            else
            {
                _notyf.Error("Profile Not Edited");
            }

            return RedirectToAction("Index");
        }
        #endregion
    }
}

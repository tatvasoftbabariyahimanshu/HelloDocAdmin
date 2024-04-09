using AspNetCoreHero.ToastNotification.Abstractions;
using HelloDocAdmin.Controllers.Authenticate;
using HelloDocAdmin.Entity.ViewModel.PatientSite;
using HelloDocAdmin.Repositories.PatientInterface;
using Microsoft.AspNetCore.Mvc;
namespace HelloDocAdmin.Controllers.PatientSite
{
    public class RequestForMEOrElseController : Controller
    {
        private IPatientRequestRepository _patientrequestrepo;
        private IPatientDashboardRepository _patientDashrepo;
        private readonly INotyfService _notyf;

        public RequestForMEOrElseController(IPatientDashboardRepository patientDashrepo, INotyfService notyf, IPatientRequestRepository patientrequestrepo)
        {

            this._patientDashrepo = patientDashrepo;
            _notyf = notyf;
            _patientrequestrepo = patientrequestrepo;
        }
        public IActionResult RequestForME()
        {
            ViewPatientRequest model = _patientDashrepo.GetDataForME(CV.LoggedUserID());
            return View("../PatientSite/PatientDashboard/RequestForME", model);
        }
        public IActionResult RequestForSomeOneElse()
        {
            ViewFamilyFriendRequest model = _patientDashrepo.GetDataForSomeOneElse(CV.LoggedUserID());
            return View("../PatientSite/PatientDashboard/RequestForsomeone_else", model);
        }

        #region CreateForME
        public IActionResult CreateForME(ViewPatientRequest viewdata)
        {
            if (ModelState.IsValid)
            {
                var region = _patientrequestrepo.CkeckRegion(viewdata.State);
                if (region == null)
                {
                    _notyf.Information("Currently we are not serving in this region");
                    ModelState.AddModelError("State", "Currently we are not serving in this region");
                    return View("../PatientSite/PatientDashboard/RequestForME", viewdata);
                }
                else
                {
                    if (_patientrequestrepo.UserIsBlocked(viewdata.Email))
                    {
                        _notyf.Information("Email Is Blocked By Admin");
                        ModelState.AddModelError("Email", "Email Is Blocked By Admin");
                        return View("../PatientSite/PatientDashboard/RequestForME", viewdata);
                    }
                    bool val = _patientDashrepo.CreateNewRequestForME(viewdata);
                    if (val)
                    {
                        _notyf.Success("Your Request is Submited");
                        return RedirectToAction("Index", "PatientDashboard");
                    }
                    else
                    {
                        _notyf.Error("Request Not Submited");
                        return View("../PatientSite/PatientDashboard/RequestForME", viewdata);
                    }
                }



            }
            else
            {
                _notyf.Error("Enter Valid Data");
                return View("../PatientSite/PatientDashboard/RequestForME", viewdata);
            }
        }
        #endregion
        #region CreateForSomeOneElse
        public IActionResult CreateForSomeOneElse(ViewFamilyFriendRequest viewdata)
        {
            if (ModelState.IsValid)
            {
                var region = _patientrequestrepo.CkeckRegion(viewdata.State);
                if (region == null)
                {
                    _notyf.Information("Currently we are not serving in this region");
                    ModelState.AddModelError("State", "Currently we are not serving in this region");
                    return View("../PatientSite/PatientDashboard/RequestForsomeone_else", viewdata);
                }
                else
                {
                    if (_patientrequestrepo.UserIsBlocked(viewdata.Email))
                    {
                        _notyf.Information("Email Is Blocked By Admin");
                        ModelState.AddModelError("Email", "Email Is Blocked By Admin");
                        return View("../PatientSite/PatientDashboard/RequestForsomeone_else", viewdata);
                    }
                    bool val = _patientDashrepo.CreateNewRequestForSomeOneElse(viewdata);
                    if (val)
                    {
                        _notyf.Success("Your Request for Some one is Submited");
                        return RedirectToAction("Index", "PatientDashboard");
                    }
                    else
                    {
                        _notyf.Error("Request Not Submited");
                        return View("../PatientSite/PatientDashboard/RequestForsomeone_else", viewdata);
                    }
                }



            }
            else
            {
                _notyf.Error("Enter Valid Data");
                return View("../PatientSite/PatientDashboard/RequestForsomeone_else", viewdata);
            }
        }
        #endregion
    }
}

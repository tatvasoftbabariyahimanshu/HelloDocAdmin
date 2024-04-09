using AspNetCoreHero.ToastNotification.Abstractions;
using HelloDocAdmin.Entity.ViewModel.PatientSite;
using HelloDocAdmin.Repositories.PatientInterface;
using Microsoft.AspNetCore.Mvc;

namespace HelloDocAdmin.Controllers.PatientSite
{
    public class PatientRequestController : Controller
    {

        private IPatientRequestRepository _patientrequestrepo;
        private readonly INotyfService _notyf;

        public PatientRequestController(IPatientRequestRepository patientrequestrepo, INotyfService notyf)
        {

            this._patientrequestrepo = patientrequestrepo;
            _notyf = notyf;

        }

        public IActionResult Index()
        {
            return View("../PatientSite/Request/PatientRequestForm");
        }

        #region CheckuserExist
        public IActionResult CheckUserExist(string? Email)
        {
            return Json(new { isvalid = _patientrequestrepo.CheckUserExist(Email) });
        }
        #endregion




        #region Create
        public async Task<IActionResult> Create(ViewPatientRequest viewdata)
        {
            if (ModelState.IsValid)
            {
                var region = _patientrequestrepo.CkeckRegion(viewdata.State);
                if (region == null)
                {
                    _notyf.Information("Currently we are not serving in this region");
                    ModelState.AddModelError("State", "Currently we are not serving in this region");
                    return View("../PatientSite/Request/PatientRequestForm", viewdata);
                }
                else
                {
                    if (_patientrequestrepo.UserIsBlocked(viewdata.Email))
                    {
                        _notyf.Information("Email Is Blocked By Admin");
                        ModelState.AddModelError("Email", "Email Is Blocked By Admin");
                        return View("../PatientSite/Request/PatientRequestForm", viewdata);
                    }

                    bool val = _patientrequestrepo.CreatePatientRequest(viewdata);
                    if (val)
                    {
                        _notyf.Success("Patient Request is Submited");
                        return RedirectToAction("Index", "Request");
                    }
                    else
                    {
                        _notyf.Error("Request Not Submited");
                        return View("../PatientSite/Request/PatientRequestForm", viewdata);
                    }
                }



            }
            else
            {
                _notyf.Error("Enter Valid Data");
                return View("../PatientSite/Request/PatientRequestForm", viewdata);
            }
        }
        #endregion
    }

}

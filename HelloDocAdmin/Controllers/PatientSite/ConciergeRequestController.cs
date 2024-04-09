using AspNetCoreHero.ToastNotification.Abstractions;
using HelloDocAdmin.Entity.ViewModel.PatientSite;
using HelloDocAdmin.Repositories.PatientInterface;
using Microsoft.AspNetCore.Mvc;

namespace HelloDocAdmin.Controllers.PatientSite
{
    public class ConciergeRequestController : Controller
    {
        private IPatientRequestRepository _patientrequestrepo;
        private readonly INotyfService _notyf;

        public ConciergeRequestController(IPatientRequestRepository patientrequestrepo, INotyfService notyf)
        {

            this._patientrequestrepo = patientrequestrepo;
            _notyf = notyf;

        }
        public IActionResult Index()
        {
            return View("../PatientSite/Request/ConcierageRequestForm");
        }
        #region Create
        public async Task<IActionResult> Create(ViewConciergeRequest viewdata)
        {
            if (ModelState.IsValid)
            {
                var region = _patientrequestrepo.CkeckRegion(viewdata.CON_State);
                if (region == null)
                {
                    _notyf.Information("Currently we are not serving in this region");
                    ModelState.AddModelError("State", "Currently we are not serving in this region");
                    return View("../PatientSite/Request/ConcierageRequestForm", viewdata);
                }
                else
                {
                    if (_patientrequestrepo.UserIsBlocked(viewdata.Email))
                    {
                        _notyf.Information("Email Is Blocked By Admin");
                        ModelState.AddModelError("Email", "Email Is Blocked By Admin");
                        return View("../PatientSite/Request/ConcierageRequestForm", viewdata);
                    }
                    bool val = _patientrequestrepo.CreateConcierge(viewdata);
                    if (val)
                    {
                        _notyf.Success("Concierage Request is Submited");
                        return RedirectToAction("Index", "Request");
                    }
                    else
                    {
                        _notyf.Error("Request Not Submited");
                        return View("../PatientSite/Request/ConcierageRequestForm", viewdata);
                    }
                }



            }
            else
            {
                _notyf.Error("Enter Valid Data");
                return View("../PatientSite/Request/ConcierageRequestForm", viewdata);
            }
        }
        #endregion
    }
}

using AspNetCoreHero.ToastNotification.Abstractions;
using HelloDocAdmin.Entity.ViewModel.PatientSite;
using HelloDocAdmin.Repositories.PatientInterface;
using Microsoft.AspNetCore.Mvc;

namespace HelloDocAdmin.Controllers.PatientSite
{
    public class BusinessPartnerRequestController : Controller
    {
        private IPatientRequestRepository _patientrequestrepo;
        private readonly INotyfService _notyf;

        public BusinessPartnerRequestController(IPatientRequestRepository patientrequestrepo, INotyfService notyf)
        {

            this._patientrequestrepo = patientrequestrepo;
            _notyf = notyf;

        }
        public IActionResult Index()
        {
            return View("../PatientSite/Request/BusinessPartnerRequestForm");
        }
        #region Create
        public async Task<IActionResult> Create(ViewBusinessPartnerRequest viewdata)
        {
            if (ModelState.IsValid)
            {
                bool region = _patientrequestrepo.CkeckRegion(viewdata.State);
                if (region)
                {
                    _notyf.Information("Currently we are not serving in this region");
                    ModelState.AddModelError("State", "Currently we are not serving in this region");
                    return View("../PatientSite/Request/BusinessPartnerRequestForm", viewdata);
                }
                else
                {
                    if (_patientrequestrepo.UserIsBlocked(viewdata.Email))
                    {
                        _notyf.Information("Email Is Blocked By Admin");
                        ModelState.AddModelError("Email", "Email Is Blocked By Admin");
                        return View("../PatientSite/Request/BusinessPartnerRequestForm", viewdata);
                    }
                    bool val = _patientrequestrepo.BusinessPartnerRequest(viewdata);
                    if (val)
                    {

                        _notyf.Success("Business Partner Request is Submited");
                        return RedirectToAction("Index", "Request");
                    }
                    else
                    {
                        _notyf.Error("Request Not Submited");
                        return View("../PatientSite/Request/BusinessPartnerRequestForm", viewdata);
                    }
                }



            }
            else
            {
                _notyf.Error("Enter Valid Data");
                return View("../PatientSite/Request/BusinessPartnerRequestForm", viewdata);
            }
        }
        #endregion
    }
}

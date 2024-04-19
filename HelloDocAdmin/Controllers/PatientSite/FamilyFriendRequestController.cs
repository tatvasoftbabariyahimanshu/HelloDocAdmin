using AspNetCoreHero.ToastNotification.Abstractions;
using HelloDocAdmin.Entity.ViewModel.PatientSite;
using HelloDocAdmin.Repositories.PatientInterface;
using Microsoft.AspNetCore.Mvc;

namespace HelloDocAdmin.Controllers.PatientSite
{
    public class FamilyFriendRequestController : Controller
    {
        private IPatientRequestRepository _patientrequestrepo;
        private readonly INotyfService _notyf;

        public FamilyFriendRequestController(IPatientRequestRepository patientrequestrepo, INotyfService notyf)
        {

            this._patientrequestrepo = patientrequestrepo;
            _notyf = notyf;

        }
        public IActionResult Index()
        {
            return View("../PatientSite/Request/FamilyFriendRequestForm");
        }
        #region Create
        public async Task<IActionResult> Create(ViewFamilyFriendRequest viewdata)
        {
            if (ModelState.IsValid)
            {
                bool region = _patientrequestrepo.CkeckRegion(viewdata.State);
                if (region)
                {
                    _notyf.Information("Currently we are not serving in this region");
                    ModelState.AddModelError("State", "Currently we are not serving in this region");
                    return View("../PatientSite/Request/FamilyFriendRequestForm", viewdata);
                }
                else
                {
                    if (_patientrequestrepo.UserIsBlocked(viewdata.Email))
                    {
                        _notyf.Information("Email Is Blocked By Admin");
                        ModelState.AddModelError("Email", "Email Is Blocked By Admin");
                        return View("../PatientSite/Request/FamilyFriendRequestForm", viewdata);
                    }
                    bool val = _patientrequestrepo.CreateFamilyFriend(viewdata);
                    if (val)
                    {
                        _notyf.Success("Family Friend Request is Submited");
                        return RedirectToAction("Index", "Request");
                    }
                    else
                    {
                        _notyf.Error("Request Not Submited");
                        return View("../PatientSite/Request/FamilyFriendRequestForm", viewdata);
                    }
                }



            }
            else
            {
                _notyf.Error("Enter Valid Data");
                return View("../PatientSite/Request/FamilyFriendRequestForm", viewdata);
            }
        }
        #endregion
    }
}

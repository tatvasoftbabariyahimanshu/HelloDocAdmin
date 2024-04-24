using HMS.Entity.Models;
using HMS.Entity.ViewModels;
using HMS.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;

namespace HMS.Controllers
{
    public class PatientController : Controller
    {
        private IPatientRepository _patientDashrepo;

        public string DATASUCCESS = null;

        public PatientController(IPatientRepository patientDashrepo)
        {

            this._patientDashrepo = patientDashrepo;


        }
        public IActionResult Index()
        {
            ViewBag.RegionComboBox = _patientDashrepo.GetDoctor();
            return View("../Patient/Index");
        }
        public IActionResult List(string PatientName, int pagesize = 10, int currentpage = 1)
        {
            PatientData pd = _patientDashrepo.DashboardData(PatientName, pagesize, currentpage);
            return PartialView("../Patient/_List", pd);
        }
        public IActionResult _AddEditPatient()
        {
            return PartialView("../Patient/_AddPatient");
        }
        public IActionResult _EditPatient(int PatientID)
        {
            ViewBag.RegionComboBox = _patientDashrepo.GetDoctor();

            return Json(_patientDashrepo.getData(PatientID));
        }
        public IActionResult Getspecialist()
        {
            var v = _patientDashrepo.GetDoctor();
            return Json(v);
        }
        public IActionResult Save(Patient model)
        {
            if (_patientDashrepo.Save(model))
            {
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        public IActionResult DeletePatient(int PatientID)
        {
            if (_patientDashrepo.Delete(PatientID))
            {
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
    }
}

using HelloDocAdmin.Controllers.Authenticate;
using HelloDocAdmin.Entity.ViewModels;
using HelloDocAdmin.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;


namespace HelloDocAdmin.Controllers.AdminSite
{
    [CustomAuthorization("Admin,Physician", "Dashboard")]
    public class DashboardController : Controller
    {
        private IDashboardRepository _dashboardrepo;
        private ICombobox _combobox;
        private readonly ILogger<DashboardController> _logger;
        public DashboardController(ILogger<DashboardController> logger, IDashboardRepository dashboardRepository, ICombobox combobox)
        {
            _logger = logger;
            _dashboardrepo = dashboardRepository;
            _combobox = combobox;
        }


        #region Index
        public async Task<IActionResult> Index()
        {


            ViewBag.RegionComboBox = await _combobox.RegionComboBox();
            ViewBag.CaseReasonComboBox = await _combobox.CaseReasonComboBox();
            DashboardCardsModel model = new DashboardCardsModel();
            model.PandingRequests = _dashboardrepo.GetRequestNumberByStatus("2", CV.LoggedUserID());
            model.NewRequests = _dashboardrepo.GetRequestNumberByStatus("1", CV.LoggedUserID());
            model.ActiveRequests = _dashboardrepo.GetRequestNumberByStatus("4,5", CV.LoggedUserID());
            model.ConcludeRequests = _dashboardrepo.GetRequestNumberByStatus("6", CV.LoggedUserID());
            model.ToCloseRequests = _dashboardrepo.GetRequestNumberByStatus("3,7,8", CV.LoggedUserID());
            model.UnpaidRequests = _dashboardrepo.GetRequestNumberByStatus("9", CV.LoggedUserID());

            return View("../AdminSite/Dashboard/Index", model);
        }
        #endregion

        #region providerbyregion
        public IActionResult ProviderbyRegion(int? Regionid)
        {
            var v = _combobox.ProviderbyRegion(Regionid);
            return Json(v);
        }
        #endregion

        #region _SearchResult
        public async Task<IActionResult> _SearchResult(string? Status, int currentpage = 1, int region = 0, int requesttype = 0, string search = "", int pagesize = 10)
        {
            ViewBag.RegionComboBox = await _combobox.RegionComboBox();

            if (Status == null)
            {
                Status = CV.CurrentStatus();
            }


            Response.Cookies.Delete("Status");
            Response.Cookies.Append("Status", Status);


            List<DashboardRequestModel> requests = _dashboardrepo.GetRequests(Status);
            Dashboarddatamodel dm = new Dashboarddatamodel();


            if (CV.LoggedUserRole() == "Admin")
            {

                dm = await _dashboardrepo.GetRequestsbyfilter(Status, search, region, requesttype, currentpage, pagesize);
            }
            else
            {
                dm = await _dashboardrepo.GetRequestsbyfilterForPhy(Status, CV.LoggedUserID(), search, region, requesttype, currentpage, pagesize);
            }


            TempData["CurrentStatusinlist"] = Status;
            TempData["CurrentStatus"] = GetStatusName(Status);


            switch (Status)
            {
                case "1":
                    Response.Cookies.Append("StatusText", "New");
                    return PartialView("../AdminSite/Dashboard/_newList", dm);

                case "2":
                    Response.Cookies.Append("StatusText", "Pending");
                    return PartialView("../AdminSite/Dashboard/_pandingList", dm);

                case "4,5":
                    Response.Cookies.Append("StatusText", "Active");
                    return PartialView("../AdminSite/Dashboard/_activeList", dm);

                case "6":
                    Response.Cookies.Append("StatusText", "Conclude");
                    return PartialView("../AdminSite/Dashboard/_concludeList", dm);

                case "3,7,8":
                    Response.Cookies.Append("StatusText", "To Close");
                    return PartialView("../AdminSite/Dashboard/_toCloseList", dm);

                case "9":
                    Response.Cookies.Append("StatusText", "Unpaid");
                    return PartialView("../AdminSite/Dashboard/_toUnpaidList", dm);

                default:
                    return PartialView("../AdminSite/Dashboard/nodata", dm);
            }
        }
        #endregion

        #region GetStatusName
        private string GetStatusName(string status)
        {
            return status switch
            {
                "1" => "New",
                "2" => "Pending",
                "4,5" => "Active",
                "6" => "Conclude",
                "3,7,8" => "To Close",
                "9" => "Unpaid",
                _ => "Unknown",
            };
        }
        #endregion

    }
}

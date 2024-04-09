using HelloDocAdmin.Entity.ViewModels;
using HelloDocAdmin.Entity.ViewModels.AdminSite;
using Microsoft.AspNetCore.Http;

namespace HelloDocAdmin.Repositories.Interface
{

    public interface IDashboardRepository
    {
        List<DashboardRequestModel> GetRequests(string Status);
        public Task<bool> AssignProvider(int RequestId, int ProviderId, string notes, string id);

        ViewCaseModel GetRequestForViewCase(int id);
        int GetRequestNumberByStatus(string Status, string id);
        public bool EditCase(ViewCaseModel model);
        public ViewNotesModel getNotesByID(int id);
        public bool EditViewNotes(string? adminnotes, string? physiciannotes, int RequestID);
        public ViewDocumentsModel ViewDocument(int id);
        public bool UploadDoc(int Requestid, IFormFile? UploadFile);
        public bool SendLink(string firstname, string lastname, string email, string phonenumber);
        public bool CancelCase(int RequestID, string Note, string CaseTag, string id);
        public bool BlockCase(int RequestID, string Note);
        public Task<Dashboarddatamodel> GetRequestsbyfilter(string Status, string search = "", int region = 0, int requesttype = 0, int currentpage = 1, int pagezise = 5);
        public Task<Dashboarddatamodel> GetRequestsbyfilterForPhy(string Status, string PhyUserID, string search = "", int region = 0, int requesttype = 0, int currentpage = 1, int pagezise = 5);

    }
}

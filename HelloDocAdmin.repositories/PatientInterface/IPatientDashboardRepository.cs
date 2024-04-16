using HelloDocAdmin.Entity.ViewModel.PatientSite;
using HelloDocAdmin.Entity.ViewModels.AdminSite;
using HelloDocAdmin.Entity.ViewModels.PatientSite;
using Microsoft.AspNetCore.Http;

namespace HelloDocAdmin.Repositories.PatientInterface
{
    public interface IPatientDashboardRepository
    {
        public ViewDashboardDataModel DashboardData(string UserID, int pagesize = 10, int currentpage = 1);
        public ViewDocumentsModel ViewDocument(int RequestID);
        public bool UploadDoc(int Requestid, IFormFile? UploadFile);
        public bool DeleteDoc(int RequestWiseFileID);
        public bool SendAllMailDoc(string path, int RequestID);
        public ViewPatientRequest GetDataForME(string id);
        public ViewFamilyFriendRequest GetDataForSomeOneElse(string id);

        public bool CreateNewRequestForME(ViewPatientRequest model);
        public bool CreateNewRequestForSomeOneElse(ViewFamilyFriendRequest model);
        public bool ProfileEdit(ViewDashboardDataModel u, string id);
    }
}

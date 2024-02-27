using HelloDocAdmin.Entity.Models;
using HelloDocAdmin.Entity.ViewModels;
using HelloDocAdmin.Entity.ViewModels.AdminSite;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocAdmin.Repositories.Interface
{

    public interface IDashboardRepository
    {
        List<DashboardRequestModel> GetRequests(string Status);
        public  Task<bool> AssignProvider(int RequestId, int ProviderId, string notes);

        ViewCaseModel GetRequestForViewCase(int id);
        int GetRequestNumberByStatus(string Status);
        public bool EditCase(ViewCaseModel model);
        public ViewNotesModel getNotesByID(int id);
        public bool EditViewNotes(string? adminnotes, string? physiciannotes, int RequestID);
        public ViewDocumentsModel ViewDocument(int id);
        public bool UploadDoc(int Requestid, IFormFile? UploadFile);
        public bool SendLink(string firstname, string lastname, string email, string phonenumber);
        public bool CancelCase(int RequestID, string Note, string CaseTag);
            public bool BlockCase(int RequestID, string Note);

    }
}

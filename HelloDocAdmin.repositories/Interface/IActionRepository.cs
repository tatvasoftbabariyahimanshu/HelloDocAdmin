using HelloDocAdmin.Entity.Models;
using HelloDocAdmin.Entity.ViewModel.PatientSite;
using HelloDocAdmin.Entity.ViewModels.AdminSite;

namespace HelloDocAdmin.Repositories.Interface
{
    public interface IActionRepository
    {
        public string GetFileName(int RequestWiseFileID);
        public bool DeleteDoc(int RequestWiseFileID);
        public Task<bool> TransferProvider(int RequestId, int ProviderId, string notes, string id);
        public Healthprofessional SelectProfessionlByID(int VendorID);
        public bool SendOrder(ViewSendOrderModel data, string id);
        public bool CloseCase(int RequestID);
        public bool AcceptCase(int RequestID);
        public bool ClearCase(int RequestID, string id);
        public bool houseCall(int RequestID, string id);
        public bool Consult(int RequestID, string id);
        public ViewCloseCaseModel CloseCaseData(int RequestID);
        public bool EditForCloseCase(ViewCloseCaseModel model);
        public bool SendAllMailDoc(string path, int RequestID);


        public EncounterViewModel GetEncounterDetailsByRequestID(int RequestID);
        public bool EditEncounterDetails(EncounterViewModel Data, string id);
        public bool CaseFinalized(EncounterViewModel model, string id);

        public Task<bool> TransferToAdmin(int RequestId, string notes, string id);
        public bool CreateNewRequestPost(ViewPatientRequest model, string id);
    }
}

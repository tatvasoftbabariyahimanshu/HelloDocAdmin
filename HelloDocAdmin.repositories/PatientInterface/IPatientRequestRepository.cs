using HelloDocAdmin.Entity.ViewModel.PatientSite;

namespace HelloDocAdmin.Repositories.PatientInterface
{
    public interface IPatientRequestRepository
    {
        public bool CheckUserExist(string? Email);
        public bool CkeckRegion(string? State);
        public bool CreatePatientRequest(ViewPatientRequest viewdata);
        public bool CreateFamilyFriend(ViewFamilyFriendRequest viewdata);
        public bool CreateConcierge(ViewConciergeRequest viewdata);
        public bool BusinessPartnerRequest(ViewBusinessPartnerRequest viewdata);
        public bool UserIsBlocked(string? Email);
    }
}

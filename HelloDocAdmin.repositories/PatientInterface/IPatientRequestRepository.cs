using HelloDocAdmin.Entity.ViewModel.PatientSite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    }
}

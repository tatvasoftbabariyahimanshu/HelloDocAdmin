using HelloDocAdmin.Entity.ViewModels.AdminSite;

namespace HelloDocAdmin.Repositories.Interface
{
    public interface ISearchRecords
    {

        public Task<RequestRecords> GetRequestsbyfilterForRecords(short status, string patientname, int requesttype, DateTime startdate, DateTime enddate, string physicianname, string email, string phonenumber, int currentpage = 1, int pagesize = 5);

        public Task<PatientHistory> Patienthistorybyfilter(string firstname, string lastname, string email, string phonenumber, int currentpage = 1, int pagesize = 5);


        public Task<PatientRecordsView> PatientRecordsViewBy(int? UserID, int currentpage = 1, int pagesize = 5);
        public Task<EmailRecords> EmailLogs(int accounttype, string email, string ReciverName, DateTime CreatedDate, DateTime SendDate, int pagesize = 5, int currentpage = 1);


        public Task<BlockRequest> BlockHistory(string name, string email, string phonenumber, DateTime CreatedDate, int pagesize = 5, int currentpage = 1);

        public Task<bool> UnBlock(int RequestID, string id);
    }
}

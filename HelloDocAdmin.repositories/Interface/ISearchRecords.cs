using HelloDocAdmin.Entity.ViewModels.AdminSite;

namespace HelloDocAdmin.Repositories.Interface
{
    public interface ISearchRecords
    {

        public Task<RequestRecords> GetRequestsbyfilterForRecords(short status, string patientname, int requesttype, DateTime startdate, DateTime enddate, string physicianname, string email, string phonenumber, int currentpage = 1, int pagesize = 5);

        public Task<PatientHistory> Patienthistorybyfilter(string firstname, string lastname, string email, string phonenumber, int currentpage = 1, int pagesize = 5);

    }
}

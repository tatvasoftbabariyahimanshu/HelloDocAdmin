namespace HelloDocAdmin.Entity.ViewModels.AdminSite
{
    public class RequestRecords
    {

        public List<SearchRecordView> requestList { get; set; }

        public int? TotalPage { get; set; }

        public int? CurrentPage { get; set; }

        public int? pageSize { get; set; }
    }
}

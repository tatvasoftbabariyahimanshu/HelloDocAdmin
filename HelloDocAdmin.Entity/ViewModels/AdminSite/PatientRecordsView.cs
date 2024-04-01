namespace HelloDocAdmin.Entity.ViewModels.AdminSite
{
    public class PatientRecordsView
    {
        public List<PatientRecords> List { get; set; }
        public int TotalPage { get; set; }
        public int UserID { get; set; }
        public int CurrentPage { get; set; }

        public int pageSize { get; set; }
    }
}

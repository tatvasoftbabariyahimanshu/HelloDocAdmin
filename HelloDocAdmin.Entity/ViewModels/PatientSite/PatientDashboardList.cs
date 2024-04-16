namespace HelloDocAdmin.Entity.ViewModels.PatientSite
{
    public class PatientDashboardList
    {
        public List<ViewDashboardDataModel> List { get; set; }
        public int TotalPage { get; set; }
        public int UserID { get; set; }
        public int CurrentPage { get; set; }

        public int pageSize { get; set; }
    }
}

namespace HelloDocAdmin.Entity.ViewModels.AdminSite
{
    public class EmailRecords
    {
        public List<Emaillogdata> List { get; set; }

        public int TotalPage { get; set; }

        public int CurrentPage { get; set; }

        public int pageSize { get; set; }
    }
}

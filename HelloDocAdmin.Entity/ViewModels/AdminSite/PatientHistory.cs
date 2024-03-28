using HelloDocAdmin.Entity.Models;

namespace HelloDocAdmin.Entity.ViewModels.AdminSite
{
    public class PatientHistory
    {
        public List<User> userList { get; set; }

        public int TotalPage { get; set; }

        public int CurrentPage { get; set; }

        public int pageSize { get; set; }
    }
}

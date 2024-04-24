using HMS.Entity.Models;

namespace HMS.Entity.ViewModels
{
    public class PatientData
    {
        public List<Patient> PatientList { get; set; }

        public int TotalPage { get; set; }

        public int CurrentPage { get; set; }

        public int pageSize { get; set; }
    }
}

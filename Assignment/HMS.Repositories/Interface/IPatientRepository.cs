using HMS.Entity.Models;
using HMS.Entity.ViewModels;

namespace HMS.Repositories.Interface
{
    public interface IPatientRepository
    {
        public PatientData DashboardData(string? PatientName, int pagesize = 10, int currentpage = 1);
        public List<Doctor> GetDoctor();
        public bool Save(Patient model);
        public bool Delete(int PatientID);
        public Patient getData(int PatientID);
    }
}

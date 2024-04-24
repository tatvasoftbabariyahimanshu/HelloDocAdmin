using HMS.Entity.Data;
using HMS.Entity.Models;
using HMS.Entity.ViewModels;
using HMS.Repositories.Interface;
using System.Collections;

namespace HMS.Repositories
{
    public class PatientRepository : IPatientRepository
    {

        private readonly ApplicationDbContext _context;

        public PatientRepository(ApplicationDbContext context)
        {
            _context = context;

        }
        public PatientData DashboardData(string PatientName, int pagesize = 10, int currentpage = 1)
        {
            BitArray bt = new BitArray(1);
            bt.Set(0, false);
            PatientData model = new PatientData();

            IQueryable<Patient> data = (from Req in _context.Patients
                                        where Req.IsDeleted == bt

                                        select new Patient()
                                        {
                                            Id = Req.Id,
                                            FirstName = Req.FirstName,
                                            LastName = Req.LastName,
                                            Age = Req.Age,
                                            Disease = Req.Disease,
                                            DoctorId = Req.DoctorId,
                                            Email = Req.Email,
                                            PhoneNo = Req.PhoneNo,
                                            Gender = Req.Gender,
                                            Specialist = Req.Specialist,


                                        }

                                               );

            model.TotalPage = (int)Math.Ceiling((double)data.Count() / pagesize);
            data = data.Skip((currentpage - 1) * pagesize).Take(pagesize);

            if (!String.IsNullOrEmpty(PatientName))

            {
                data = data.Where(r => r.FirstName.ToLower().Contains(PatientName.ToLower()));
            }

            List<Patient> Request = data.ToList();
            model.pageSize = pagesize;
            model.CurrentPage = currentpage;
            model.PatientList = Request;

            return model;
        }


        public List<Doctor> GetDoctor()
        {
            List<Doctor> data = _context.Doctors.ToList();
            return data;
        }
        public bool Save(Patient model)
        {
            try
            {
                BitArray bt = new BitArray(1);
                bt.Set(0, false);
                if (model.Id == 0)
                {

                    Patient data = new Patient()
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        DoctorId = model.DoctorId,
                        Email = model.Email,
                        PhoneNo = model.PhoneNo,
                        Gender = model.Gender,
                        Specialist = _context.Doctors.FirstOrDefault(e => e.DoctorId == model.DoctorId).Specialist,

                        Age = model.Age,
                        Disease = model.Disease,
                        IsDeleted = bt,

                    };
                    _context.Patients.Add(data);
                    _context.SaveChanges();

                    return true;
                }
                else
                {
                    var data = _context.Patients.FirstOrDefault(e => e.Id == model.Id);
                    data.FirstName = model.FirstName;
                    data.LastName = model.LastName;
                    data.DoctorId = model.DoctorId;
                    data.Email = model.Email;
                    data.PhoneNo = model.PhoneNo;
                    data.Gender = model.Gender;
                    data.Specialist = _context.Doctors.FirstOrDefault(e => e.DoctorId == model.DoctorId).Specialist;
                    data.Age = model.Age;
                    data.Disease = model.Disease;
                    data.IsDeleted = bt;
                    _context.Patients.Update(data);
                    _context.SaveChanges(true);
                    return true;
                }


            }
            catch (Exception e)
            {
                return false;

            }



        }
        public bool Delete(int PatientID)
        {
            BitArray bt = new BitArray(1);
            bt.Set(0, true);
            try
            {
                Patient data = _context.Patients.FirstOrDefault(e => e.Id == PatientID);

                _context.Patients.Remove(data);
                _context.SaveChanges();

                return true;
            }
            catch (Exception e)
            {
                return false;

            }



        }
        public Patient getData(int PatientID)
        {
            return _context.Patients.FirstOrDefault(e => e.Id == PatientID);



        }
    }
}

using HelloDocAdmin.Entity.Models;
using HelloDocAdmin.Entity.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocAdmin.Repositories.Interface
{
    public interface IPhysicianRepository
    {
        Task<List<PhysiciansViewModel>> PhysicianAll();
        Task<List<PhysiciansViewModel>> PhysicianByRegion(int? region);
          Task<bool> ChangeNotificationPhysician(Dictionary<int, bool> changedValuesDict);
    }
}

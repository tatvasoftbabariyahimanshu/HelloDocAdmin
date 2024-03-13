﻿using HelloDocAdmin.Entity.Models;
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
        public  Task<bool> EditAccountInfo(PhysiciansViewModel vm);
        public Task<bool> ResetPassword(int Physicianid,string Password);
        public Task<bool> PhysicianAddEdit(PhysiciansViewModel physiciandata, string AdminId);
        public  Task<PhysiciansViewModel> GetPhysicianById(int id);
    }
}
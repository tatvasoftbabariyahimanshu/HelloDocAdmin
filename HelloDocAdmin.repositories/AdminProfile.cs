using HelloDocAdmin.Entity.Data;
using HelloDocAdmin.Entity.Models;
using HelloDocAdmin.Entity.ViewModel;
using HelloDocAdmin.Entity.ViewModels.AdminSite;
using HelloDocAdmin.Repositories.Interface;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocAdmin.Repositories
{
    public class AdminProfile:IAdminProfile
    {

        private readonly ApplicationDbContext _context;
        private readonly EmailConfiguration _email;

        public AdminProfile(ApplicationDbContext context, EmailConfiguration email)
        {
            _context = context;
            _email = email;
        }


        public ViewAdminProfileModel GetDetailsForAdminProfile()
        {
            
            var aspnetuserdata=_context.Aspnetusers.FirstOrDefault(e=>e.Id== "001e35a5-cd12-4ec8-a077-95db9d54da0f");
            ViewAdminProfileModel model = new ViewAdminProfileModel
            {
                ASP_UserName=aspnetuserdata.Username,
                ASP_Password=aspnetuserdata.Passwordhash
                
            };
            var admindata = _context.Admins.FirstOrDefault(e => e.Aspnetuserid == "001e35a5-cd12-4ec8-a077-95db9d54da0f");

            model.ASP_Status = admindata.Status;
            model.ASP_RoleID=admindata.Roleid;

            var listdata= _context.Adminregions.Where(e => e.Adminid == admindata.Adminid).ToList();
            List<int> regionComboBoxes = new List<int>();

            foreach (var region in listdata)
            {
                var rgn=_context.Regions.FirstOrDefault(e => e.Regionid == region.Regionid);
                regionComboBoxes.Add(
                   region.Regionid
                    );
            }
            model.AdminReqionList=regionComboBoxes;

            model.User_PhoneNumber = admindata.Mobile;
            model.User_Email = admindata.Email;
            model.User_FirtName = admindata.Firstname;
            model.User_LastName = admindata.Lastname;
            model.Address1 = admindata.Address1;
            model.Address2 = admindata.Address2;
            model.City = admindata.City;
            model.zip = admindata.Zip;

            return model;
        }

        
        public  bool Edit_Admin_Profile(ViewAdminProfileModel vm)
        {
            try
            {
                if (vm == null)
                {
                    return false;
                }
                else
                {

                    Admin DataForChange = _context.Admins.FirstOrDefault(e => e.Aspnetuserid == "001e35a5-cd12-4ec8-a077-95db9d54da0f");

                    if (DataForChange != null)
                    {

                        DataForChange.Email = vm.User_Email;
                        DataForChange.Firstname = vm.User_FirtName;
                        DataForChange.Lastname = vm.User_LastName;
                        DataForChange.Mobile = vm.User_PhoneNumber;


                        _context.Admins.Update(DataForChange);
                        _context.SaveChanges();

                        List<Adminregion> dataforregion = _context.Adminregions.Where(e => e.Adminid == DataForChange.Adminid).ToList();
                        foreach (var dataitem in dataforregion)
                        {
                            _context.Adminregions.Remove(dataitem);
                            _context.SaveChanges();
                        }
                        foreach (var dataitem2 in vm.AdminReqionList)
                        {
                            Adminregion adr = new Adminregion
                            {
                                Adminid = DataForChange.Adminid,
                                Regionid = dataitem2
                            };

                            _context.Adminregions.Add(adr);
                            _context.SaveChanges();


                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public bool Edit_Billing_Info(ViewAdminProfileModel vm)
        {
            try
            {
                if (vm == null)
                {
                    return false;
                }
                else
                {

                    Admin DataForChange = _context.Admins.FirstOrDefault(e => e.Aspnetuserid == "001e35a5-cd12-4ec8-a077-95db9d54da0f");

                    if (DataForChange != null)
                    {

                        DataForChange.Address1 = vm.Address1;
                        DataForChange.Address2 = vm.Address2;
                        DataForChange.City = vm.City;
                        DataForChange.Mobile = vm.User_PhoneNumber;


                        _context.Admins.Update(DataForChange);
                        _context.SaveChanges();

      
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }



        public bool ChangePassword(string password)
        {
            var hasher = new PasswordHasher<string>();

            string hashedPassword = hasher.HashPassword(null, password);

           var aspnetuser=_context.Aspnetusers.FirstOrDefault(e=>e.Id== "001e35a5-cd12-4ec8-a077-95db9d54da0f");
            if (aspnetuser != null)
            {
                aspnetuser.Passwordhash = hashedPassword;
                _context.Aspnetusers.Update(aspnetuser);
                _context.SaveChanges();
                return true;
            }
            return false;
        }










        }
}

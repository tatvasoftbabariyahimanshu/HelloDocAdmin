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


        public ViewAdminProfileModel GetDetailsForAdminProfile(string id)
        {
            
            var aspnetuserdata=_context.Aspnetusers.FirstOrDefault(e=>e.Id== id);
            ViewAdminProfileModel model = new ViewAdminProfileModel
            {
                ASP_UserName=aspnetuserdata.Username,
                ASP_Password=aspnetuserdata.Passwordhash
                
            };
            var admindata = _context.Admins.FirstOrDefault(e => e.Aspnetuserid == id);

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
            model.AspnetUserID=admindata.Aspnetuserid;
            model.RegionID = (int)admindata.Regionid;

            return model;
        }

        
        public  bool Edit_Admin_Profile(ViewAdminProfileModel vm,string id)
        {
            try
            {
                if (vm == null)
                {
                    return false;
                }
                else
                {

                    Admin DataForChange = _context.Admins.FirstOrDefault(e => e.Aspnetuserid == "787a81c3-1917-41d9-abc6-2e3536b8906c");

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
        public bool Edit_Billing_Info(ViewAdminProfileModel vm,string id)
        {
            try
            {
                if (vm == null)
                {
                    return false;
                }
                else
                {

                    Admin DataForChange = _context.Admins.FirstOrDefault(e => e.Aspnetuserid == "787a81c3-1917-41d9-abc6-2e3536b8906c");

                    if (DataForChange != null)
                    {

                        DataForChange.Address1 = vm.Address1;
                        DataForChange.Address2 = vm.Address2;
                        DataForChange.City = vm.City;
                        DataForChange.Regionid = vm.RegionID;
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



        public bool ChangePassword(string password,string id)
        {
            var hasher = new PasswordHasher<string>();

            string hashedPassword = hasher.HashPassword(null, password);

            var aspnetuser = _context.Aspnetusers.FirstOrDefault(e => e.Id == "787a81c3-1917-41d9-abc6-2e3536b8906c");

            if (aspnetuser != null)
            {
                aspnetuser.Passwordhash = hashedPassword;
                _context.Aspnetusers.Update(aspnetuser);
                _context.SaveChanges();
                return true;
            }
            return false;
        }


        public bool AddAdmin(ViewAdminProfileModel data,string id) {
            try
            {
                if (data != null)
                {
                    var hasher = new PasswordHasher<string>();

                    string hashedPassword = hasher.HashPassword(null, data.ASP_Password);
                    Aspnetuser user = new Aspnetuser()
                    {
                        Username = data.ASP_UserName,
                        Passwordhash = hashedPassword,
                        Email = data.User_Email,
                        CreatedDate = DateTime.Now,
                        Modifieddate = DateTime.Now,
                        Id = Guid.NewGuid().ToString()

                };
                    _context.Aspnetusers.Add(user);
                    _context.SaveChanges();


                    Admin admin = new Admin()
                    {
                        Firstname = data.User_FirtName,
                        Lastname = data.User_LastName,
                        Email = data.User_Email,
                        Mobile = data.User_PhoneNumber,
                        Aspnetuserid = user.Id,
                        Address1 = data.Address1,
                        Address2 = data.Address2,
                        Status = data.ASP_Status,
                        Roleid=data.ASP_RoleID,
                        City = data.City,
                        Regionid = data.RegionID,
                        Zip = data.zip,
                       
                        Createdby = id,
                        Createddate = DateTime.Now,
                        Modifiedby = id,
                        Modifieddate = DateTime.Now,

                    };
                    _context.Admins.Add(admin);
                    _context.SaveChanges();
                    Admin DataForChange = _context.Admins.FirstOrDefault(e => e.Aspnetuserid == admin.Aspnetuserid);
                    List<Adminregion> dataforregion = _context.Adminregions.Where(e => e.Adminid == DataForChange.Adminid).ToList();
                    foreach (var dataitem in dataforregion)
                    {
                        _context.Adminregions.Remove(dataitem);
                        _context.SaveChanges();
                    }
                    foreach (var dataitem2 in data.AdminReqionList)
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
            catch (Exception ex)
            {
                return false;
            }
            
        
        
        
        
        }
        public bool EditAdmin(ViewAdminProfileModel data, string id)
        {
            try
            {
                if (data != null)
                {
                    Aspnetuser asp=_context.Aspnetusers.FirstOrDefault(E=>E.Id==data.AspnetUserID);
                    asp.Username = data.ASP_UserName;
                    asp.Modifieddate = DateTime.Now;
                    _context.Aspnetusers.Update(asp);
                     _context.SaveChanges();



                    Admin adm=_context.Admins.FirstOrDefault(E=>E.Aspnetuserid==data.AspnetUserID);

                    adm.Address1 = data.Address1;
                    adm.Address2 = data.Address2;
                    adm.Altphone = data.User_PhoneNumber;
                    adm.Firstname = data.User_FirtName;
                    adm.Lastname = data.User_LastName;
                    adm.Email = data.User_Email;
                    adm.Mobile = data.User_PhoneNumber;
                    adm.Regionid = data.RegionID;
                    adm.Roleid = data.ASP_RoleID;
                    adm.Status=data.ASP_Status;
                    adm.City = data.City;
                    adm.Zip = data.zip;
                    adm.Modifiedby = id;
                    adm.Modifieddate = DateTime.Now;    


                    _context.Admins.Update(adm);
                    _context.SaveChanges();

                    Admin DataForChange = _context.Admins.FirstOrDefault(e => e.Aspnetuserid == adm.Aspnetuserid);
                    List<Adminregion> dataforregion = _context.Adminregions.Where(e => e.Adminid == DataForChange.Adminid).ToList();
                    foreach (var dataitem in dataforregion)
                    {
                        _context.Adminregions.Remove(dataitem);
                        _context.SaveChanges();
                    }
                    foreach (var dataitem2 in data.AdminReqionList)
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
            catch (Exception ex)
            {
                return false;
            }





        }







    }
}

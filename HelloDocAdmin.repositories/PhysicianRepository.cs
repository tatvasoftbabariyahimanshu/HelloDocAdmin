using GoogleMaps.LocationServices;
using HelloDocAdmin.Entity.Data;
using HelloDocAdmin.Entity.Models;
using HelloDocAdmin.Entity.ViewModels;
using HelloDocAdmin.Entity.ViewModels;
using HelloDocAdmin.Entity.ViewModels.AdminSite;
using HelloDocAdmin.Repositories.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HelloDocAdmin.Repositories
{
    public class PhysicianRepository:IPhysicianRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailConfiguration _email;

        public PhysicianRepository(ApplicationDbContext context, EmailConfiguration email)
        {
            _context = context;
            _email = email;
        }
        #region Index List
       
        public async Task<List<PhysiciansViewModel>> PhysicianAll()
        {
            BitArray bt = new BitArray(1);
            bt.Set(0, false);
            List<PhysiciansViewModel> pl = (from r in _context.Physicians
                                         join Notifications in _context.Physiciannotifications
                                         on r.Physicianid equals Notifications.Physicianid into aspGroup
                                         from nof in aspGroup.DefaultIfEmpty()
                                         join role in _context.Roles
                                         on r.Roleid equals role.Roleid into roleGroup
                                         from roles in roleGroup.DefaultIfEmpty()
                                         where  r.Isdeleted == bt
                                         select new PhysiciansViewModel
                                         {
                                             notificationid = nof.Id,
                                             Createddate = r.Createddate,
                                             Physicianid = r.Physicianid,
                                             Address1 = r.Address1,
                                             Address2 = r.Address2,
                                             Adminnotes = r.Adminnotes,
                                             Altphone = r.Altphone,
                                             Businessname = r.Businessname,
                                             Businesswebsite = r.Businesswebsite,
                                             City = r.City,

                                             Firstname = r.Firstname,
                                             Lastname = r.Lastname,
                                             notification = nof.Isnotificationstopped,
                                             role = roles.Name,
                                             Status = r.Status,
                                             Email = r.Email
                        
                                         })
                                        .ToList();

            return pl;

        }
        #endregion
        #region PhysicianByRegion
        public async Task<List<PhysiciansViewModel>> PhysicianByRegion(int? region)
        {
            BitArray bt = new BitArray(1);
            bt.Set(0, false);

            List<PhysiciansViewModel> pl = await (
                                        from pr in _context.Physicianregions
                                        join ph in _context.Physicians
                                         on pr.Physicianid equals ph.Physicianid into rGroup
                                        from r in rGroup.DefaultIfEmpty()
                                        join Notifications in _context.Physiciannotifications
                                         on r.Physicianid equals Notifications.Physicianid into aspGroup
                                        from nof in aspGroup.DefaultIfEmpty()
                                        join role in _context.Roles
                                        on r.Roleid equals role.Roleid into roleGroup
                                        from roles in roleGroup.DefaultIfEmpty()
                                        where pr.Regionid == region && r.Isdeleted == bt
                                        select new PhysiciansViewModel
                                        {
                                            Createddate = r.Createddate,
                                            Physicianid = r.Physicianid,
                                            Address1 = r.Address1,
                                            Address2 = r.Address2,
                                            Adminnotes = r.Adminnotes,
                                            Altphone = r.Altphone,
                                            Businessname = r.Businessname,
                                            Businesswebsite = r.Businesswebsite,
                                            City = r.City,
                                            Firstname = r.Firstname,
                                            Lastname = r.Lastname,
                                            notification = nof.Isnotificationstopped,
                                            role = roles.Name,
                                            Status = r.Status

                                        })
                                        .ToListAsync();


            return pl;

        }
        #endregion
        #region Change_Notification_Physician

        public async Task<bool> ChangeNotificationPhysician(Dictionary<int, bool> changedValuesDict)
        {
            try
            {
                if (changedValuesDict == null)
                {
                    return false;
                }
                else
                {


                    foreach (var item in changedValuesDict)
                    {
                        var ar = _context.Physiciannotifications.Find(item.Key);
                        if (ar != null)
                        {
                            ar.Isnotificationstopped[0] = item.Value;
                            _context.Physiciannotifications.Update(ar);
                            _context.SaveChanges();
                        }
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        #endregion
        #region reset password
        public async Task<bool> ResetPassword(int Physicianid, string Password)
        {
            try
            {
                var hasher = new PasswordHasher<string>();


                var req = await _context.Physicians
                    .Where(W => W.Physicianid == Physicianid)
                        .FirstOrDefaultAsync();


                if (req != null)
                {
                    var U = await _context.Aspnetusers.Where(m => m.Id == req.Aspnetuserid).FirstOrDefaultAsync();
                    U.Passwordhash = hasher.HashPassword(null, Password);
                    U.Modifieddate = DateTime.Now;

                    _context.Aspnetusers.Update(U);
                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch { return false; }
           
        }
#endregion
        #region SavePhysicianInfo
        public async Task<bool> EditAccountInfo(PhysiciansViewModel vm)
        {
            try
            {
                if (vm == null)
                {
                    return false;
                }
                else
                {
                    var DataForChange = await _context.Physicians
                        .Where(W => W.Physicianid == vm.Physicianid)
                        .FirstOrDefaultAsync();
                    Aspnetuser U = await _context.Aspnetusers.FirstOrDefaultAsync(m => m.Id == DataForChange.Aspnetuserid);

                    if (DataForChange != null)
                    {

                        U.Username = vm.UserName;
                        DataForChange.Status = vm.Status;
                        DataForChange.Roleid = vm.Roleid;


                        _context.Physicians.Update(DataForChange);
                        _context.Aspnetusers.Update(U);
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


        public async Task<bool> EditAdminInfo(PhysiciansViewModel vm)
        {
            try
            {
                if (vm == null)
                {
                    return false;
                }
                else
                {
                    var DataForChange = await _context.Physicians
                        .Where(W => W.Physicianid == vm.Physicianid)
                        .FirstOrDefaultAsync();
                 

                    if (DataForChange != null)
                    {

                       
                        DataForChange.Firstname = vm.Firstname;
                        DataForChange.Lastname  = vm.Lastname;
                        DataForChange.Email=vm.Email;
                        DataForChange.Mobile=vm.Mobile;
                        DataForChange.Medicallicense = vm.Medicallicense;
                        DataForChange.Npinumber = vm.Npinumber;
                        DataForChange.Syncemailaddress = vm.Syncemailaddress;
                         


                        _context.Physicians.Update(DataForChange);
                        List<int> priceList = vm.Regionsid.Split(',').Select(int.Parse).ToList();
                       

                      
                      
                        foreach (var dataitem2 in priceList)
                        {
                            var data = _context.Physicianregions.FirstOrDefault(e => e.Physicianid == vm.Physicianid && e.Regionid == dataitem2);
                            if(data != null)
                            {

                            }
                            else {
                                Physicianregion adr = new Physicianregion
                                {
                                    Physicianid = DataForChange.Physicianid,
                                    Regionid = dataitem2
                                };

                                _context.Physicianregions.Add(adr);
                                _context.SaveChanges();
                            }
                           


                        }



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
        public async Task<bool> DeletePhysician(int PhysicianID, string AdminID)
        {
            try
            {
                BitArray bt = new BitArray(1);
                bt.Set(0, true);
                if (PhysicianID == null)
                {
                    return false;
                }
                else
                {
                    var DataForChange = await _context.Physicians
                        .Where(W => W.Physicianid == PhysicianID)
                        .FirstOrDefaultAsync();


                    if (DataForChange != null)
                    {


                        DataForChange.Isdeleted = bt;
                        DataForChange.Modifieddate = DateTime.Now;
                        DataForChange.Modifiedby = AdminID;
                        _context.Physicians.Update(DataForChange);
                      


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
        public async Task<bool> EditMailBilling(PhysiciansViewModel vm)
        {
            try
            {
                if (vm == null)
                {
                    return false;
                }
                else
                {
                    var DataForChange = await _context.Physicians
                        .Where(W => W.Physicianid == vm.Physicianid)
                        .FirstOrDefaultAsync();
                    var locationchange=await _context.Physicianlocations.FirstOrDefaultAsync(E=>E.Physicianid==vm.Physicianid);

                    if (DataForChange != null)
                    {


                        DataForChange.Address1 = vm.Address1;
                        DataForChange.City = vm.City;
                        DataForChange.Regionid = vm.Regionid;
                        DataForChange.Zip = vm.Zipcode;
                        DataForChange.Altphone = vm.Altphone;
                        _context.Physicians.Update(DataForChange);

                        _context.SaveChanges();



                        if(locationchange !=null)
                        {
                            locationchange.Address = vm.Address1 + ","+ vm.City+","+ vm.Zipcode;
                            var locationService = new GoogleLocationService("AIzaSyARrk6kY-nnnSpReeWotnQxCAo_MoI4qbU");
                            var point = locationService.GetLatLongFromAddress(locationchange.Address);
                            locationchange.Latitude = (decimal?)point.Latitude;
                            locationchange.Longitude = (decimal?)point.Longitude;
                            _context.Physicianlocations.Update(locationchange);
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
        public async Task<bool> EditProviderProfile(PhysiciansViewModel vm, string AdminId)
        {
            try
            {
                if (vm == null)
                {
                    return false;
                }
                else
                {
                    var DataForChange = await _context.Physicians
                        .Where(W => W.Physicianid == vm.Physicianid)
                        .FirstOrDefaultAsync();


                    if (DataForChange != null)
                    {
                        if(vm.PhotoFile!=null)
                        {
                            DataForChange.Photo = vm.PhotoFile != null ? vm.Firstname + "-" + DateTime.Now.ToString("yyyyMMddhhmm") + "-Photo." + Path.GetExtension(vm.PhotoFile.FileName).Trim('.') : null;
                            CM.UploadProviderDoc(vm.PhotoFile, vm.Physicianid, vm.Firstname + "-" + DateTime.Now.ToString("yyyyMMddhhmm") + "-Photo." + Path.GetExtension(vm.PhotoFile.FileName).Trim('.'));

                        }
                        if(vm.SignatureFile!=null)
                        {
                            DataForChange.Signature = vm.SignatureFile != null ? vm.Firstname + "-" + DateTime.Now.ToString("yyyyMMddhhmm") + "-Signature.png" : null;
                            CM.UploadProviderDoc(vm.SignatureFile, vm.Physicianid, vm.Firstname + "-" + DateTime.Now.ToString("yyyyMMddhhmm") + "-Signature.png");
                        }
                     


                        DataForChange.Businessname = vm.Businessname;
                        DataForChange.Businesswebsite = vm.Businesswebsite;
                        DataForChange.Modifiedby = AdminId;
                        DataForChange.Adminnotes = vm.Adminnotes;
                        DataForChange.Modifieddate=DateTime.Now;
                        _context.Physicians.Update(DataForChange);

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
        #endregion
        #region PhysicianAddEdit
        public async Task<bool> PhysicianAddEdit(PhysiciansViewModel physiciandata, string AdminId)
        {
            try
            {
                if (physiciandata.UserName != null /*&& physiciandata.PassWord != null*/)
                {
                    var Aspnetuser = new Aspnetuser();
                    var hasher = new PasswordHasher<string>();
                    Aspnetuser.Id = Guid.NewGuid().ToString();
                    Aspnetuser.Username = physiciandata.UserName;
                    //Aspnetuser.Passwordhash = hasher.HashPassword(null, physiciandata.PassWord);
                    Aspnetuser.Email = physiciandata.Email;
                    Aspnetuser.CreatedDate = DateTime.Now;
                    _context.Aspnetusers.Add(Aspnetuser);
                    _context.SaveChanges();


                    BitArray bt = new BitArray(1);
                    bt.Set(0, true); 



                    // Physician
                    var Physician = new Physician();
                    Physician.Aspnetuserid = Aspnetuser.Id;
                    Physician.Firstname = physiciandata.Firstname;
                    Physician.Lastname = physiciandata.Lastname;
                    Physician.Status = physiciandata.Status;
                    Physician.Roleid = physiciandata.Roleid;
                    Physician.Email = physiciandata.Email;
                    Physician.Mobile = physiciandata.Mobile;
                    Physician.Medicallicense = physiciandata.Medicallicense;
                    Physician.Npinumber = physiciandata.Npinumber;
                    Physician.Syncemailaddress = physiciandata.Syncemailaddress;
                    Physician.Address1 = physiciandata.Address1;
                    Physician.Address2 = physiciandata.Address2;
                    Physician.City = physiciandata.City;
                    Physician.Zip = physiciandata.Zipcode;
                    Physician.Altphone = physiciandata.Altphone;
                    Physician.Businessname = physiciandata.Businessname;
                    Physician.Businesswebsite = physiciandata.Businesswebsite;
                    Physician.Createddate = DateTime.Now;
                    Physician.Createdby = AdminId;
                    Physician.Regionid = physiciandata.Regionid;
                    Physician.Isdeleted = bt;
                    Physician.Isagreementdoc = new BitArray(1);
                    Physician.Isbackgrounddoc = new BitArray(1);
                    Physician.Isnondisclosuredoc = new BitArray(1);
                    Physician.Islicensedoc = new BitArray(1);
                    Physician.Istrainingdoc = new BitArray(1);

                    Physician.Isagreementdoc[0] = physiciandata.Isagreementdoc;
                    Physician.Isbackgrounddoc[0] = physiciandata.Isbackgrounddoc;
                    Physician.Isnondisclosuredoc[0] = physiciandata.Isnondisclosuredoc;
                    Physician.Islicensedoc[0] = physiciandata.Islicensedoc;
                    Physician.Istrainingdoc[0] = physiciandata.Istrainingdoc;
                    Physician.Adminnotes = physiciandata.Adminnotes;
                    Physician.Status = physiciandata.Status;


                    Physician.Photo = physiciandata.PhotoFile != null ? Physician.Firstname + "-" + DateTime.Now.ToString("yyyyMMddhhmm") + "-Photo." + Path.GetExtension(physiciandata.PhotoFile.FileName).Trim('.') : null;
                    Physician.Signature = physiciandata.SignatureFile != null ? Physician.Firstname + "-" + DateTime.Now.ToString("yyyyMMddhhmm") + "-Signature.png" : null;



                    _context.Physicians.Add(Physician);
                    _context.SaveChanges();

                   

                    Physiciannotification nf=new Physiciannotification();
                    nf.Physicianid = Physician.Physicianid;
                    nf.Isnotificationstopped = bt;
                    _context.Physiciannotifications.Add(nf);
                    _context.SaveChanges();

                    CM.UploadProviderDoc(physiciandata.Agreementdoc, Physician.Physicianid, "Agreementdoc.pdf");
                    CM.UploadProviderDoc(physiciandata.BackGrounddoc, Physician.Physicianid, "BackGrounddoc.pdf");
                    CM.UploadProviderDoc(physiciandata.NonDisclosuredoc, Physician.Physicianid, "NonDisclosuredoc.pdf");
                    CM.UploadProviderDoc(physiciandata.Licensedoc, Physician.Physicianid, "Agreementdoc.pdf");
                    CM.UploadProviderDoc(physiciandata.Trainingdoc, Physician.Physicianid, "Trainingdoc.pdf");

                    CM.UploadProviderDoc(physiciandata.SignatureFile, Physician.Physicianid, Physician.Firstname + "-" + DateTime.Now.ToString("yyyyMMddhhmm") + "-Signature.png");
                    CM.UploadProviderDoc(physiciandata.PhotoFile, Physician.Physicianid, Physician.Firstname + "-" + DateTime.Now.ToString("yyyyMMddhhmm") + "-Photo." + Path.GetExtension(physiciandata.PhotoFile.FileName).Trim('.'));

                    List<int> priceList = physiciandata.Regionsid.Split(',').Select(int.Parse).ToList();
                    foreach (var item in priceList)
                    {
                        Physicianregion ar = new Physicianregion();
                        ar.Regionid = item;
                        ar.Physicianid = (int)Physician.Physicianid;
                        _context.Physicianregions.Add(ar);
                        _context.SaveChanges();

                    }

                    Physicianlocation pl=new Physicianlocation();
                    pl.Physicianid = Physician.Physicianid;
                    pl.Address = physiciandata.Address1 +","+ physiciandata.City +","+ physiciandata.Zipcode;
                    var locationService = new GoogleLocationService("AIzaSyARrk6kY-nnnSpReeWotnQxCAo_MoI4qbU");
                    var point = locationService.GetLatLongFromAddress(pl.Address);
                    pl.Latitude= (decimal?)point.Latitude;
                    pl.Longitude = (decimal?)point.Longitude;
                    pl.Createddate= DateTime.Now;
                    pl.Physicianname = physiciandata.Firstname + " " + physiciandata.Lastname;
                    _context.Physicianlocations.Add(pl);
                    _context.SaveChanges();

                    return true;


                }
                    else
                    {
                    return false;
                    }
                
            }
            catch (Exception e)
            {
                return false;
            } 
          
        }
        #endregion
        #region GetPhysicianById
        public async Task<PhysiciansViewModel> GetPhysicianById(int id)
        {


            PhysiciansViewModel pl = await (from r in _context.Physicians
                                   join Aspnetuser in _context.Aspnetusers
                                   on r.Aspnetuserid equals Aspnetuser.Id into aspGroup
                                   from asp in aspGroup.DefaultIfEmpty()
                                          
                                            join Notifications in _context.Physiciannotifications
                                    on r.Physicianid equals Notifications.Physicianid into PhyNGroup
                                   from nof in PhyNGroup.DefaultIfEmpty()
                                   join role in _context.Roles
                                   on r.Roleid equals role.Roleid into roleGroup
                                   from roles in roleGroup.DefaultIfEmpty()
                                   where r.Physicianid == id
                                   select new PhysiciansViewModel
                                   {
                                       UserName = asp.Username,
                                       Roleid = r.Roleid,
                                       Status = r.Status,
                                       notificationid = nof.Id,
                                       Createddate = r.Createddate,
                                       Physicianid = r.Physicianid,
                                       Address1 = r.Address1,
                                       Address2 = r.Address2,
                                       Adminnotes = r.Adminnotes,
                                       Altphone = r.Altphone,
                                       Businessname = r.Businessname,
                                       Businesswebsite = r.Businesswebsite,
                                       City = r.City,
                                      Regionid = r.Regionid,
                                     Mobile=r.Mobile,
                                     Zipcode=r.Zip,
                                     Medicallicense=r.Medicallicense,
                                     Npinumber=r.Npinumber,
                                     Syncemailaddress=r.Syncemailaddress,
                                
                                  
                                       Firstname = r.Firstname,
                                       Lastname = r.Lastname,
                                       notification = nof.Isnotificationstopped,
                                       role = roles.Name,
                                       Email = r.Email,
                                       Photo = r.Photo,
                                       Signature = r.Signature,
                                       Isagreementdoc = r.Isagreementdoc[0],
                                       Isnondisclosuredoc = r.Isnondisclosuredoc[0],
                                       Isbackgrounddoc = r.Isbackgrounddoc[0],
                                       Islicensedoc = r.Islicensedoc[0],
                                       Istrainingdoc = r.Istrainingdoc[0]

                                   })
                                   .FirstOrDefaultAsync();

            List< PhysiciansViewModel.Regions> regions = new List<PhysiciansViewModel.Regions>();

            regions = _context.Physicianregions
                  .Where(r => r.Physicianid == pl.Physicianid)
                  .Select(req => new PhysiciansViewModel.Regions()
                  {
                      regionid = req.Regionid
                  })
                  .ToList();
            if(regions!=null)
            {
                pl.Regionids = regions;
            }
          

            return pl;

        }
        #endregion

    }
}

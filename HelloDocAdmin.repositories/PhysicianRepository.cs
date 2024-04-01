using GoogleMaps.LocationServices;
using HelloDocAdmin.Entity.Data;
using HelloDocAdmin.Entity.Models;
using HelloDocAdmin.Entity.ViewModels;
using HelloDocAdmin.Entity.ViewModels.AdminSite;
using HelloDocAdmin.Repositories.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace HelloDocAdmin.Repositories
{
    public class PhysicianRepository : IPhysicianRepository
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
                                            where r.Isdeleted == bt
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
                        DataForChange.Lastname = vm.Lastname;
                        DataForChange.Email = vm.Email;
                        DataForChange.Mobile = vm.Mobile;
                        DataForChange.Medicallicense = vm.Medicallicense;
                        DataForChange.Npinumber = vm.Npinumber;
                        DataForChange.Syncemailaddress = vm.Syncemailaddress;
                        _context.Physicians.Update(DataForChange);
                        if (vm.Regionsid != null)
                        {
                            var details = _context.Physicianregions.Where(e => e.Physicianid == vm.Physicianid);

                            _context.Physicianregions.RemoveRange(details);
                            _context.SaveChanges();

                            List<int> priceList = vm.Regionsid.Split(',').Select(int.Parse).ToList();
                            foreach (var dataitem2 in priceList)
                            {
                                var data = _context.Physicianregions.FirstOrDefault(e => e.Physicianid == vm.Physicianid && e.Regionid == dataitem2);
                                if (data != null)
                                {

                                }
                                else
                                {
                                    Physicianregion adr = new Physicianregion
                                    {
                                        Physicianid = DataForChange.Physicianid,
                                        Regionid = dataitem2
                                    };
                                    _context.Physicianregions.Add(adr);
                                    _context.SaveChanges();
                                }
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
                    var locationchange = await _context.Physicianlocations.FirstOrDefaultAsync(E => E.Physicianid == vm.Physicianid);

                    if (DataForChange != null)
                    {


                        DataForChange.Address1 = vm.Address1;
                        DataForChange.City = vm.City;
                        DataForChange.Regionid = vm.Regionid;
                        DataForChange.Zip = vm.Zipcode;
                        DataForChange.Altphone = vm.Altphone;
                        _context.Physicians.Update(DataForChange);

                        _context.SaveChanges();



                        if (locationchange != null)
                        {
                            locationchange.Address = vm.Address1 + "," + vm.City + "," + vm.Zipcode;
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
                        if (vm.PhotoFile != null)
                        {
                            DataForChange.Photo = vm.PhotoFile != null ? vm.Firstname + "-" + DateTime.Now.ToString("yyyyMMddhhmm") + "-Photo." + Path.GetExtension(vm.PhotoFile.FileName).Trim('.') : null;
                            CM.UploadProviderDoc(vm.PhotoFile, vm.Physicianid, vm.Firstname + "-" + DateTime.Now.ToString("yyyyMMddhhmm") + "-Photo." + Path.GetExtension(vm.PhotoFile.FileName).Trim('.'));

                        }
                        if (vm.SignatureFile != null)
                        {
                            DataForChange.Signature = vm.SignatureFile != null ? vm.Firstname + "-" + DateTime.Now.ToString("yyyyMMddhhmm") + "-Signature.png" : null;
                            CM.UploadProviderDoc(vm.SignatureFile, vm.Physicianid, vm.Firstname + "-" + DateTime.Now.ToString("yyyyMMddhhmm") + "-Signature.png");
                        }



                        DataForChange.Businessname = vm.Businessname;
                        DataForChange.Businesswebsite = vm.Businesswebsite;
                        DataForChange.Modifiedby = AdminId;
                        DataForChange.Adminnotes = vm.Adminnotes;
                        DataForChange.Modifieddate = DateTime.Now;
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



                    Physiciannotification nf = new Physiciannotification();
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

                    Physicianlocation pl = new Physicianlocation();
                    pl.Physicianid = Physician.Physicianid;
                    pl.Address = physiciandata.Address1 + "," + physiciandata.City + "," + physiciandata.Zipcode;
                    var locationService = new GoogleLocationService("AIzaSyARrk6kY-nnnSpReeWotnQxCAo_MoI4qbU");
                    var point = locationService.GetLatLongFromAddress(pl.Address);
                    pl.Latitude = (decimal?)point.Latitude;
                    pl.Longitude = (decimal?)point.Longitude;
                    pl.Createddate = DateTime.Now;
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
                                                Mobile = r.Mobile,
                                                Zipcode = r.Zip,
                                                Medicallicense = r.Medicallicense,
                                                Npinumber = r.Npinumber,
                                                Syncemailaddress = r.Syncemailaddress,


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

            List<PhysiciansViewModel.Regions> regions = new List<PhysiciansViewModel.Regions>();

            regions = _context.Physicianregions
                  .Where(r => r.Physicianid == pl.Physicianid)
                  .Select(req => new PhysiciansViewModel.Regions()
                  {
                      regionid = req.Regionid
                  })
                  .ToList();
            if (regions != null)
            {
                pl.Regionids = regions;
            }


            return pl;

        }
        #endregion


        #region Create Shift
        public async Task<bool> CreateShift(Schedule s, string id)
        {
            try
            {
                Shift shift = new Shift();
                shift.Physicianid = s.Physicianid;
                shift.Repeatupto = s.Repeatupto;
                shift.Startdate = s.Startdate;
                shift.Createdby = id;
                shift.Createddate = DateTime.Now;
                shift.Isrepeat = new BitArray(1);
                shift.Isrepeat[0] = s.Isrepeat;
                _context.Shifts.Add(shift);
                _context.SaveChanges();


                Shiftdetail sd = new Shiftdetail();
                sd.Shiftid = shift.Shiftid;
                sd.Shiftdate = new DateTime(s.Startdate.Year, s.Startdate.Month, s.Startdate.Day);
                sd.Starttime = s.Starttime;
                sd.Endtime = s.Endtime;
                sd.Regionid = s.Regionid;
                sd.Status = s.Status;
                sd.Isdeleted = new BitArray(1);
                sd.Isdeleted[0] = false;
                _context.Shiftdetails.Add(sd);
                _context.SaveChanges();
                if (s.checkWeekday != null)
                {

                    List<int> day = s.checkWeekday.Split(',').Select(int.Parse).ToList();


                    foreach (int d in day)
                    {
                        DayOfWeek df = (DayOfWeek)d;
                        DateTime today = DateTime.Today;
                        DateTime nextrepete = new DateTime(s.Startdate.Year, s.Startdate.Month, s.Startdate.Day);
                        int found = 0;
                        while (found < s.Repeatupto)
                        {
                            if (nextrepete.DayOfWeek == df)
                            {
                                Shiftdetail sdd = new Shiftdetail();
                                sdd.Shiftid = shift.Shiftid;
                                sdd.Shiftdate = nextrepete;
                                sdd.Starttime = s.Starttime;
                                sdd.Endtime = s.Endtime;
                                sdd.Regionid = s.Regionid;
                                sdd.Status = s.Status;
                                sdd.Isdeleted = new BitArray(1);
                                sdd.Isdeleted[0] = false;
                                _context.Shiftdetails.Add(sdd);
                                _context.SaveChanges();
                                Shiftdetailregion srr = new Shiftdetailregion();
                                srr.Shiftdetailid = sdd.Shiftdetailid;
                                srr.Regionid = s.Regionid;
                                srr.Isdeleted = new BitArray(1);
                                srr.Isdeleted[0] = false;
                                _context.Shiftdetailregions.Add(srr);
                                _context.SaveChanges();
                                found++;


                            }
                            nextrepete = nextrepete.AddDays(1);
                        }
                    }

                }


                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        #endregion


        #region Get Shift By Month
        public async Task<List<Schedule>> GetShift(int month)
        {
            BitArray bt = new BitArray(1);
            bt.Set(0, false);
            List<Schedule> ScheduleDetails = new List<Schedule>();
            var uniqueDates = await _context.Shiftdetails
                            .Where(sd => sd.Shiftdate.Month == month)
                            .Select(sd => sd.Shiftdate.Date)
                            .Distinct()
                            .ToListAsync();
            foreach (DateTime schedule in uniqueDates)
            {



                List<Schedule> ss = await (from s in _context.Shifts
                                           join pd in _context.Physicians
                                           on s.Physicianid equals pd.Physicianid
                                           join sd in _context.Shiftdetails
                                           on s.Shiftid equals sd.Shiftid into shiftGroup
                                           from sd in shiftGroup.DefaultIfEmpty()
                                           where sd.Shiftdate == schedule && sd.Isdeleted == bt && sd.Shiftdate.Month == month
                                           select new Schedule
                                           {
                                               Shiftid = sd.Shiftdetailid,
                                               Status = sd.Status,
                                               Starttime = sd.Starttime,
                                               Endtime = sd.Endtime,
                                               PhysicianName = pd.Firstname + ' ' + pd.Lastname,
                                           })
                                              .ToListAsync();

                Schedule temp = new Schedule();
                temp.ShiftDate = schedule;
                temp.DayList = ss;
                ScheduleDetails.Add(temp);
            }




            return ScheduleDetails;
        }
        #endregion





        #region GetShiftByShiftdetailId
        public async Task<Schedule> GetShiftByShiftdetailId(int Shiftdetailid)
        {
            BitArray bt = new BitArray(1);
            bt.Set(0, false);

            Schedule schedule = (from sh in _context.Shifts
                                 join ph in _context.Physicians
                                 on sh.Physicianid equals ph.Physicianid
                                 join sd in _context.Shiftdetails
                                 on sh.Shiftid equals sd.Shiftid into shiftdetailsgroup
                                 from sd in shiftdetailsgroup.DefaultIfEmpty()
                                 join Region in _context.Regions
                                 on sd.Regionid equals Region.Regionid
                                 where sd.Shiftdetailid == Shiftdetailid
                                 select new Schedule
                                 {
                                     Regionid = (int)sd.Regionid,
                                     Shiftid = sd.Shiftdetailid,
                                     Status = sd.Status,
                                     Physicianid = ph.Physicianid,
                                     Starttime = sd.Starttime,
                                     Endtime = sd.Endtime,
                                     PhysicianName = ph.Firstname + " " + ph.Lastname,
                                     ShiftDate = sd.Shiftdate

                                 }).FirstOrDefault();






            return schedule;

        }
        #endregion
        #region UpdateStatusShift
        public async Task<bool> UpdateStatusShift(string s, string AdminID)
        {
            List<int> shidtID = s.Split(',').Select(int.Parse).ToList();
            try
            {
                foreach (int i in shidtID)
                {
                    Shiftdetail sd = _context.Shiftdetails.FirstOrDefault(sd => sd.Shiftdetailid == i);
                    if (sd != null)
                    {
                        sd.Status = (short)(sd.Status == 1 ? 0 : 1);
                        sd.Modifiedby = AdminID;
                        sd.Modifieddate = DateTime.Now;
                        _context.Shiftdetails.Update(sd);
                        _context.SaveChanges();
                    }
                    else
                    {
                        return false;
                    }
                }



                return true;

            }
            catch (Exception ex)
            {
                return false;
            }

        }
        #endregion

        #region EditShift
        public async Task<bool> EditShift(Schedule s, string AdminID)
        {
            try
            {
                Shiftdetail sd = _context.Shiftdetails.FirstOrDefault(sd => sd.Shiftdetailid == s.Shiftid);
                if (sd != null)
                {
                    sd.Shiftdate = (DateTime)s.ShiftDate;
                    sd.Starttime = s.Starttime;
                    sd.Endtime = s.Endtime;
                    sd.Modifiedby = AdminID;
                    sd.Modifieddate = DateTime.Now;
                    _context.Shiftdetails.Update(sd);
                    _context.SaveChanges();
                }
                else
                {
                    return false;
                }

                return true;

            }
            catch (Exception ex)
            {
                return false;
            }

        }
        #endregion
        #region DeleteShift
        public async Task<bool> DeleteShift(string s, string AdminID)
        {
            BitArray bt = new BitArray(1);
            bt.Set(0, true);
            List<int> shidtID = s.Split(',').Select(int.Parse).ToList();
            try
            {
                foreach (int i in shidtID)
                {
                    Shiftdetail sd = _context.Shiftdetails.FirstOrDefault(sd => sd.Shiftdetailid == i);
                    if (sd != null)
                    {
                        sd.Isdeleted = bt;
                        sd.Modifiedby = AdminID;
                        sd.Modifieddate = DateTime.Now;
                        _context.Shiftdetails.Update(sd);
                        _context.SaveChanges();
                    }
                    else
                    {
                        return false;
                    }
                }



                return true;

            }
            catch (Exception ex)
            {
                return false;
            }

        }
        #endregion
        #region PhysicianAll
        public async Task<List<Schedule>> PhysicianAll1()
        {
            BitArray bt = new BitArray(1);
            bt.Set(0, false);
            List<Schedule> ScheduleDetails = new List<Schedule>();

            List<Physicians> pl = await (from r in _context.Physicians
                                         join Notifications in _context.Physiciannotifications
                                         on r.Physicianid equals Notifications.Physicianid into aspGroup
                                         from nof in aspGroup.DefaultIfEmpty()
                                         join role in _context.Roles
                                         on r.Roleid equals role.Roleid into roleGroup
                                         from roles in roleGroup.DefaultIfEmpty()
                                         where r.Isdeleted == bt
                                         select new Physicians
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
                                             Email = r.Email,
                                             Photo = r.Photo

                                         })
                                        .ToListAsync();
            foreach (Physicians schedule in pl)
            {
                List<Schedule> ss = await (from s in _context.Shifts
                                           join pd in _context.Physicians
                                           on s.Physicianid equals pd.Physicianid
                                           join sd in _context.Shiftdetails
                                           on s.Shiftid equals sd.Shiftid into shiftGroup
                                           from sd in shiftGroup.DefaultIfEmpty()
                                           join rg in _context.Regions
                                           on sd.Regionid equals rg.Regionid
                                           where s.Physicianid == schedule.Physicianid && sd.Isdeleted == new BitArray(1)
                                           select new Schedule
                                           {
                                               RegionName = rg.Name,
                                               Shiftid = sd.Shiftdetailid,
                                               Status = sd.Status,
                                               Starttime = sd.Starttime,
                                               ShiftDate = sd.Shiftdate,
                                               Endtime = sd.Endtime,
                                               PhysicianName = pd.Firstname + ' ' + pd.Lastname,

                                           })
                                              .ToListAsync();

                Schedule temp = new Schedule();
                temp.PhysicianName = schedule.Firstname + ' ' + schedule.Lastname;
                temp.PhysicianPhoto = schedule.Photo;
                temp.Physicianid = (int)schedule.Physicianid;
                temp.DayList = ss;
                ScheduleDetails.Add(temp);
            }

            return ScheduleDetails;


        }
        #endregion


        #region PhysicianOnCall
        public async Task<List<Physicians>> PhysicianOnCall(int? region)
        {
            DateTime currentDateTime = DateTime.Now;
            TimeOnly currentTimeOfDay = TimeOnly.FromDateTime(DateTime.Now);

            List<Physicians> pl = await (from r in _context.Physicians
                                         where r.Isdeleted == new BitArray(1)
                                         select new Physicians
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
                                             Status = r.Status,
                                             Email = r.Email,
                                             Photo = r.Photo

                                         }).ToListAsync();



            foreach (var item in pl)
            {
                List<int> shifts = await (from s in _context.Shifts
                                          where s.Physicianid == item.Physicianid
                                          select s.Shiftid).ToListAsync();
                foreach (var data in shifts)
                {
                    var shiftDetails = (from sd in _context.Shiftdetails
                                        where sd.Shiftid == data &&
                                        sd.Shiftdate.Date == currentDateTime.Date &&
                                        sd.Starttime <= currentTimeOfDay && currentTimeOfDay <= sd.Endtime
                                        select sd).FirstOrDefault();



                    if (shiftDetails != null)
                    {
                        item.onCallStatus = 1;
                    }
                }
            }


            return pl;
        }
        #endregion
    }
}

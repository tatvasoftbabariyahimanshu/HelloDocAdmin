using HelloDocAdmin.Entity.Data;
using HelloDocAdmin.Entity.Models;
using HelloDocAdmin.Entity.ViewModels.AdminSite;
using HelloDocAdmin.Repositories.Interface;
using Microsoft.IdentityModel.Tokens;
using System.Collections;

namespace HelloDocAdmin.Repositories
{
    public class VendorRepository : IVendorRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailConfiguration _email;

        public VendorRepository(ApplicationDbContext context, EmailConfiguration email)
        {
            _context = context;
            _email = email;
        }
        public VendorData getallvendor(string? vendorname, int? helthprofessionaltype, int pagesize = 5, int currentpage = 1)
        {
            BitArray bt = new BitArray(1);
            bt.Set(0, false);
            VendorData dm = new VendorData();

            IQueryable<VendorListView> data = (from Vendor in _context.Healthprofessionals
                                               join type in _context.Healthprofessionaltypes
                                               on Vendor.Profession equals type.Healthprofessionalid
                                               where Vendor.Isdeleted == bt
                                               select new VendorListView
                                               {
                                                   VendorID = Vendor.Vendorid,
                                                   Profession = type.Professionname,
                                                   BusinessContact = Vendor.Businesscontact ?? "",
                                                   Email = Vendor.Email,
                                                   FaxNumber = Vendor.Faxnumber,
                                                   PhoneNumber = Vendor.Phonenumber,
                                                   VendorName = Vendor.Vendorname,
                                                   helthprofessiontypeID = type.Healthprofessionalid,

                                               });
            if (helthprofessionaltype != 0)
            {
                data = data.Where(r => r.helthprofessiontypeID == helthprofessionaltype);
            }
            if (!vendorname.IsNullOrEmpty())

            {
                data = data.Where(r => r.VendorName.ToLower().Contains(vendorname.ToLower()));
            }
            dm.TotalPage = (int)Math.Ceiling((double)data.Count() / pagesize);
            data = data.Skip((currentpage - 1) * pagesize).Take(pagesize);


            dm.List = data.ToList();
            dm.pageSize = pagesize;
            dm.CurrentPage = currentpage;
            return dm;
        }
        public Healthprofessional gethelthprofessionaldetails(int vendorid)
        {
            Healthprofessional Data = _context.Healthprofessionals.FirstOrDefault(E => E.Vendorid == vendorid);
            return Data;
        }
        public bool delete(int? vendorid)
        {
            BitArray bt = new BitArray(1);
            bt.Set(0, true);
            try
            {
                Healthprofessional hp = _context.Healthprofessionals.FirstOrDefault(E => E.Vendorid == vendorid);
                hp.Isdeleted = bt;
                hp.Modifieddate = DateTime.Now;

                _context.Healthprofessionals.Update(hp);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool addVendor(Healthprofessional model)
        {
            var region = _context.Regions.FirstOrDefault(u => u.Name == model.State.Trim().ToLower().Replace(" ", ""));
            BitArray bt = new BitArray(1);
            bt.Set(0, false);
            try
            {
                Healthprofessional hp = new Healthprofessional();
                hp.Vendorname = model.Vendorname;
                hp.Address = model.Address;
                hp.Profession = model.Profession;
                hp.Zip = model.Zip;
                hp.City = model.City;
                hp.Businesscontact = model.Businesscontact;
                hp.Email = model.Email;
                hp.Createddate = DateTime.Now;
                hp.Faxnumber = model.Faxnumber;
                hp.Phonenumber = model.Phonenumber;
                hp.Isdeleted = bt;
                hp.Modifieddate = DateTime.Now;
                hp.State = model.State;
                hp.Regionid = region.Regionid;

                _context.Healthprofessionals.Add(hp);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool EditVendor(Healthprofessional model)
        {
            var region = _context.Regions.FirstOrDefault(u => u.Name == model.State.Trim().ToLower().Replace(" ", ""));

            try
            {
                Healthprofessional hp = _context.Healthprofessionals.FirstOrDefault(E => E.Vendorid == model.Vendorid);



                hp.Vendorname = model.Vendorname;
                hp.Address = model.Address;
                hp.Profession = model.Profession;
                hp.Zip = model.Zip;
                hp.City = model.City;
                hp.Businesscontact = model.Businesscontact;
                hp.Email = model.Email;
                hp.Faxnumber = model.Faxnumber;
                hp.Phonenumber = model.Phonenumber;
                hp.Modifieddate = DateTime.Now;
                hp.State = model.State;
                hp.Regionid = region.Regionid;

                _context.Healthprofessionals.Update(hp);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }

        }

        public int isBusinessNameExist(string businessName)
        {
            int data = _context.Healthprofessionals.Count(e => e.Vendorname.ToLower().Equals(businessName.ToLower()));
            return data;
        }
        public int isEmailExist(string Email)
        {
            int data = _context.Healthprofessionals.Count(e => e.Email.ToLower().Equals(Email.ToLower()));
            return data;
        }
    }
}

using HelloDocAdmin.Entity.Data;
using HelloDocAdmin.Entity.Models;
using HelloDocAdmin.Entity.ViewModels.AdminSite;
using HelloDocAdmin.Repositories.Interface;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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
        public List<VendorListView> getallvendor()
        {
            BitArray bt = new BitArray(1);
            bt.Set(0, false);
            List<VendorListView> list = (from Vendor in _context.Healthprofessionals
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

                                         }


                                         ).ToList();
            return list;
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
            }catch (Exception ex)
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
    }
}

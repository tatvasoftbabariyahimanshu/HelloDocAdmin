
using HelloDocAdmin.Entity.Data;
using HelloDocAdmin.Entity.Models;
using HelloDocAdmin.Entity.ViewModel;
using HelloDocAdmin.Entity.ViewModels;
using HelloDocAdmin.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace HelloDocAdmin.Repositories
{
    public class Combobox : ICombobox
    {
        private readonly ApplicationDbContext _context;

        public Combobox(ApplicationDbContext context)
        {
            _context = context;

        }
        public async Task<List<HealthprofessionaltypeCombobox>> healthprofessionaltype()
        {
            return await _context.Healthprofessionaltypes.Select(req => new HealthprofessionaltypeCombobox()
            {
                HealthprofessionalID = req.Healthprofessionalid,
                ProfessionName = req.Professionname
            })
               .ToListAsync();
        }
        public async Task<List<HealthprofessionalCombobox>> healthprofessionals()
        {
            return await _context.Healthprofessionals.Select(req => new HealthprofessionalCombobox()
            {
                VendorID = req.Vendorid,
                VendorName = req.Vendorname
            })
               .ToListAsync();
        }
        public async Task<List<RegionComboBox>> RegionComboBox()
        {
            return await _context.Regions.Select(req => new RegionComboBox()
            {
                RegionId = req.Regionid,
                RegionName = req.Name
            })
                .ToListAsync();
        }
        public async Task<List<UserRoleCombobox>> UserRole()
        {
            return await _context.Aspnetroles.Select(req => new UserRoleCombobox()
            {
                RoleId = req.Id,
                RoleName = req.Name
            })
                .ToListAsync();
        }
        public async Task<List<RoleComboBox>> RolelistAdmin()
        {
            return await _context.Roles.Where(e => e.Accounttype == 2).Select(req => new RoleComboBox()
            {
                RoleID = req.Roleid,
                Name = req.Name
            })
                .ToListAsync();
        }
        public async Task<List<RoleComboBox>> RolelistProvider()
        {
            return await _context.Roles.Where(e => e.Accounttype == 3).Select(req => new RoleComboBox()
            {
                RoleID = req.Roleid,
                Name = req.Name
            })
                .ToListAsync();
        }
        public async Task<List<CaseReasonComboBox>> CaseReasonComboBox()
        {
            return await _context.Casetags.Select(req => new CaseReasonComboBox()
            {
                CaseReasonId = req.Casetagid,
                CaseReasonName = req.Name
            })
                .ToListAsync();
        }
        #region Provider_By_Region
        public List<Physician> ProviderbyRegion(int? regionid)
        {
            BitArray bt = new BitArray(1);
            bt.Set(0, false);
            var result = _context.Physicians
                        .Where(r => r.Regionid == regionid && r.Isdeleted == bt)
                        .OrderByDescending(x => x.Createddate)
                        .ToList();

            return result;
        }
        #endregion

        public List<HealthprofessionalCombobox> ProfessionalByType(int? HealthprofessionalID)
        {
            BitArray bt = new BitArray(1);
            bt.Set(0, false);
            var result = _context.Healthprofessionals
                        .Where(r => r.Profession == HealthprofessionalID && r.Isdeleted == bt)

                        .Select(req => new HealthprofessionalCombobox()
                        {
                            VendorID = req.Vendorid,
                            VendorName = req.Vendorname
                        }).ToList();

            return result;
        }

    }
}

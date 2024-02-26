﻿
using HelloDocAdmin.Entity.Data;
using HelloDocAdmin.Entity.Models;
using HelloDocAdmin.Entity.ViewModel;
using HelloDocAdmin.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocAdmin.Repositories
{
    public class Combobox : ICombobox
    {
        private readonly ApplicationDbContext _context;
      
        public Combobox(ApplicationDbContext context)
        {
            _context = context;
            
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
        public  List<Physician> ProviderbyRegion(int? regionid)
        {
            var result =  _context.Physicians
                        .Where(r => r.Regionid == regionid)
                        .OrderByDescending(x => x.Createddate)
                        .ToList();

            return result;
        }
        #endregion

    }
}

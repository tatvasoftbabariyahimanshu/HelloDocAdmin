﻿using HelloDocAdmin.Entity.Data;
using HelloDocAdmin.Entity.Models;
using HelloDocAdmin.Entity.ViewModels.AdminSite;
using HelloDocAdmin.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HelloDocAdmin.Repositories
{
    public  class AccessRepository:IAccessRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailConfiguration _email;

        public AccessRepository(ApplicationDbContext context, EmailConfiguration email)
        {
            _context = context;
            _email = email;
        }
        #region GetRoleAccessDetails
        public  List<Role> GetRoleAccessDetails()
        {

            List<Role> v =  _context.Roles.ToList();

            return v;

        }
        #endregion

        #region GetRoleByMenus
        public  RolesModel GetRoleByMenus(int roleid)
        {
            return  _context.Roles
                        .Where(r => r.Roleid == roleid)
                        .Select(r => new RolesModel
                        {
                            Accounttype = r.Accounttype,
                            Createdby = r.Createdby,
                            Roleid = r.Roleid,
                            Name = r.Name,
                            Isdeleted = r.Isdeleted
                        })
                        .FirstOrDefault();
        }
        #endregion

        #region GetMenusByAccount
        public async Task<List<Menu>> GetMenusByAccount(short Accounttype)
        {
            return await _context.Menus.Where(r => r.Accounttype == Accounttype).ToListAsync();
        }
        #endregion

     

        #region PostRoleMenu
        public async Task<bool> PostRoleMenu(RolesModel role, string Menusid, string ID)
        {
            try
            {
                Role check = await _context.Roles.Where(r => r.Name == role.Name).FirstOrDefaultAsync();
                if (check == null && role != null && Menusid != null)
                {

                    Role r = new Role();
                    r.Name = role.Name;
                    r.Accounttype = role.Accounttype;
                    r.Createdby = ID;
                    r.Createddate = DateTime.Now;
                    r.Isdeleted = new System.Collections.BitArray(1);
                    r.Isdeleted[0] = false;
                    _context.Roles.Add(r);
                    _context.SaveChanges();

                    List<int> priceList = Menusid.Split(',').Select(int.Parse).ToList();
                    foreach (var item in priceList)
                    {
                        Rolemenu ar = new Rolemenu();
                        ar.Roleid = r.Roleid;
                        ar.Menuid = item;
                        _context.Rolemenus.Add(ar);

                    }
                    _context.SaveChanges();
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
        #endregion

        #region PutRoleMenu
        public async Task<bool> PutRoleMenu(RolesModel role, string Menusid, string ID)
        {
            try
            {
                Role check = await _context.Roles.Where(r => r.Roleid == role.Roleid).FirstOrDefaultAsync();
                if (check != null && role != null && Menusid != null)
                {
                    check.Name = role.Name;
                    check.Accounttype = role.Accounttype;
                    check.Modifiedby = ID;
                    check.Modifieddate = DateTime.Now;
                    _context.Roles.Update(check);
                    _context.SaveChanges();


                    List<int> regions = await CheckMenuByRole(check.Roleid);

                    List<int> priceList = Menusid.Split(',').Select(int.Parse).ToList();

                    foreach (var item in priceList)
                    {
                        if (regions.Contains(item))
                        {
                            regions.Remove(item);
                        }
                        else
                        {
                            Rolemenu ar = new Rolemenu();
                            ar.Menuid = item;
                            ar.Roleid = check.Roleid;
                            _context.Rolemenus.Update(ar);
                            await _context.SaveChangesAsync();
                            regions.Remove(item);

                        }
                    }
                    if (regions.Count > 0)
                    {
                        foreach (var item in regions)
                        {
                            Rolemenu ar = await _context.Rolemenus.Where(r => r.Roleid == check.Roleid && r.Menuid == item).FirstAsync();
                            _context.Rolemenus.Remove(ar);
                            await _context.SaveChangesAsync();
                        }
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
        #endregion
        #region CheckMenuByRole
        public async Task<List<int>> CheckMenuByRole(int roleid)
        {
            return await _context.Rolemenus
                        .Where(r => r.Roleid == roleid)
                        .Select(r => r.Menuid)
                        .ToListAsync();
        }
        #endregion


        #region GetProfileAll
        public async Task<List<ViewUserAccess>>  GetAllUserDetails()
        {


            List<ViewUserAccess> v = await (
                                         from user in _context.Aspnetusers
                                         join admin in _context.Admins on user.Id equals admin.Aspnetuserid into adminGroup
                                         from admin in adminGroup.DefaultIfEmpty()
                                         join physician in _context.Physicians on user.Id equals physician.Aspnetuserid into physicianGroup
                                         from physician in physicianGroup.DefaultIfEmpty()
                                         where admin != null || physician != null
                                         select new ViewUserAccess
                                         {
                                             UserName = user.Username,
                                             FirstName = admin != null ? admin.Firstname : (physician != null ? physician.Firstname : null),
                                             isAdmin = admin != null ? true : false,
                                             UserID = admin != null ? admin.Adminid : (physician != null ? physician.Physicianid : null),
                                             accounttype = admin != null ? 2 : (physician != null ? 3 : null),
                                             status = admin != null ? admin.Status : (physician != null ? physician.Status : null),
                                             Mobile = admin != null ? admin.Mobile : (physician != null ? physician.Mobile : null),
                                             aspnetuserid=user.Id
                                         }
                                     ).ToListAsync();
            return v;

        }
        #endregion

    }
}

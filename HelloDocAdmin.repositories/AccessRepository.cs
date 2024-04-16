using HelloDocAdmin.Entity.Data;
using HelloDocAdmin.Entity.Models;
using HelloDocAdmin.Entity.ViewModels.AdminSite;
using HelloDocAdmin.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System.Collections;


namespace HelloDocAdmin.Repositories
{
    public class AccessRepository : IAccessRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailConfiguration _email;

        public AccessRepository(ApplicationDbContext context, EmailConfiguration email)
        {
            _context = context;
            _email = email;
        }
        #region GetRoleAccessDetails
        public List<Role> GetRoleAccessDetails()
        {
            BitArray bt = new BitArray(1);
            bt.Set(0, false);
            List<Role> v = _context.Roles.Where(e => e.Isdeleted == bt).ToList();

            return v;

        }
        #endregion

        #region GetRoleByMenus
        public RolesModel GetRoleByMenus(int roleid)
        {
            return _context.Roles
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


        public List<string> getManuByID(int RoleID)
        {

            List<Rolemenu> data = _context.Rolemenus.Where(r => r.Roleid == RoleID).ToList();


            List<string> list = new List<string>();
            foreach (var item in data)
            {
                string str = _context.Menus.FirstOrDefault(e => e.Menuid == item.Menuid).Name;
                list.Add(str);
            }
            return list;
        }

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

                BitArray bt = new BitArray(1);
                bt.Set(0, false);
                Role check = await _context.Roles.Where(r => r.Name == role.Name).FirstOrDefaultAsync();
                if (check == null && role != null && Menusid != null)
                {

                    Role r = new Role();
                    r.Name = role.Name;
                    r.Accounttype = role.Accounttype;
                    r.Createdby = ID;
                    r.Createddate = DateTime.Now;

                    r.Isdeleted = bt;
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

        public bool DeleteAccess(int id)
        {
            BitArray bt = new BitArray(1);
            bt.Set(0, true);
            try
            {
                Role rn = _context.Roles.FirstOrDefault(E => E.Roleid == id);
                rn.Isdeleted = bt;
                _context.Roles.Update(rn);
                _context.SaveChanges();
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #region GetProfileAll
        public async Task<UserAccessData> GetAllUserDetails(int? accounttype, int pagesize = 10, int currentpage = 1)
        {
            BitArray bt = new BitArray(1);
            bt.Set(0, false);
            UserAccessData dm = new UserAccessData();
            IQueryable<ViewUserAccess> data = (
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
                                             status = admin != null ? 0 : (physician != null ? (short?)_context.Requests.Count(e => e.Physicianid == physician.Physicianid) : null),
                                             Mobile = admin != null ? admin.Mobile : (physician != null ? physician.Mobile : null),
                                             aspnetuserid = user.Id
                                         }
                                     );
            if (accounttype != 0)
            {
                data = data.Where(r => r.accounttype == accounttype);
            }

            dm.TotalPage = (int)Math.Ceiling((double)data.Count() / pagesize);
            data = data.Skip((currentpage - 1) * pagesize).Take(pagesize);


            dm.List = data.ToList();
            dm.pageSize = pagesize;
            dm.CurrentPage = currentpage;
            return dm;


        }
        #endregion

    }
}

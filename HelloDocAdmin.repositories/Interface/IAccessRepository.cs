using HelloDocAdmin.Entity.Models;
using HelloDocAdmin.Entity.ViewModels.AdminSite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocAdmin.Repositories.Interface
{
    public interface IAccessRepository
    {
        Task<List<Menu>> GetMenusByAccount(short Accounttype);
        public Task<bool> PostRoleMenu(RolesModel role, string Menusid, string ID);
        public  Task<bool> PutRoleMenu(RolesModel role, string Menusid, string ID);
        public List<Role> GetRoleAccessDetails();
        public Task<List<int>> CheckMenuByRole(int roleid);
        public  RolesModel GetRoleByMenus(int roleid);
        public bool DeleteAccess(int id);

        public Task<List<ViewUserAccess>> GetAllUserDetails();
    }
}

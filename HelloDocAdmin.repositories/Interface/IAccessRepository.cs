using HelloDocAdmin.Entity.Models;
using HelloDocAdmin.Entity.ViewModels.AdminSite;

namespace HelloDocAdmin.Repositories.Interface
{
    public interface IAccessRepository
    {
        Task<List<Menu>> GetMenusByAccount(short Accounttype);
        public Task<bool> PostRoleMenu(RolesModel role, string Menusid, string ID);
        public Task<bool> PutRoleMenu(RolesModel role, string Menusid, string ID);
        public List<Role> GetRoleAccessDetails();
        public Task<List<int>> CheckMenuByRole(int roleid);
        public RolesModel GetRoleByMenus(int roleid);
        public bool DeleteAccess(int id);
        public List<string> getManuByID(int RoleID);

        public Task<UserAccessData> GetAllUserDetails(int? accounttype, int pagesize = 5, int currentpage = 1);
    }
}

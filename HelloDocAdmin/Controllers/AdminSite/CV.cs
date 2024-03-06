using Microsoft.AspNetCore.Mvc;

namespace HelloDocAdmin.Controllers.AdminSite
{
    public class CV : Controller
    {
        private static IHttpContextAccessor _httpContextAccessor;

        static CV()
        {
            _httpContextAccessor = new HttpContextAccessor();
        }

        public static string? LoggedUserName()
        {
            string? UserName = null;
            if (_httpContextAccessor.HttpContext.Session.GetString("UserName") != null)
            {
                UserName = _httpContextAccessor.HttpContext.Session.GetString("UserName").ToString();
            }
            return UserName;
        }

        public static string? LoggedUserID()
        {
            string? UserID = null;

            if (_httpContextAccessor.HttpContext.Session.GetString("UserID") != null)
            {
                UserID = _httpContextAccessor.HttpContext.Session.GetString("UserID").ToString();

            }
            return UserID;
        }
        public static string? LoggedUserRole()
        {
            string? UserRole = null;

            if (_httpContextAccessor.HttpContext.Session.GetString("UserRole") != null)
            {
                UserRole = _httpContextAccessor.HttpContext.Session.GetString("UserRole").ToString();

            }
            return UserRole;
        }
    }
}

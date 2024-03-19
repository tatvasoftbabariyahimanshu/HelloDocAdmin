using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
namespace HelloDocAdmin.Controllers.Authenticate
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
            
             
            string? UserName = _httpContextAccessor.HttpContext.Request.Cookies["Username"];
          
            return UserName;
        }

        public static string? LoggedUserID()
        {
            string? UserID = _httpContextAccessor.HttpContext.Request.Cookies["Userid"];

           
            return UserID;
        }
        public static string? CurrentStatus()
        {
            string? Status = _httpContextAccessor.HttpContext.Request.Cookies["Status"];


            return Status;
        }
        public static string? CurrentStatusText()
        {
            string? StatusText = _httpContextAccessor.HttpContext.Request.Cookies["StatusText"];


            return StatusText;
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

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using HelloDocAdmin.Entity.ViewModels.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Configuration;

namespace HelloDocAdmin.Controllers.Authenticate
{
    public class CV : Controller
    {
        private static IHttpContextAccessor _httpContextAccessor;

        static CV()
        {
            _httpContextAccessor = new HttpContextAccessor();
        }
        public static CookieModel getmodel(string token)
        {
            
             
            JwtSecurityToken jwtSecurityToken = null;
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("Himanshubabariyahimanshubabariyahimanshubabariya");
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero

            }, out SecurityToken validatedToken);

            // Corrected access to the validatedToken
            jwtSecurityToken = (JwtSecurityToken)validatedToken;

            CookieModel cookieModel = new CookieModel()
            {
                AspNetUserID = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == "AspNetUserID").Value,

                role = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role).Value,

                UserName = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value,
            };



            return cookieModel;
        }

        public static string? LoggedUserName()
        {
            string? token = _httpContextAccessor.HttpContext.Request.Cookies["jwt"];
            CookieModel sm = getmodel(token);
           
            return sm.UserName;
        }

        public static string? LoggedUserID()
        {
            string? token = _httpContextAccessor.HttpContext.Request.Cookies["jwt"];
            CookieModel sm = getmodel(token);

            return sm.AspNetUserID;
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
            string? token = _httpContextAccessor.HttpContext.Request.Cookies["jwt"];
            CookieModel sm = getmodel(token);

            return sm.role;
        }
    }
}

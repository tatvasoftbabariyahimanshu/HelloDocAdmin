using HelloDocAdmin.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HelloDocAdmin.Controllers.Authenticate

{
    public class CustomAuthorization : ActionFilterAttribute, IAuthorizationFilter
    {
       
        private readonly    string _role;
        public CustomAuthorization( string role="") {
            this._role=role;
        }
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            var jwtservice = filterContext.HttpContext.RequestServices.GetService<IJWTService>();

            if (jwtservice == null)
            {
                filterContext.Result = new RedirectResult("~/Login");
                return;
            }

            var request = filterContext.HttpContext.Request;
            var toket = request.Cookies["jwt"];

            if (toket == null || !jwtservice.ValidateToken(toket, out JwtSecurityToken jwtSecurityTokenHandler))
            {
                filterContext.Result = new RedirectResult("~/Login");
                return;
            }

            var roles = jwtSecurityTokenHandler.Claims.FirstOrDefault(claiim => claiim.Type == ClaimTypes.Role);

            if (roles == null)
            {
                filterContext.Result = new RedirectResult("~/Login");
                return;
            }

            if (string.IsNullOrWhiteSpace(_role) || roles.Value != _role)
            {
                filterContext.Result = new RedirectResult("~/Login/AccessDenide");

            }
        }

    
    }
}

using HelloDocAdmin.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ServiceStack;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HelloDocAdmin.Controllers.Authenticate

{
    public class CustomAuthorization : ActionFilterAttribute, IAuthorizationFilter
    {

        private readonly List<string> _role;
        private readonly string _manu;
        public CustomAuthorization(string role = "", string? manu = "")
        {
            _role = role.Split(',').ToList();
            _manu = manu;
        }
        public async void OnAuthorization(AuthorizationFilterContext filterContext)
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
            List<string> str = null;
            if (CV.LoggedUserRole() != "Patient")
            {

                int RoleID = jwtSecurityTokenHandler.Claims.FirstOrDefault(claiim => claiim.Type == "RoleID").Value.ToInt();
                str = new List<string>();
                var Accessrepo = filterContext.HttpContext.RequestServices.GetService<IAccessRepository>();
                str = Accessrepo.getManuByID(RoleID);
            }
            if (roles == null)
            {
                filterContext.Result = new RedirectResult("~/Login");
                return;
            }
            bool flage = false;
            foreach (var role in _role)
            {

                if (string.IsNullOrWhiteSpace(role) || roles.Value != role)
                {

                    flage = false;
                }
                else
                {
                    flage = true;
                    break;
                }
            }





            if (CV.LoggedUserRole() != "Patient")
            {
                if (flage == false || !str.Contains(_manu))
                {
                    filterContext.Result = new RedirectResult("~/Login/AccessDenide");

                }
            }
            //if (flage == false)
            //{
            //    filterContext.Result = new RedirectResult("~/Login/AccessDenide");

            //}
        }


    }
}

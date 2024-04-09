﻿using HelloDocAdmin.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HelloDocAdmin.Controllers.Authenticate

{
    public class CustomAuthorization : ActionFilterAttribute, IAuthorizationFilter
    {

        private readonly List<string> _role;
        public CustomAuthorization(string role = "")
        {
            _role = role.Split(',').ToList();
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

            //if (string.IsNullOrWhiteSpace(_role) || roles.Value != _role)
            if (flage == false)
            {
                filterContext.Result = new RedirectResult("~/Login/AccessDenide");

            }
        }


    }
}

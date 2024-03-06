using HelloDocAdmin.Entity.ViewModels.Authentication;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocAdmin.Repositories.Interface
{
    public interface IJWTService
    {
        string GenerateJWTAuthetication(UserInfo userinfo);
        bool ValidateToken(string token, out JwtSecurityToken jwtSecurityTokenHandler);
    }
}

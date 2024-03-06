using HelloDocAdmin.Entity.Data;
using HelloDocAdmin.Entity.Models;
using HelloDocAdmin.Entity.ViewModels.Authentication;
using HelloDocAdmin.Repositories.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocAdmin.Repositories
{
    public class LoginRepository:ILoginRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailConfiguration _email;

        public LoginRepository(ApplicationDbContext context, EmailConfiguration email)
        {
            _context = context;
            _email = email;
        }

       
        public async Task<UserInfo> CheckAccessLogin(LoginViewModel vm)
        {
            var user = await _context.Aspnetusers.FirstOrDefaultAsync(u => u.Email == vm.Email);
            var admindata= _context.Admins.FirstOrDefault(u=> u.Aspnetuserid == user.Id); 
            UserInfo admin = new UserInfo();
            if (user != null)
            {
                var hasher = new PasswordHasher<string>();
                PasswordVerificationResult result = hasher.VerifyHashedPassword(null, user.Passwordhash, vm.Password);
                if (result != PasswordVerificationResult.Success)
                {

                    return admin;
                }
                else
                {
                    var data =  _context.Aspnetuserroles.FirstOrDefault(E => E.Userid == user.Id);
                    var datarole=_context.Aspnetroles.FirstOrDefault(e=>e.Id == data.Roleid);

                    
                                       admin.Username = user.Username;
                    admin.FirstName = admin.FirstName ?? string.Empty;
                                       admin.LastName = admin.LastName ?? string.Empty;
                                       admin.Role = datarole.Name;
                    admin.UserId = admindata.Adminid;
                                  
                    return admin;
                }
            }
            else
            {
                return admin;
            }
        }
       
    }
}

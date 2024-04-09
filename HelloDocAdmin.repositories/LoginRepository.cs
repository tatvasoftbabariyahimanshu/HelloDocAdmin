﻿using HelloDocAdmin.Entity.Data;
using HelloDocAdmin.Entity.Models;
using HelloDocAdmin.Entity.ViewModels.Authentication;
using HelloDocAdmin.Repositories.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HelloDocAdmin.Repositories
{
    public class LoginRepository : ILoginRepository
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
            try
            {
                var user = _context.Aspnetusers.FirstOrDefault(u => u.Email == vm.Email);

                UserInfo admin = new UserInfo();
                if (user != null)
                {
                    var hasher = new PasswordHasher<string>();
                    PasswordVerificationResult result = hasher.VerifyHashedPassword(null, user.Passwordhash, vm.Password);
                    if (result != PasswordVerificationResult.Success)
                    {
                        admin = null;
                        return admin;
                    }
                    else
                    {
                        var data = _context.Aspnetuserroles.FirstOrDefault(E => E.Userid == user.Id);
                        var datarole = _context.Aspnetroles.FirstOrDefault(e => e.Id == data.Roleid);


                        admin.Username = user.Username;
                        admin.FirstName = admin.FirstName ?? string.Empty;
                        admin.LastName = admin.LastName ?? string.Empty;
                        admin.Role = datarole.Name;
                        admin.Email = user.Email;

                        admin.AspUserID = user.Id;


                        if (admin.Role == "Admin")
                        {
                            var admindata = _context.Admins.FirstOrDefault(u => u.Aspnetuserid == user.Id);
                            admin.UserId = admindata.Adminid;
                            admin.RoleID = (int)admindata.Roleid;
                        }
                        else if (admin.Role == "Patient")
                        {
                            var admindata2 = _context.Users.FirstOrDefault(u => u.Aspnetuserid == user.Id);



                        }
                        else
                        {
                            var admindata = _context.Physicians.FirstOrDefault(u => u.Aspnetuserid == user.Id);

                            admin.RoleID = (int)admindata.Roleid;

                        }

                        return admin;
                    }
                }
                else
                {
                    return admin = null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool sendmailforresetpass(string Email)
        {
            try
            {

                ENC encyptdecypt = new ENC();

                string encyptemail = encyptdecypt.EnryptString(Email);
                string encyptdatetime = encyptdecypt.EncryptDate(DateTime.Now);

                string resetLink = $"https://localhost:44341/Login/ChangePassword?email={encyptemail}&datetime={encyptdatetime}";
                string emailContent = $@"
                    <html>
                    <body>
                     <p>We received a request to reset your password.</p>
                    <p>To reset your password, click the following link:</p>
                    <p><a href=""{resetLink}"">Reset Password</a></p>
                    <p>If you didn't request a password reset, you can ignore this email.</p>
                    </body>
                    </html>";


                if (_email.SendMail(Email, "New Patient Account Creation", emailContent))
                {
                    Emaillog el = new Emaillog();
                    el.Action = 5;

                    el.Sentdate = DateTime.Now;
                    el.Createdate = DateTime
                         .Now;
                    el.Emailtemplate = "first";
                    el.Senttries = 1;
                    el.Subjectname = "New Patient Account Creation";

                    el.Roleid = 4;
                    el.Emailid = Email;
                    _context.Emaillogs.Add(el);
                    _context.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }

        }
        public bool islinkexist(string pwdModified)
        {
            var data = _context.Aspnetusers.FirstOrDefault(e => e.pwdModified == pwdModified);
            if (data == null)
            {
                return false;
            }
            else
            {
                return true;
            }

        }
        public bool savepass(ChangePassModel cpm)
        {
            if (cpm == null)
            {
                return false;
            }
            else
            {
                var hasher = new PasswordHasher<string>();

                string hashedPassword = hasher.HashPassword(null, cpm.Password);
                var aspnetuser = _context.Aspnetusers.FirstOrDefault(m => m.Email == cpm.Email);
                if (aspnetuser != null)
                {
                    aspnetuser.Passwordhash = hashedPassword;
                    aspnetuser.Modifieddate = DateTime.Now;
                    aspnetuser.pwdModified = cpm.pwdModified;
                    _context.Aspnetusers.Update(aspnetuser);
                    _context.SaveChanges();
                    return true;
                }
                else
                {

                    return false;
                }
            }
        }
        public bool saveuser(NewRegistration cpm)
        {
            try
            {

                var hasher = new PasswordHasher<string>();


                string hashedPassword = hasher.HashPassword(null, cpm.Password);
                var U = _context.Requestclients.FirstOrDefault(m => m.Email == cpm.Email);
                Guid id = Guid.NewGuid();
                Aspnetuser aspnetuser = new Aspnetuser
                {
                    Id = id.ToString(),
                    Email = cpm.Email,
                    Passwordhash = hashedPassword,
                    Username = cpm.Email,
                    CreatedDate = DateTime.Now,
                    Phonenumber = U.Phonenumber,
                    Modifieddate = DateTime.Now
                };
                _context.Aspnetusers.Add(aspnetuser);
                _context.SaveChanges();

                Aspnetuserrole asp = new Aspnetuserrole
                {
                    Roleid = "2",
                    Userid = aspnetuser.Id

                };
                _context.Aspnetuserroles.Add(asp);
                _context.SaveChanges();

                var User = new User
                {
                    Aspnetuserid = aspnetuser.Id,
                    Firstname = U.Firstname,
                    Intdate = U.Intdate,
                    Lastname = U.Lastname,
                    Intyear = U.Intyear,
                    Strmonth = U.Strmonth,

                    Email = cpm.Email,
                    Createdby = aspnetuser.Id,
                    Createddate = DateTime.Now,

                };
                _context.Users.Add(User);
                _context.SaveChanges();
                List<Requestclient> rc = new List<Requestclient>();
                rc = _context.Requestclients.Where(e => e.Email == cpm.Email).ToList();

                foreach (var r in rc)
                {
                    Request req = _context.Requests.Find(r.Requestid);
                    if (req != null)
                    {
                        req.Userid = User.Userid;
                        _context.Requests.Update(req);
                        _context.SaveChangesAsync();
                    }

                }
                return true;
            }
            catch
            {
                return false;
            }
        }



        public bool isAccessGranted(int roleId, string menuName)
        {
            // Get the list of menu IDs associated with the role
            IQueryable<int> menuIds = _context.Rolemenus
                                            .Where(e => e.Roleid == roleId)
                                            .Select(e => e.Menuid);

            // Check if any menu with the given name exists in the list of menu IDs
            bool accessGranted = _context.Menus
                                         .Any(e => menuIds.Contains(e.Menuid) && e.Name == menuName);

            return accessGranted;
        }




    }
}

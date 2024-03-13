using HelloDocAdmin.Entity.Models;
using HelloDocAdmin.Entity.ViewModels.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocAdmin.Repositories.Interface
{
    public interface ILoginRepository
    {
        Task<UserInfo> CheckAccessLogin(LoginViewModel vm);
        public bool sendmailforresetpass(string Email);

        public bool savepass(ChangePassModel cpm);
        public bool saveuser(NewRegistration cpm);
        public bool islinkexist(string pwdModified);
    }
}

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
    }
}

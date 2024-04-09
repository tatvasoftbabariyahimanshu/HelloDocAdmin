using HelloDocAdmin.Entity.ViewModels.Authentication;

namespace HelloDocAdmin.Repositories.Interface
{
    public interface ILoginRepository
    {
        Task<UserInfo> CheckAccessLogin(LoginViewModel vm);
        public bool sendmailforresetpass(string Email);

        public bool savepass(ChangePassModel cpm);
        public bool saveuser(NewRegistration cpm);
        public bool islinkexist(string pwdModified);

        public bool isAccessGranted(int RoleID, string manuname);
    }
}

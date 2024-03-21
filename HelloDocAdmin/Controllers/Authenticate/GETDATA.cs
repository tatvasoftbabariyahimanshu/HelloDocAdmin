using HelloDocAdmin.Entity.ViewModels.Authentication;
using HelloDocAdmin.Repositories.Interface;

namespace HelloDocAdmin.Controllers.Authenticate
{
    public class GETDATA
    {
        public IJWTService _JWTService;
        public GETDATA(IJWTService jWTService) {
            _JWTService = jWTService;


        }



       public string getusername(string token)
        {
            CookieModel cm=_JWTService.getDetails(token);
            return cm.UserName;
        }
    }
}

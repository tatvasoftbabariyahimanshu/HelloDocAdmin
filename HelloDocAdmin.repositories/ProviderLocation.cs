using HelloDocAdmin.Entity.Data;
using HelloDocAdmin.Entity.ViewModels.AdminSite;
using HelloDocAdmin.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Xsl;

namespace HelloDocAdmin.Repositories
{
    public class ProviderLocation:IProviderLocation
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailConfiguration _email;

        public ProviderLocation(ApplicationDbContext context, EmailConfiguration email)
        {
            _context = context;
            _email = email;
        }

        public List<AllProviderLocation> GetAllProviderAddress()
        {
           var data=_context.Physicianlocations.ToList();

            List<AllProviderLocation> lst=new List<AllProviderLocation>();

            foreach (var item in data)
            {
                lst.Add(new AllProviderLocation
                {
                    Name=item.Physicianname,
                    Latitude=item.Latitude,
                    Longitude=item.Longitude,
                    Address=item.Address

                });
            }
            return lst;
        }
    }
}

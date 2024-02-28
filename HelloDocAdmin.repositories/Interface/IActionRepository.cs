using HelloDocAdmin.Entity.Models;
using HelloDocAdmin.Entity.ViewModels.AdminSite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocAdmin.Repositories.Interface
{
    public interface IActionRepository
    {
        public bool DeleteDoc(int RequestWiseFileID);
        public  Task<bool> TransferProvider(int RequestId, int ProviderId, string notes);
        public Healthprofessional SelectProfessionlByID(int VendorID);
        public bool SendOrder(ViewSendOrderModel data);
        public bool ClearCase(int RequestID);
    }
}

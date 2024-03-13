using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocAdmin.Entity.ViewModels.AdminSite
{
    public class Constant
    {
        public enum RequestType
        {
            Business = 4,
            Patient=1,
            Family=2,
            Concierge=3
        }
        public enum AdminDashStatus
        {
            New = 1,
            Pending,
            Active,
            Conclude,
            ToClose,
            UnPaid
        }
        public enum Status
        {
            Unassigne = 1, Accepted, Cancelled, MDEnRoute, MDONSite, Conclude, CancelledByPatients, Closed, Unpaid, Clear,
            Block

        }
        public enum AdminStatus
        {
            Pending = 1, Active, NotActive

        }
    }
}

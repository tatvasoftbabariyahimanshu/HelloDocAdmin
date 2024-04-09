namespace HelloDocAdmin.Entity.ViewModels.AdminSite
{
    public class Constant
    {
        public enum RequestType
        {
            Business = 4,
            Patient = 1,
            Family = 2,
            Concierge = 3
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
        public enum AccountType
        {
            All = 1, Admin, Physician,
            Patient
        }
        public enum Action1
        {
            Sendorder = 1, Request, SendLink, SendAgreement, Forgot, NewRegistration, contact

        }




    }
}

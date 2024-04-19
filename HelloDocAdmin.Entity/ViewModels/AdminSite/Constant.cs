﻿namespace HelloDocAdmin.Entity.ViewModels.AdminSite
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
            Pending = 2, Active = 1, NotActive = 3

        }
        public enum AccountType
        {
            Admin = 2, Physician,

        }
        public enum Action1
        {
            Sendorder = 1, Request, SendLink, SendAgreement, Forgot, NewRegistration, contact

        }




    }
}

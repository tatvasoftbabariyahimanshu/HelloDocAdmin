namespace HelloDocAdmin.Entity.ViewModels
{
    public class DashboardRequestModel
    {
        public string PatientName { get; set; }
        public DateOnly? Dob { get; set; }
        public string PatientId { get; set; }
        public string Requestor { get; set; }
        public DateTime RequestedDate { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string? RequestorPhoneNumber { get; set; }
        public int? RegionID { get; set; }
        public int? RequestID { get; set; }
        public int? RequestTypeID { get; set; }
        public string? Address { get; set; }
        public List<string>? Notes { get; set; }
        public short Status { get; set; }
        public int? ProviderID { get; set; }
        public string? ProviderName { get; set; }
        public string? Region { get; set; }
    }
}

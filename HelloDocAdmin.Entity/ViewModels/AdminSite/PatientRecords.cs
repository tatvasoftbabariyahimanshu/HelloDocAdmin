namespace HelloDocAdmin.Entity.ViewModels.AdminSite
{
    public class PatientRecords
    {
        public int? RequestID { get; set; }
        public int? RequestClientsID { get; set; }
        public string? ClientName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ConfirmationNumber { get; set; }
        public string ProviderName { get; set; }
        public DateTime? ConcludeDate { get; set; }
        public int StatusID { get; set; }
        public DateTime? Modifieddate { get; set; }


        public bool? isfinal { get; set; }


    }
}

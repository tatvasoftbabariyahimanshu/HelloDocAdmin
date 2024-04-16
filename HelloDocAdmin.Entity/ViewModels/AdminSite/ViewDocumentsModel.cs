namespace HelloDocAdmin.Entity.ViewModels.AdminSite
{
    public class ViewDocumentsModel
    {
        public List<Documents>? documentslist { get; set; } = null;
        public string? Firstanme { get; set; }
        public string? Lastanme { get; set; }
        public string? ConfirmationNumber { get; set; }
        public int? RequestID { get; set; }
        public int? RequestWiseFileID { get; set; }


    }
}

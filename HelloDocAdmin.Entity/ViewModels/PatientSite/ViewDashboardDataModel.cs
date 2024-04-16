using HelloDocAdmin.Entity.Models;
using System.Collections;

namespace HelloDocAdmin.Entity.ViewModels.PatientSite
{
    public class ViewDashboardDataModel
    {
        public Dictionary<int, int> ids = new Dictionary<int, int>();

        public List<Request> requests = new List<Request>();
        public int TotalPage { get; set; }
        public int UserID { get; set; }
        public int CurrentPage { get; set; }

        public int pageSize { get; set; }
        public int Userid { get; set; }

        public string? Aspnetuserid { get; set; }

        public string Firstname { get; set; } = null!;

        public string? Lastname { get; set; }

        public DateTime BirthDate { get; set; }

        public string Email { get; set; } = null!;

        public string? Mobile { get; set; }

        public BitArray? Ismobile { get; set; }

        public string? Street { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }

        public int? Regionid { get; set; }

        public string? Zipcode { get; set; }

        public string Strmonth { get; set; }

        public int Intyear { get; set; }

        public int Intdate { get; set; }

        public string Createdby { get; set; } = null!;

        public DateTime Createddate { get; set; }

        public string? Modifiedby { get; set; }

        public DateTime? Modifieddate { get; set; }

        public short? Status { get; set; }

        public BitArray? Isdeleted { get; set; }

        public string? Ip { get; set; }

        public BitArray? Isrequestwithemail { get; set; }

        public virtual Aspnetuser? Aspnetuser { get; set; }

        public virtual ICollection<Request> Requests { get; } = new List<Request>();
    }
}

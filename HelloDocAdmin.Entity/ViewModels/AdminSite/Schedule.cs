namespace HelloDocAdmin.Entity.ViewModels.AdminSite
{
    public class Schedule
    {
        public int Shiftid { get; set; }

        public int Physicianid { get; set; }
        public string? PhysicianName { get; set; }
        public int Regionid { get; set; }


        public int ShiftDetailsID { get; set; }

        public DateOnly Startdate { get; set; }
        public DateTime? ShiftDate { get; set; }
        public TimeOnly Starttime { get; set; }
        public TimeOnly Endtime { get; set; }

        public bool Isrepeat { get; set; }

        public string? checkWeekday { get; set; }

        public int? Repeatupto { get; set; }
        public short Status { get; set; }
        public List<Schedule> DayList { get; set; }


    }
}
namespace HelloDocAdmin.Entity.ViewModels.AdminSite
{
    public class AllProviderLocation
    {

        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string Name { get; set; }

        public string ImgPath { get; set; }
        public string Address { get; set; }

        public DateTime Created { get; set; }

        public string PhoneNumber { get; set; }

        public int PhysicianID { get; set; }
    }
}

﻿namespace HelloDocAdmin.Entity.ViewModels.AdminSite
{
    public class RequestedShift
    {
        public List<ShiftDetailData> List { get; set; }

        public int TotalPage { get; set; }

        public int CurrentPage { get; set; }

        public int pageSize { get; set; }
    }
}

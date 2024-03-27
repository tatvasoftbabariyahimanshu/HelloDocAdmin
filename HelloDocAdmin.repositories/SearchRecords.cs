﻿using HelloDocAdmin.Entity.Data;
using HelloDocAdmin.Entity.ViewModels;
using HelloDocAdmin.Entity.ViewModels.AdminSite;
using HelloDocAdmin.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocAdmin.Repositories
{

    public class SearchRecords:ISearchRecords
    {

        private readonly ApplicationDbContext _context;
        private readonly EmailConfiguration _email;

        public SearchRecords(ApplicationDbContext context, EmailConfiguration email)
        {
            _context = context;
            _email = email;
        }
        public async Task<RequestRecords> GetRequestsbyfilterForRecords(short status, string patientname, int requesttype, DateTime startdate, DateTime enddate, string physicianname, string email, string phonenumber, int currentpage = 1, int pagesize = 5)
        {
            RequestRecords dm = new RequestRecords();



            IQueryable<SearchRecordView> allData = (from req in _context.Requests
                                                    join reqClient in _context.Requestclients
                                                    on req.Requestid equals reqClient.Requestid into reqClientGroup
                                                    from rc in reqClientGroup.DefaultIfEmpty()
                                                    join phys in _context.Physicians
                                                    on req.Physicianid equals phys.Physicianid into physGroup
                                                    from p in physGroup.DefaultIfEmpty()
                                                    join reg in _context.Regions
                                                    on rc.Regionid equals reg.Regionid into RegGroup
                                                    from rg in RegGroup.DefaultIfEmpty()
                                                    join nts in _context.Requestnotes
                                                    on req.Requestid equals nts.Requestid into ntsgrp
                                                    from nt in ntsgrp.DefaultIfEmpty()
                                                   

                                                    orderby req.Createddate descending
                                                    select new SearchRecordView
                                                    {
                                                       
                                                        PatientName = req.Firstname + " " + req.Lastname,
                                                        RequestID = req.Requestid,
                                                        DateOfService = req.Createddate,
                                                        PhoneNumber = rc.Phonenumber  ?? "",
                                                         Email=rc.Email ?? "",
                                                         Address=rc.Address +","+rc.City+" "+rc.Zipcode,
                                                         RequestTypeID=req.Requesttypeid,
                                                         Status=req.Status,
                                                         PhysicianName= p.Firstname+" "+p.Lastname ?? "",
                                                         AdminNote=nt!=null? nt.Adminnotes ?? "":"",
                                                         PhysicianNote= nt != null ? nt.Physiciannotes ?? "" : "",
                                                        PatientNote=rc.Notes ?? ""
                                                    }); 


            if (status != 0)
            {
                allData = allData.Where(r => r.Status == status);
            }
            if (requesttype != 0)
            {
                allData = allData.Where(r => r.RequestTypeID == requesttype);
            }
            if (startdate != default(DateTime))
            {
                allData = allData.Where(r => r.DateOfService.Date >= startdate.Date);
            }
            if (enddate != default(DateTime))
            {
                allData = allData.Where(r => r.DateOfService.Date <= enddate.Date);
            }
            if (!patientname.IsNullOrEmpty())

            {
                allData = allData.Where(r => r.PatientName.ToLower().Contains(patientname.ToLower()));
            }
            if (!physicianname.IsNullOrEmpty())

            {
                allData = allData.Where(r => r.PhysicianName.ToLower().Contains(physicianname.ToLower()));
            }
            if (!email.IsNullOrEmpty())

            {
                allData = allData.Where(r => r.Email.ToLower().Contains(email.ToLower()));
            }
            if (!phonenumber.IsNullOrEmpty())

            {
                allData = allData.Where(r => r.PhoneNumber.ToLower().Contains(phonenumber.ToLower()));
            }
            dm.TotalPage = (int)Math.Ceiling((double)allData.Count() / pagesize);
            allData = allData.OrderByDescending(x => x.DateOfService).Skip((currentpage - 1) * pagesize).Take(pagesize);
            dm.requestList = allData.ToList();
            dm.pageSize = pagesize;
            dm.CurrentPage = currentpage;

            return dm;
        }
    }
}
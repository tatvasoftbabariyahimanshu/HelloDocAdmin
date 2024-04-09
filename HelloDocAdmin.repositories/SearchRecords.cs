using HelloDocAdmin.Entity.Data;
using HelloDocAdmin.Entity.Models;
using HelloDocAdmin.Entity.ViewModels;
using HelloDocAdmin.Entity.ViewModels.AdminSite;
using HelloDocAdmin.Repositories.Interface;
using Microsoft.IdentityModel.Tokens;
using System.Collections;

namespace HelloDocAdmin.Repositories
{

    public class SearchRecords : ISearchRecords
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

            BitArray bt = new BitArray(1);
            bt.Set(0, false);

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
                                                    where req.Isdeleted == bt

                                                    orderby req.Createddate descending
                                                    select new SearchRecordView
                                                    {
                                                        Modifieddate = req.Modifieddate,
                                                        PatientName = req.Firstname + " " + req.Lastname,
                                                        RequestID = req.Requestid,
                                                        DateOfService = req.Createddate,
                                                        PhoneNumber = rc.Phonenumber ?? "-",
                                                        Email = rc.Email ?? "-",
                                                        Address = rc.Address + "," + rc.City + " " + rc.Zipcode,
                                                        RequestTypeID = req.Requesttypeid,
                                                        Status = req.Status,
                                                        PhysicianName = p.Firstname + "-" + p.Lastname ?? "-",
                                                        AdminNote = nt != null ? nt.Adminnotes ?? "-" : "-",
                                                        PhysicianNote = nt != null ? nt.Physiciannotes ?? "-" : "-",
                                                        PatientNote = rc.Notes ?? "-",
                                                        Zip = rc.Zipcode
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


            for (int i = 0; i < dm.requestList.Count; i++)
            {
                if (dm.requestList[i].Status == 9)
                {
                    dm.requestList[i].CloseCaseDate = dm.requestList[i].Modifieddate;
                }
                else
                {
                    dm.requestList[i].CloseCaseDate = null;
                }
                if (dm.requestList[i].Status == 3 && dm.requestList[i].PhysicianName != null)
                {
                    var data = _context.Requeststatuslogs.FirstOrDefault(x => (x.Status == 3) && (x.Requestid == dm.requestList[i].RequestID));
                    dm.requestList[i].CancelByProviderNote = data.Notes;
                }

            }
            dm.pageSize = pagesize;
            dm.CurrentPage = currentpage;

            return dm;
        }



        public async Task<PatientHistory> Patienthistorybyfilter(string firstname, string lastname, string email, string phonenumber, int currentpage = 1, int pagesize = 5)
        {
            PatientHistory dm = new PatientHistory();
            IQueryable<User> allData = (from user in _context.Users
                                        select new User
                                        {

                                            Userid = user.Userid,
                                            Firstname = user.Firstname,
                                            Lastname = user.Lastname,
                                            Email = user.Email,
                                            Mobile = user.Mobile,
                                            Street = user.Street,
                                            City = user.City,
                                            State = user.State,
                                            Zipcode = user.Zipcode


                                        }


                                       );


            if (!email.IsNullOrEmpty())

            {
                allData = allData.Where(r => r.Email.ToLower().Contains(email.ToLower()));
            }
            if (!phonenumber.IsNullOrEmpty())

            {
                allData = allData.Where(r => r.Mobile.ToLower().Contains(phonenumber.ToLower()));
            }
            if (!firstname.IsNullOrEmpty())

            {
                allData = allData.Where(r => r.Firstname.ToLower().Contains(firstname.ToLower()));
            }
            if (!lastname.IsNullOrEmpty())

            {
                allData = allData.Where(r => r.Lastname.ToLower().Contains(lastname.ToLower()));
            }

            dm.TotalPage = (int)Math.Ceiling((double)allData.Count() / pagesize);
            allData = allData.Skip((currentpage - 1) * pagesize).Take(pagesize);


            dm.userList = allData.ToList();
            dm.pageSize = pagesize;
            dm.CurrentPage = currentpage;

            return dm;

        }

        public async Task<PatientRecordsView> PatientRecordsViewBy(int? UserID, int currentpage = 1, int pagesize = 5)
        {
            PatientRecordsView dm = new PatientRecordsView();

            IQueryable<PatientRecords> data = (from req in _context.Requests
                                               join cl in _context.Requestclients
                                                 on req.Requestid equals cl.Requestid into reqClientGroup
                                               from rc in reqClientGroup.DefaultIfEmpty()
                                               join phy in _context.Physicians
                                               on req.Physicianid equals phy.Physicianid into phyGroup
                                               from phys in phyGroup.DefaultIfEmpty()
                                               where req.Userid == UserID
                                               select new PatientRecords
                                               {
                                                   RequestID = rc.Requestid,
                                                   ClientName = rc.Firstname + " " + rc.Lastname,
                                                   ConfirmationNumber = req.Confirmationnumber ?? "-",
                                                   CreatedDate = req.Createddate,
                                                   ProviderName = phys.Firstname ?? "-" + " " + phys.Lastname ?? "-",
                                                   StatusID = req.Status,
                                                   Modifieddate = req.Modifieddate,
                                               }
                                             );

            dm.TotalPage = (int)Math.Ceiling((double)data.Count() / pagesize);
            data = data.Skip((currentpage - 1) * pagesize).Take(pagesize);
            dm.List = data.ToList();
            for (int i = 0; i < dm.List.Count; i++)
            {
                if (dm.List[i].StatusID == 6)
                {
                    dm.List[i].ConcludeDate = dm.List[i].Modifieddate;
                }
                else
                {
                    dm.List[i].ConcludeDate = null;
                }
            }
            dm.UserID = (int)UserID;
            dm.pageSize = pagesize;
            dm.CurrentPage = currentpage;

            return dm;
        }

        public async Task<EmailRecords> EmailLogs(int accounttype, string email, string ReciverName, DateTime CreatedDate, DateTime SendDate, int pagesize = 5, int currentpage = 1)
        {
            EmailRecords dm = new EmailRecords();
            IQueryable<Emaillogdata> data = (from req in _context.Emaillogs


                                             select new Emaillogdata
                                             {
                                                 Recipient = _context.Aspnetusers.FirstOrDefault(e => e.Email == req.Emailid).Username ?? null,
                                                 Confirmationnumber = req.Confirmationnumber ?? "-",
                                                 Createdate = req.Createdate,
                                                 Emailtemplate = req.Emailtemplate,
                                                 Filepath = req.Filepath,
                                                 Sentdate = req.Sentdate,
                                                 Roleid = req.Roleid,
                                                 Emailid = req.Emailid,
                                                 Senttries = req.Senttries,
                                                 Subjectname = req.Subjectname,
                                                 Action = req.Action
                                             }
                                          );
            if (accounttype != 0)
            {
                data = data.Where(r => r.Roleid == accounttype);
            }
            if (CreatedDate != default(DateTime))
            {
                data = data.Where(r => r.Createdate.Date == CreatedDate.Date);
            }
            if (SendDate != default(DateTime))
            {
                data = data.Where(r => r.Sentdate.Date == SendDate.Date);
            }
            if (!ReciverName.IsNullOrEmpty())

            {
                data = data.Where(r => r.Recipient.ToLower().Contains(ReciverName.ToLower()));
            }
            if (!email.IsNullOrEmpty())

            {
                data = data.Where(r => r.Emailid.ToLower().Contains(email.ToLower()));
            }

            dm.TotalPage = (int)Math.Ceiling((double)data.Count() / pagesize);
            data = data.Skip((currentpage - 1) * pagesize).Take(pagesize);
            dm.List = data.ToList();

            dm.pageSize = pagesize;
            dm.CurrentPage = currentpage;
            return dm;
        }
        public async Task<SMSLogs> SMSLogs(int accounttype, string phonenumber, string ReciverName, DateTime CreatedDate, DateTime SendDate, int pagesize = 5, int currentpage = 1)
        {
            SMSLogs dm = new SMSLogs();
            IQueryable<SMSLogsData> data = (from req in _context.Smslogs


                                            select new SMSLogsData
                                            {
                                                Recipient = _context.Aspnetusers.FirstOrDefault(e => e.Phonenumber == req.Mobilenumber).Username ?? null,
                                                Confirmationnumber = req.Confirmationnumber ?? "-",
                                                Createdate = req.Createdate,
                                                Smstemplate = req.Smstemplate,
                                                Sentdate = (DateTime)req.Sentdate,
                                                Roleid = req.Roleid,
                                                Mobilenumber = req.Mobilenumber,
                                                Senttries = req.Senttries,
                                                Action = req.Action
                                            }
                                          );
            if (accounttype != 0)
            {
                data = data.Where(r => r.Roleid == accounttype);
            }
            if (CreatedDate != default(DateTime))
            {
                data = data.Where(r => r.Createdate.Date == CreatedDate.Date);
            }
            if (SendDate != default(DateTime))
            {
                data = data.Where(r => r.Sentdate.Date == SendDate.Date);
            }
            if (!ReciverName.IsNullOrEmpty())

            {
                data = data.Where(r => r.Recipient.ToLower().Contains(ReciverName.ToLower()));
            }
            if (!phonenumber.IsNullOrEmpty())

            {
                data = data.Where(r => r.Mobilenumber.ToLower().Contains(phonenumber.ToLower()));
            }

            dm.TotalPage = (int)Math.Ceiling((double)data.Count() / pagesize);
            data = data.Skip((currentpage - 1) * pagesize).Take(pagesize);
            dm.List = data.ToList();

            dm.pageSize = pagesize;
            dm.CurrentPage = currentpage;
            return dm;
        }
        public async Task<BlockRequest> BlockHistory(string name, string email, string phonenumber, DateTime CreatedDate, int pagesize = 5, int currentpage = 1)
        {
            BlockRequest dm = new BlockRequest();
            IQueryable<BlockRequestData> data = (from req in _context.Blockrequests


                                                 select new BlockRequestData
                                                 {
                                                     PatientName = _context.Requests.FirstOrDefault(e => e.Requestid == req.Requestid).Firstname,
                                                     Email = req.Email,
                                                     Createddate = (DateTime)req.Createddate,
                                                     Isactive = req.Isactive,
                                                     Requestid = req.Requestid,
                                                     Phonenumber = req.Phonenumber,
                                                     Reason = req.Reason,
                                                 }
                                         );

            if (CreatedDate != default(DateTime))
            {
                data = data.Where(r => r.Createddate.Date == CreatedDate.Date);
            }

            if (!name.IsNullOrEmpty())

            {
                data = data.Where(r => r.PatientName.ToLower().Contains(name.ToLower()));
            }
            if (!email.IsNullOrEmpty())

            {
                data = data.Where(r => r.Email.ToLower().Contains(email.ToLower()));
            }
            if (!phonenumber.IsNullOrEmpty())

            {
                data = data.Where(r => r.Phonenumber.ToLower().Contains(phonenumber.ToLower()));
            }

            dm.TotalPage = (int)Math.Ceiling((double)data.Count() / pagesize);
            data = data.Skip((currentpage - 1) * pagesize).Take(pagesize);
            dm.List = data.ToList();

            dm.pageSize = pagesize;
            dm.CurrentPage = currentpage;
            return dm;
        }
        public async Task<bool> UnBlock(int RequestID, string id)
        {
            try
            {
                Blockrequest br = _context.Blockrequests.FirstOrDefault(e => e.Requestid == RequestID);
                _context.Blockrequests.Remove(br);
                _context.SaveChanges();


                Request re = _context.Requests.FirstOrDefault(e => e.Requestid == RequestID);
                re.Status = 1;

                re.Modifieddate = DateTime.Now;
                _context.Requests.Update(re);
                _context.SaveChanges();
                var admindata = _context.Admins.FirstOrDefault(e => e.Aspnetuserid == id);
                Requeststatuslog rs = new Requeststatuslog();
                rs.Status = 1;
                rs.Requestid = RequestID;
                rs.Adminid = admindata.Adminid;
                rs.Createddate = DateTime.Now;

                _context.Requeststatuslogs.Add(rs);
                _context.SaveChanges();






                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }



        public bool Delete(int RequestID, string id)
        {
            try
            {
                BitArray bt = new BitArray(1);
                bt.Set(0, true);
                Request re = _context.Requests.FirstOrDefault(e => e.Requestid == RequestID);
                re.Isdeleted = bt;

                re.Modifieddate = DateTime.Now;

                _context.Requests.Update(re);
                _context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}

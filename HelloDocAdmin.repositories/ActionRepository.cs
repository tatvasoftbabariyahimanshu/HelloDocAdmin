using HelloDocAdmin.Entity.Data;
using HelloDocAdmin.Entity.Models;
using HelloDocAdmin.Entity.ViewModels.AdminSite;
using HelloDocAdmin.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocAdmin.Repositories
{
    public class ActionRepository : IActionRepository
    {

        private readonly ApplicationDbContext _context;
        private readonly EmailConfiguration _email;

        public ActionRepository(ApplicationDbContext context, EmailConfiguration email)
        {
            _context = context;
            _email = email;
        }

        public bool SendAllMailDoc(string path, int RequestID)
        {

            List<int> pathList = path.Split(',').Select(int.Parse).ToList();


            string senderEmail = "practicetatvasoft@outlook.com";
            string senderPassword = "Tatvasoft@123";

            SmtpClient client = new SmtpClient("smtp.office365.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(senderEmail, senderPassword),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false
            };


            string? email = _context.Requestclients.FirstOrDefault(x => x.Requestid == RequestID).Email;
            var requestWiseFile = from requestFiles in _context.Requestwisefiles
                                  where pathList.Contains(requestFiles.Requestwisefileid)
                                  select new Requestwisefile
                                  {
                                      Requestwisefileid = requestFiles.Requestwisefileid,
                                      Filename = requestFiles.Filename,
                                      Requestid = requestFiles.Requestid,
                                  };
            string message = $@"<html>
                                <body>  
                                <h1>All Documents</h1>
                                </body>
                                </html>";
            if (email != null)
            {
                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail, "hallodoc@gmail.com"),
                    Subject = "Documents",
                    Body = message,
                    IsBodyHtml = true
                };
                foreach (var item in requestWiseFile)
                {
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Upload/req_" + item.Requestid + "/" + item.Filename);
                    Attachment attachment = new Attachment(filePath);
                    mailMessage.Attachments.Add(attachment);
                }
                mailMessage.To.Add(email)
;
                client.Send(mailMessage);
                foreach (var attachment in mailMessage.Attachments)
                {
                    attachment.Dispose();
                }
                return true;
            }
            return false;
        }
        public string GetFileName(int RequestWiseFileID)
        {
            var data = _context.Requestwisefiles.FirstOrDefault(e => e.Requestwisefileid == RequestWiseFileID);
            if (data != null)
            {


                return data.Filename;
            }
            else
            {
                return "";
            }

        }
        public bool DeleteDoc(int RequestWiseFileID)
        {
            var data = _context.Requestwisefiles.FirstOrDefault(e => e.Requestwisefileid == RequestWiseFileID);
            if (data != null)
            {
                BitArray bt = new BitArray(1);
                bt.Set(0, true);
                data.Isdeleted = bt;
                _context.Requestwisefiles.Update(data);
                _context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }

        }
        public bool ClearCase(int RequestID)
        {
            try
            {
                var requestData = _context.Requests.FirstOrDefault(e => e.Requestid == RequestID);
                if (requestData != null)
                {

                    requestData.Status = 10;
                    _context.Requests.Update(requestData);
                    _context.SaveChanges();

                    Requeststatuslog rsl = new Requeststatuslog
                    {
                        Requestid = RequestID,


                        Status = 10,
                        Createddate = DateTime.Now
                    };
                    _context.Requeststatuslogs.Add(rsl);
                    _context.SaveChanges();
                    return true;
                }
                else { return false; }
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public Healthprofessional SelectProfessionlByID(int VendorID)
        {
            return _context.Healthprofessionals.FirstOrDefault(e => e.Vendorid == VendorID);
        }
        public bool SendOrder(ViewSendOrderModel data)
        {
            try
            {
                Orderdetail od = new Orderdetail
                {
                    Requestid = data.RequestID,
                    Vendorid = data.VendorID,
                    Faxnumber = data.FaxNumber,
                    Email = data.Email,
                    Businesscontact = data.BusinessContact,
                    Prescription = data.Prescription,
                    Noofrefill = data.NoOFRefill,
                    Createddate = DateTime.Now,
                    Createdby = "001e35a5 - cd12 - 4ec8 - a077 - 95db9d54da0f"


                };
                _context.Orderdetails.Add(od);
                _context.SaveChanges(true);
                var req = _context.Requests.FirstOrDefault(e => e.Requestid == data.RequestID);
                _email.SendMail(data.Email, "New Order arrived", data.Prescription + "Request name" + req.Firstname);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }


        }
        #region Transfer_Provider
        public async Task<bool> TransferProvider(int RequestId, int ProviderId, string notes)
        {

            var request = await _context.Requests.FirstOrDefaultAsync(req => req.Requestid == RequestId);
            request.Physicianid = ProviderId;
            request.Status = 2;
            _context.Requests.Update(request);
            _context.SaveChanges();

            Requeststatuslog rsl = new Requeststatuslog();
            rsl.Requestid = RequestId;
            rsl.Physicianid = ProviderId;
            rsl.Notes = notes;

            rsl.Createddate = DateTime.Now;
            rsl.Transtophysicianid = ProviderId;
            rsl.Status = 2;
            _context.Requeststatuslogs.Add(rsl);
            _context.SaveChanges();

            return true;





        }
        #endregion

        public ViewCloseCaseModel CloseCaseData(int RequestID)
        {
            ViewCloseCaseModel alldata = new ViewCloseCaseModel();

            var result = from requestWiseFile in _context.Requestwisefiles
                         join request in _context.Requests on requestWiseFile.Requestid equals request.Requestid
                         join physician in _context.Physicians on request.Physicianid equals physician.Physicianid into physicianGroup
                         from phys in physicianGroup.DefaultIfEmpty()
                         join admin in _context.Admins on requestWiseFile.Adminid equals admin.Adminid into adminGroup
                         from adm in adminGroup.DefaultIfEmpty()
                         where request.Requestid == RequestID
                         select new
                         {

                             Uploader = requestWiseFile.Physicianid != null ? phys.Firstname :
                             (requestWiseFile.Adminid != null ? adm.Firstname : request.Firstname),
                             requestWiseFile.Filename,
                             requestWiseFile.Createddate,
                             requestWiseFile.Requestwisefileid

                         };
            List<Documents> doc = new List<Documents>();
            foreach (var item in result)
            {
                doc.Add(new Documents
                {
                    Createddate = item.Createddate,
                    Filename = item.Filename,
                    Uploader = item.Uploader,
                    RequestWiseFileID = item.Requestwisefileid

                });

            }
            alldata.documentslist = doc;
            Request req = _context.Requests.FirstOrDefault(r => r.Requestid == RequestID);

            alldata.Firstname = req.Firstname;
            alldata.RequestID = req.Requestid;
            alldata.ConfirmationNumber = req.Confirmationnumber;
            alldata.Lastname = req.Lastname;

            var reqcl = _context.Requestclients.FirstOrDefault(e => e.Requestid == RequestID);

            alldata.RC_Email = reqcl.Email;
            alldata.RC_Dob = new DateTime((int)reqcl.Intyear, DateTime.ParseExact(reqcl.Strmonth, "MMMM", new CultureInfo("en-US")).Month, (int)reqcl.Intdate);
            alldata.RC_FirstName = reqcl.Firstname;
            alldata.RC_LastName = reqcl.Lastname;
            alldata.RC_PhoneNumber = reqcl.Phonenumber;
            return alldata;
        }
        public bool EditForCloseCase(ViewCloseCaseModel model)
        {
            try

            {

                Requestclient client = _context.Requestclients.FirstOrDefault(E => E.Requestid == model.RequestID);

                if (client != null)
                {
                    client.Phonenumber = model.RC_PhoneNumber;
                    client.Email = model.RC_Email;
                    _context.Requestclients.Update(client);
                    _context.SaveChangesAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public bool CloseCase(int RequestID)
        {
            try
            {
                var requestData = _context.Requests.FirstOrDefault(e => e.Requestid == RequestID);
                if (requestData != null)
                {

                    requestData.Status = 9;
                    requestData.Modifieddate = DateTime.Now;

                    _context.Requests.Update(requestData);
                    _context.SaveChanges();

                    Requeststatuslog rsl = new Requeststatuslog
                    {
                        Requestid = RequestID,


                        Status = 9,
                        Createddate = DateTime.Now

                    };
                    _context.Requeststatuslogs.Add(rsl);
                    _context.SaveChanges();
                    return true;
                }
                else { return false; }
            }
            catch (Exception ex)
            {
                return false;
            }

        }
    }
}
